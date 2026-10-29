<script setup>
import { onMounted, onBeforeUnmount, ref, watch } from 'vue'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import api from '../services/api'
import { getWeatherEmoji } from '../helper/weather'
import { useLocale, isNorwayLocation } from '../i18n'

const mapContainer = ref(null)
const selectedPosition = ref(null)
const currentWeather = ref(null)
const loading = ref(false)
const errorMessage = ref('')
const coordinateQuery = ref('')
const searchError = ref('')
const { t } = useLocale()
const placeResults = ref([])
let map
let marker
let popupContent
let requestId = 0
let searchTimer
let placeRequestId = 0
let lastChosenPlaceKey = ''

watch(coordinateQuery, (value) => {
  clearTimeout(searchTimer)
  placeRequestId += 1
  placeResults.value = []
  const term = value.trim()
  if (term.length < 2 || /^[+-]?(?:\d+\.?\d*|\.\d+)[,;\s]+[+-]?(?:\d+\.?\d*|\.\d+)$/.test(term)) return
  const id = placeRequestId
  searchTimer = setTimeout(async () => {
    try {
      const { data } = await api.get('/city/search', { params: { query: term } })
      if (id === placeRequestId) {
        const seen = new Set()
        placeResults.value = (Array.isArray(data) ? data : []).filter((place) => {
          if (place.countryCode !== 'no') return false
          const key = `${place.latitude},${place.longitude}`
          if (seen.has(key)) return false
          seen.add(key)
          return true
        })
      }
    } catch (error) { console.error('Failed to search places', error) }
  }, 300)
})

function formatTemperature(value) { return value == null ? '—' : `${Math.round(value)}°` }
function formatWind(value) { return value == null ? '—' : `${value} m/s` }

function renderPopup() {
  if (!popupContent || !selectedPosition.value) return
  const position = selectedPosition.value
  const weather = currentWeather.value
  const card = document.createElement('div')
  card.className = 'w-64 p-1 font-sans text-slate-800'
  const heading = document.createElement('p')
  heading.className = 'text-xs font-medium uppercase tracking-wide text-sky-700'
  heading.textContent = position.name || t('selectedLocation')
  const coords = document.createElement('p')
  coords.className = 'mt-1 text-xs text-slate-500'
  coords.textContent = `${position.latitude.toFixed(4)}°, ${position.longitude.toFixed(4)}°`
  card.append(heading, coords)

  if (loading.value) {
    const status = document.createElement('p')
    status.className = 'mt-4 text-sm text-slate-500'
    status.textContent = t('loadingWeather')
    card.append(status)
  } else if (errorMessage.value) {
    const error = document.createElement('p')
    error.className = 'mt-3 rounded-xl bg-rose-50 p-3 text-sm text-rose-700'
    error.textContent = errorMessage.value
    card.append(error)
  } else if (weather) {
    const summary = document.createElement('div')
    summary.className = 'mt-3 flex items-center justify-between rounded-xl bg-sky-50 p-3'
    const details = document.createElement('div')
    const temperature = document.createElement('p')
    temperature.className = 'text-3xl font-semibold tracking-tight text-slate-900'
    temperature.textContent = formatTemperature(weather.temperature)
    details.append(temperature)
    const icon = document.createElement('span')
    icon.className = 'text-4xl'
    icon.setAttribute('aria-hidden', 'true')
    icon.textContent = getWeatherEmoji(weather.weatherSymbol)
    summary.append(details, icon)
    const metrics = document.createElement('dl')
    metrics.className = 'mt-3 grid grid-cols-2 gap-2'
    for (const [label, value] of [['Wind', formatWind(weather.windSpeed)], ['Humidity', weather.humidity == null ? '—' : `${weather.humidity}%`]]) {
      const cell = document.createElement('div')
      cell.className = 'rounded-xl bg-slate-50 p-2'
      const term = document.createElement('dt')
      term.className = 'text-xs text-slate-500'
      term.textContent = label
      const description = document.createElement('dd')
      description.className = 'mt-1 text-sm font-semibold text-slate-800'
      description.textContent = value
      cell.append(term, description)
      metrics.append(cell)
    }
    card.append(summary, metrics)
  }
  popupContent.replaceChildren(card)
}

function searchCoordinates() {
  const values = coordinateQuery.value.trim().split(/[\s,;]+/).map(Number)
  if (values.length === 2 && values.every(Number.isFinite) && Math.abs(values[0]) <= 90 && Math.abs(values[1]) <= 180) {
    if (!isNorwayLocation(values[0], values[1])) { searchError.value = t('outsideArea'); return }
    searchError.value = ''
    map.setView(values, 9)
    selectPosition(values[0], values[1])
    coordinateQuery.value = ''
  } else if (placeResults.value.length) choosePlace(placeResults.value[0])
  else searchError.value = t('invalidCoordinates')
}

function choosePlace(place) {
  if (!isNorwayLocation(place.latitude, place.longitude)) { searchError.value = t('outsideArea'); return }
  const key = `${place.latitude},${place.longitude}`
  if (key === lastChosenPlaceKey) return
  lastChosenPlaceKey = key
  coordinateQuery.value = ''
  placeResults.value = []
  searchError.value = ''
  map.setView([place.latitude, place.longitude], 9)
  selectPosition(place.latitude, place.longitude, place.displayName.split(',')[0])
}

