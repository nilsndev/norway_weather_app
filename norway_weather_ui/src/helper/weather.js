export function getWeatherEmoji(symbol) {
    
    switch (symbol) {        
        case 'clearsky_day':
            return '☀️';

        case 'clearsky_night':
            return '🌙';

        case 'fair_day':
            return '🌤️';

        case 'partlycloudy_day':
            return '⛅';

        case 'cloudy':
            return '☁️';

        case 'rain':
            return '🌧️';

        case 'snow':
            return '❄️';

        case 'fog':
            return '🌫️';

        default:
            return '🌡️';
    }
}