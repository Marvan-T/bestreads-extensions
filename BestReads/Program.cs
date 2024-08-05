using BestReads;
using BestReads.Infrastructure.Data;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

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

builder.Host.UseSerilog(
    (context, configuration) =>
    {
        configuration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext();

        if (context.HostingEnvironment.IsProduction())
        {
            try
            {
                configuration.WriteTo.AzureBlobStorage(
                    connectionString: context.Configuration["AZURE_BLOB_STORAGE_CONNECTION_STRING"],
                    storageContainerName: "best-reads-logs",
                    storageFileName: "log-{yyyy}/{MM}/{dd}.json",
                    formatter: new JsonFormatter(renderMessage: true, formatProvider: null)
                );
            }
            catch (Exception ex)
            {
                // Fallback to stderr for Docker logging
                configuration.WriteTo.Console(
                    restrictedToMinimumLevel: LogEventLevel.Warning,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                );

                // Log the configuration error
                Console.Error.WriteLine(
                    $"Failed to configure Azure Blob Storage logging: {ex.Message}"
                );
            }
        }
        else
        {
            configuration.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Debug);
            configuration.WriteTo.File(
                formatter: new JsonFormatter(renderMessage: true, formatProvider: null),
                path: "./logs/log-.json",
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true,
                restrictedToMinimumLevel: LogEventLevel.Verbose
            );
        }
    }
);

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
