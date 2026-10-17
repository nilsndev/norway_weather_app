export function formatWeatherTime(time) {
    return new Date(time).toLocaleTimeString('no-NO', {
        hour: '2-digit',
        minute: '2-digit',
        timeZone: 'Europe/Oslo'
    });
}