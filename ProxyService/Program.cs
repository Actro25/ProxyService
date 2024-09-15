using ProxyService.Interfaces;
using ProxyService.Middlewars;
using ProxyService.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("../logs/WebAppLog-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

Log.Information("Starting web application");

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Services.AddSerilog();
builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Реєструємо HttpClient та сервіси
builder.Services.AddHttpClient();
builder.Services.AddScoped<IUserService, UserService>(); // Реєструємо наш сервіс

// Додаємо HttpClient в DI-контейнер
builder.Services.AddHttpClient();

var app = builder.Build();

//app.UseMiddleware<HTTPHeaderMiddlewar>();
//app.UseHeaderValidation();

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

app.Run();