async function selectPosition(latitude, longitude, name = '') {
  if (!isNorwayLocation(latitude, longitude)) { searchError.value = t('outsideArea'); return }
  if (selectedPosition.value?.latitude === latitude && selectedPosition.value?.longitude === longitude && (loading.value || currentWeather.value)) return
  selectedPosition.value = { latitude, longitude, name }
  currentWeather.value = null
  errorMessage.value = ''
  const activeRequest = ++requestId
  if (marker) marker.setLatLng([latitude, longitude])
  else marker = L.circleMarker([latitude, longitude], { radius: 8, color: '#ffffff', weight: 2, fillColor: '#0284c7', fillOpacity: 1 }).addTo(map)

  loading.value = true
  popupContent = document.createElement('div')
  marker.bindPopup(popupContent, { maxWidth: 300, minWidth: 260 }).openPopup()
  renderPopup()
  try {
    const params = { latitude: String(latitude), longitude: String(longitude) }
    const { data } = await api.get('/weather/current', { params })
    if (activeRequest === requestId) {
      currentWeather.value = data
      renderPopup()
    }
  } catch (error) {
    console.error('Failed to fetch weather:', error)
    if (activeRequest === requestId) {
      errorMessage.value = t('weatherLoadFailed')
      renderPopup()
    }
  } finally {
    if (activeRequest === requestId) {
      loading.value = false
      renderPopup()
    }
  }
}

onMounted(() => {
  map = L.map(mapContainer.value).setView([64.5, 11], 5)
  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', { attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>', maxZoom: 19 }).addTo(map)
  map.on('click', (event) => selectPosition(event.latlng.lat, event.latlng.lng))
})

onBeforeUnmount(() => {
  requestId += 1
  clearTimeout(searchTimer)
  if (map) { map.remove(); map = undefined }
})
</script>

<template>
  <div class="min-h-[calc(100vh-4rem)] p-4 sm:p-6 lg:p-8">
    <div class="mx-auto max-w-7xl">
      <header class="mb-4">
        <p class="text-sm font-medium text-slate-500">Norway Weather</p>
        <h1 class="mt-1 text-2xl font-semibold tracking-tight text-slate-900">{{ t('mapTitle') }}</h1>
        <p class="mt-1 text-sm text-slate-500">{{ t('mapDescription') }}</p>
        <button type="button" class="mt-2 rounded-lg bg-white px-3 py-2 text-sm font-medium text-sky-800 ring-1 ring-slate-200" @click="map?.setView([64.5, 11], 5)">{{ t('backToNorway') }}</button>
      </header>
      <section class="relative overflow-hidden rounded-2xl bg-white shadow-sm ring-1 ring-slate-200" :aria-label="t('mapTitle')">
        <form class="absolute left-3 right-3 top-3 z-[500] mx-auto flex max-w-lg gap-2 rounded-2xl bg-white/95 p-2 shadow-lg ring-1 ring-slate-200 backdrop-blur sm:left-1/2 sm:right-auto sm:w-[min(32rem,calc(100%-2rem))] sm:-translate-x-1/2" @submit.prevent="searchCoordinates">
          <label class="sr-only" for="map-coordinates">{{ t('mapSearch') }}</label>
          <input id="map-coordinates" v-model="coordinateQuery" type="text" autocomplete="off" :placeholder="t('mapPlaceholder')" class="min-w-0 flex-1 rounded-xl bg-transparent px-3 py-2 text-sm text-slate-800 outline-none placeholder:text-slate-400 focus:ring-2 focus:ring-sky-500" @input="searchError = ''" @keydown.esc="placeResults = []" />
          <button type="submit" class="min-h-10 rounded-xl bg-sky-700 px-4 text-sm font-medium text-white transition hover:bg-sky-800 focus:outline-none focus:ring-2 focus:ring-sky-500 focus:ring-offset-2">{{ t('go') }}</button>
        </form>
        <ul v-if="placeResults.length" class="absolute left-3 right-3 top-[4.2rem] z-[501] mx-auto max-w-lg rounded-xl border border-slate-200 bg-white p-1 shadow-xl sm:left-1/2 sm:right-auto sm:w-[min(32rem,calc(100%-2rem))] sm:-translate-x-1/2" role="listbox"><li v-for="place in placeResults" :key="`${place.latitude}-${place.longitude}`"><button type="button" class="w-full rounded-lg px-3 py-2 text-left text-sm text-slate-700 hover:bg-sky-50" @click="choosePlace(place)">{{ place.displayName }}</button></li></ul>
        <p v-if="searchError" class="absolute left-3 right-3 top-[4.5rem] z-[500] mx-auto max-w-lg rounded-xl bg-rose-50 px-3 py-2 text-xs text-rose-700 shadow sm:left-1/2 sm:right-auto sm:w-[min(32rem,calc(100%-2rem))] sm:-translate-x-1/2" role="alert">{{ searchError }}</p>
        <div ref="mapContainer" class="h-[58vh] min-h-[360px] w-full sm:h-[68vh] lg:h-[calc(100vh-12rem)] lg:min-h-[500px]"></div>
      </section>
    </div>
  </div>
</template>
