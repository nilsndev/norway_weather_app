
using DotNetEnv;
using norway_weather_api.Services;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var email = Environment.GetEnvironmentVariable("METNO_CONTACT_EMAIL")
    ?? throw new InvalidOperationException(
        "METNO_CONTACT_EMAIL is missing.");

builder.Services.AddHttpClient<IWeatherService, WeatherService>(
    client =>
    {
        client.BaseAddress = new Uri("https://api.met.no/");

        client.DefaultRequestHeaders.UserAgent.ParseAdd(
            $"norway_weather_api/1.0 (contact: {email})");
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
