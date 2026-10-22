
using DotNetEnv;
using norway_weather_api.Services;
using norway_weather_api.Data;
using Microsoft.AspNetCore.Identity;
using norway_weather_api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
    ?? throw new InvalidOperationException("JWT_SECRET_KEY is missing.");


builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Norway Weather API",
        Version = "v1"
    });

});

builder.Services.AddMemoryCache();

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

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = false;
        options.RequireHttpsMetadata = false;
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["norway_weather_auth"];
                return Task.CompletedTask;
            }
        };
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],

                IssuerSigningKey = new SymmetricSecurityKey(
                    Convert.FromBase64String(jwtKey)
                )
            };

    });


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

builder.Services.AddHttpClient<IGeocodingService, GeocodingService>(
    client =>
    {
        client.BaseAddress = new Uri(
            "https://nominatim.openstreetmap.org/");

        client.DefaultRequestHeaders.UserAgent.ParseAdd(
        $"norway_weather_api/1.0 (contact: {email})");

        client.Timeout = TimeSpan.FromSeconds(10);
    });

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? ["http://localhost:3000", "http://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("VueApp", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Norway Weather API v1");
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("VueApp");

app.Use(async (context, next) =>
{
    var isUnsafeMethod = HttpMethods.IsPost(context.Request.Method)
        || HttpMethods.IsPut(context.Request.Method)
        || HttpMethods.IsPatch(context.Request.Method)
        || HttpMethods.IsDelete(context.Request.Method);
    var origin = context.Request.Headers.Origin.ToString();

    if (isUnsafeMethod
        && !string.IsNullOrEmpty(origin)
        && !allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase))
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return;
    }

    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
