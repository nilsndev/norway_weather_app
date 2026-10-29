namespace norway_weather_api.Services;

/// <summary>
/// Coarse coverage envelope around mainland Norway, including nearby Sweden.
/// This is a rectangular service boundary, not a precise national border.
/// </summary>
public static class WeatherAreaPolicy
{
    public static bool Includes(double latitude, double longitude) =>
        latitude is >= 57.8 and <= 71.5
        && longitude is >= 4.0 and <= 31.5;
}
