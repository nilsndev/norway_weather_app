import { computed, ref } from 'vue'

const savedLocale = localStorage.getItem('norway-weather-locale')
const locale = ref(['no', 'en', 'de'].includes(savedLocale) ? savedLocale : 'en')
document.documentElement.lang = locale.value

const messages = {
  no: {
    language: 'Språk', home: 'Hjem', cities: 'Byer', map: 'Kart', explore: 'Utforsk', logout: 'Logg ut', openMenu: 'Åpne meny', closeMenu: 'Lukk meny', closeNavigation: 'Lukk navigasjonsmeny',
    tagline: 'Værvarsel og forhold', homeTitle: 'Været i {city}', homeDescription: 'Søk etter en by eller et sted i Norge.', searchPlace: 'Søk etter by eller sted i Norge', search: 'Søk', searching: 'Søker …', loadingWeather: 'Henter vær …',
    currentWeather: 'Nåværende vær', forecast: 'Værvarsel', conditions: 'Forhold', temperature: 'Temperatur', wind: 'Vind', humidity: 'Luftfuktighet', noForecast: 'Ingen værvarsel tilgjengelig', weatherFailed: 'Kunne ikke hente værdata. Prøv igjen.',
    citiesTitle: 'Byer i Norge', citiesDescription: 'Søk etter et sted i Norge for å legge det til.', addPlace: 'Søk etter by eller sted i Norge', loadingCities: 'Henter vær for byene …', viewConditions: 'Vis værforhold',
    mapTitle: 'Værkart for Norge', mapDescription: 'Kartet er sentrert på Norge. Klikk i Norge for å se været.', mapSearch: 'Søk etter sted i Norge eller koordinater', mapPlaceholder: 'Sted eller koordinater (59.91, 10.75)', go: 'Vis', backToNorway: 'Tilbake til Norge', outsideArea: 'Dette stedet ligger utenfor området appen dekker.', invalidCoordinates: 'Skriv inn et sted i Norge eller koordinater innenfor Norge.', selectedLocation: 'Valgt sted', weatherLoadFailed: 'Kunne ikke hente vær for dette stedet. Prøv igjen.',
    closeDetails: 'Lukk værdetaljer', weatherIn: 'Været i {city}', weatherConditions: 'Værforhold',
    createAccount: 'Opprett konto', registerDescription: 'Registrer deg for å se værkartet.', email: 'E-post', password: 'Passord', confirmPassword: 'Bekreft passord', register: 'Registrer', creatingAccount: 'Oppretter konto …', alreadyRegistered: 'Har du allerede en konto?', signIn: 'Logg inn', passwordsMismatch: 'Passordene er ikke like.', registrationFailed: 'Kunne ikke opprette konto. Kontroller opplysningene og prøv igjen.',
    signInTitle: 'Logg inn', signInDescription: 'Logg inn for å utforske værkartet.', signingIn: 'Logger inn …', incorrectCredentials: 'E-postadressen eller passordet er feil.', signInFailed: 'Kunne ikke logge inn. Prøv igjen.', newHere: 'Ny bruker?', createAccountLink: 'Opprett en konto',
    noResults: 'Ingen steder funnet i Norge. Prøv et annet søk.', searchFailed: 'Kunne ikke søke etter steder. Prøv igjen.',
    passRequired: 'Passord er påkrevd.', passLength: 'Passordet må være minst {count} tegn langt.', passUpper: 'Passordet må inneholde minst én stor bokstav.', passLower: 'Passordet må inneholde minst én liten bokstav.', passDigit: 'Passordet må inneholde minst ett tall.', passNonAlnum: 'Passordet må inneholde minst ett spesialtegn (for eksempel ! eller #).', passUnique: 'Passordet må inneholde minst {count} ulike tegn.', emailTaken: 'E-postadressen er allerede registrert.', invalidEmail: 'Skriv inn en gyldig e-postadresse.',
  },
  en: {
    language: 'Language', home: 'Home', cities: 'Cities', map: 'Map', explore: 'Explore', logout: 'Log out', openMenu: 'Open menu', closeMenu: 'Close menu', closeNavigation: 'Close navigation menu',
    tagline: 'Forecast and conditions', homeTitle: '{city} weather', homeDescription: 'Search for a city or place in Norway.', searchPlace: 'Search for a city or place in Norway', search: 'Search', searching: 'Searching …', loadingWeather: 'Loading weather …',
    currentWeather: 'Current weather', forecast: 'Forecast', conditions: 'Conditions', temperature: 'Temperature', wind: 'Wind', humidity: 'Humidity', noForecast: 'No forecasts available', weatherFailed: 'Could not get weather data. Please try again.',
    citiesTitle: 'Cities in Norway', citiesDescription: 'Search for a place in Norway to add it.', addPlace: 'Search for a city or place in Norway', loadingCities: 'Loading city weather …', viewConditions: 'View current conditions',
    mapTitle: 'Weather map of Norway', mapDescription: 'The map is centered on Norway. Click within Norway to see the weather.', mapSearch: 'Search for a place in Norway or coordinates', mapPlaceholder: 'Place or coordinates (59.91, 10.75)', go: 'Go', backToNorway: 'Back to Norway', outsideArea: 'This place is outside the area covered by the app.', invalidCoordinates: 'Enter a place in Norway or coordinates within Norway.', selectedLocation: 'Selected location', weatherLoadFailed: 'Could not load weather for this place. Please try again.',
    closeDetails: 'Close weather details', weatherIn: 'Weather in {city}', weatherConditions: 'Weather conditions',
    createAccount: 'Create your account', registerDescription: 'Register to access the weather map.', email: 'Email', password: 'Password', confirmPassword: 'Confirm password', register: 'Register', creatingAccount: 'Creating account …', alreadyRegistered: 'Already registered?', signIn: 'Sign in', passwordsMismatch: 'Passwords do not match.', registrationFailed: 'Unable to create your account. Please check your details and try again.',
    signInTitle: 'Sign in', signInDescription: 'Sign in to explore the weather map.', signingIn: 'Signing in …', incorrectCredentials: 'The email or password is incorrect.', signInFailed: 'Unable to sign in. Please try again.', newHere: 'New here?', createAccountLink: 'Create an account',
    noResults: 'No places found in Norway. Try another search.', searchFailed: 'Could not search places. Try again.',
    passRequired: 'Password is required.', passLength: 'Password must be at least {count} characters long.', passUpper: 'Password must include at least one uppercase letter.', passLower: 'Password must include at least one lowercase letter.', passDigit: 'Password must include at least one number.', passNonAlnum: 'Password must include at least one special character (for example ! or #).', passUnique: 'Password must contain at least {count} different characters.', emailTaken: 'This email address is already registered.', invalidEmail: 'Enter a valid email address.',
  },
  de: {
    language: 'Sprache', home: 'Startseite', cities: 'Städte', map: 'Karte', explore: 'Entdecken', logout: 'Abmelden', openMenu: 'Menü öffnen', closeMenu: 'Menü schließen', closeNavigation: 'Navigation schließen',
    tagline: 'Vorhersage und Wetterlage', homeTitle: 'Wetter in {city}', homeDescription: 'Suche nach einer Stadt oder einem Ort in Norwegen.', searchPlace: 'Stadt oder Ort in Norwegen suchen', search: 'Suchen', searching: 'Suche …', loadingWeather: 'Wetter wird geladen …',
    currentWeather: 'Aktuelles Wetter', forecast: 'Vorhersage', conditions: 'Wetterlage', temperature: 'Temperatur', wind: 'Wind', humidity: 'Luftfeuchtigkeit', noForecast: 'Keine Vorhersage verfügbar', weatherFailed: 'Wetterdaten konnten nicht geladen werden. Bitte versuche es erneut.',
    citiesTitle: 'Städte in Norwegen', citiesDescription: 'Suche nach einem Ort in Norwegen, um ihn hinzuzufügen.', addPlace: 'Stadt oder Ort in Norwegen suchen', loadingCities: 'Stadtwetter wird geladen …', viewConditions: 'Aktuelle Wetterlage anzeigen',
    mapTitle: 'Wetterkarte von Norwegen', mapDescription: 'Die Karte ist auf Norwegen zentriert. Klicke innerhalb Norwegens, um das Wetter zu sehen.', mapSearch: 'Ort in Norwegen oder Koordinaten suchen', mapPlaceholder: 'Ort oder Koordinaten (59.91, 10.75)', go: 'Anzeigen', backToNorway: 'Zurück nach Norwegen', outsideArea: 'Dieser Ort liegt außerhalb des von der App abgedeckten Gebiets.', invalidCoordinates: 'Gib einen Ort oder Koordinaten innerhalb Norwegens ein.', selectedLocation: 'Ausgewählter Ort', weatherLoadFailed: 'Wetter für diesen Ort konnte nicht geladen werden. Bitte versuche es erneut.',
    closeDetails: 'Wetterdetails schließen', weatherIn: 'Wetter in {city}', weatherConditions: 'Wetterlage',
    createAccount: 'Konto erstellen', registerDescription: 'Registriere dich, um die Wetterkarte zu nutzen.', email: 'E-Mail', password: 'Passwort', confirmPassword: 'Passwort bestätigen', register: 'Registrieren', creatingAccount: 'Konto wird erstellt …', alreadyRegistered: 'Bereits registriert?', signIn: 'Anmelden', passwordsMismatch: 'Die Passwörter stimmen nicht überein.', registrationFailed: 'Konto konnte nicht erstellt werden. Bitte überprüfe deine Angaben und versuche es erneut.',
    signInTitle: 'Anmelden', signInDescription: 'Melde dich an, um die Wetterkarte zu erkunden.', signingIn: 'Anmeldung läuft …', incorrectCredentials: 'E-Mail-Adresse oder Passwort ist falsch.', signInFailed: 'Anmeldung fehlgeschlagen. Bitte versuche es erneut.', newHere: 'Neu hier?', createAccountLink: 'Konto erstellen',
    noResults: 'Keine Orte in Norwegen gefunden. Versuche eine andere Suche.', searchFailed: 'Orte konnten nicht gesucht werden. Bitte versuche es erneut.',
    passRequired: 'Ein Passwort ist erforderlich.', passLength: 'Das Passwort muss mindestens {count} Zeichen lang sein.', passUpper: 'Das Passwort muss mindestens einen Großbuchstaben enthalten.', passLower: 'Das Passwort muss mindestens einen Kleinbuchstaben enthalten.', passDigit: 'Das Passwort muss mindestens eine Zahl enthalten.', passNonAlnum: 'Das Passwort muss mindestens ein Sonderzeichen enthalten (zum Beispiel ! oder #).', passUnique: 'Das Passwort muss mindestens {count} verschiedene Zeichen enthalten.', emailTaken: 'Diese E-Mail-Adresse ist bereits registriert.', invalidEmail: 'Gib eine gültige E-Mail-Adresse ein.',
  },
}

export function useLocale() {
  const t = (key, values = {}) => {
    let value = messages[locale.value][key] ?? messages.en[key] ?? key
    for (const [name, replacement] of Object.entries(values)) value = value.replaceAll(`{${name}}`, replacement)
    return value
  }
  function setLocale(value) {
    if (!messages[value]) return
    locale.value = value
    localStorage.setItem('norway-weather-locale', value)
    document.documentElement.lang = value
  }
  return { locale: computed(() => locale.value), t, setLocale }
}

export function isNorwayLocation(latitude, longitude) {
  // A conservative, rectangular Norway envelope for map clicks and raw coordinates.
  return latitude >= 57.8 && latitude <= 71.5 && longitude >= 4.0 && longitude <= 31.5
}
