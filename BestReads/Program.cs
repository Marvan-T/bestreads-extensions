using BestReads;
using BestReads.Infrastructure.Data;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

var mongoSettings = new MongoDbSettings();
builder.Configuration.GetSection("MongoDbSettings").Bind(mongoSettings);
builder.Services.AddSingleton(mongoSettings);
services.SetupMongoDB(mongoSettings);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
services.RegisterDatabaseDependencies();
services.RegisterInfrastructureDependencies();
services.RegisterDependencies();
services.AddDefaultAutoMapper();
services.SetupRefit(builder.Configuration);

builder.Host.SetupSerilog();

var app = builder.Build();

// Initialize indexes
using (var scope = app.Services.CreateScope())
{
    var indexInitializer = scope.ServiceProvider.GetRequiredService<IndexInitializer>();
    indexInitializer.Initialize();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

app.Run();
