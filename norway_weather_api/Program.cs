
using DotNetEnv;
using norway_weather_api.Services;
using norway_weather_api.Data;
using Microsoft.AspNetCore.Identity;
using norway_weather_api.Models;
using Microsoft.EntityFrameworkCore;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString =
    Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
    ?? throw new InvalidOperationException(
        "POSTGRES_CONNECTION_STRING is missing.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

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

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("VueApp", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); 
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("VueApp");

app.MapControllers();

app.Run();
