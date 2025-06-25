using Serilog;
using Prometheus;
var builder = WebApplication.CreateBuilder(args);

// Configura Serilog para registrar en un archivo .log
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information
    .WriteTo.Console()
    .WriteTo.File("logs/app-log.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHttpsRedirection();
app.UseHttpMetrics();

app.UseAuthorization();

app.MapControllers();
app.MapMetrics();

app.Run();
