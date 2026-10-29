export function getWeatherEmoji(symbol) {
  const condition = symbol?.split('_')[0]

  if (condition?.startsWith('heavyrain')) return '🌧️🌧️'

  switch (symbol) {
    case 'clearsky_day': return '☀️'
    case 'clearsky_night': return '🌙'
    case 'fair_day': return '🌤️'
    case 'partlycloudy_day': return '⛅'
    case 'partlycloudy_night': return '🌙☁️'
    case 'cloudy': return '☁️'
    case 'rain': return '🌧️'
    case 'snow': return '❄️'
    case 'fog': return '🌫️'
    default: return '🌡️'
  }
}
