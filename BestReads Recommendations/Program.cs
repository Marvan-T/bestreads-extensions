using BestReads_Recommendations;
using BestReads_Recommendations.Infrastructure.Data;

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
services.RegisterDatabaseDependencies();
services.RegisterDependencies();
services.AddDefaultAutoMapper();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();