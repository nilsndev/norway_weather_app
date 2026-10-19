<script setup>
import { onMounted, onBeforeUnmount, ref } from 'vue'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import api from '../services/api'
import { getWeatherEmoji } from '../helper/weather'

const mapContainer = ref(null)
const selectedPosition = ref(null)
const currentWeather = ref(null)
const loading = ref(false)
const errorMessage = ref('')
const coordinateQuery = ref('')
const searchError = ref('')

let map
let marker
let requestId = 0

function formatTemperature(temperature) {
  return temperature == null ? '—' : `${Math.round(temperature)}°`
}

function formatWind(speed) {
  return speed == null ? '—' : `${speed} m/s`
}

function searchCoordinates() {
  const values = coordinateQuery.value.trim().split(/[\s,;]+/).map(Number)
  if (values.length !== 2 || values.some((value) => !Number.isFinite(value)) || Math.abs(values[0]) > 90 || Math.abs(values[1]) > 180) {
    searchError.value = 'Enter latitude and longitude, for example 59.91, 10.75.'
    return
  }

  searchError.value = ''
  const [latitude, longitude] = values
  map.setView([latitude, longitude], 9)
  selectPosition(latitude, longitude)
}

async function selectPosition(latitude, longitude) {
  selectedPosition.value = { latitude, longitude }
  currentWeather.value = null
  errorMessage.value = ''
  const activeRequest = ++requestId

  if (marker) {
    marker.setLatLng([latitude, longitude])
  } else {
    marker = L.circleMarker([latitude, longitude], {
      radius: 8,
      color: '#ffffff',
      weight: 2,
      fillColor: '#0284c7',
      fillOpacity: 1,
    }).addTo(map)
  }

  loading.value = true
  try {
    const query = new URLSearchParams({ latitude: String(latitude), longitude: String(longitude) })
    const { data } = await api.get(`/weather/current?${query}`)
    if (activeRequest === requestId) currentWeather.value = data
  } catch (error) {
    console.error('Failed to fetch weather:', error)
    if (activeRequest === requestId) errorMessage.value = 'Could not load weather for this location. Please try again.'
  } finally {
    if (activeRequest === requestId) loading.value = false
  }
}

onMounted(() => {
  map = L.map(mapContainer.value).setView([64.5, 11], 4)
  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
    maxZoom: 19,
  }).addTo(map)
  map.on('click', (event) => selectPosition(event.latlng.lat, event.latlng.lng))
})

onBeforeUnmount(() => {
  requestId += 1
  if (map) {
    map.remove()
    map = undefined
  }
})
</script>

<template>
  <div class="min-h-[calc(100vh-4rem)] p-4 sm:p-6 lg:p-8">
    <div class="mx-auto max-w-7xl">
      <header class="mb-4">
        <p class="text-sm font-medium text-slate-500">Norway Weather</p>
        <h1 class="mt-1 text-2xl font-semibold tracking-tight text-slate-900">Weather map</h1>
        <p class="mt-1 text-sm text-slate-500">Click the map to see current conditions.</p>
      </header>

      <div class="grid gap-4 lg:grid-cols-[minmax(0,1fr)_19rem]">
        <section class="relative overflow-hidden rounded-2xl bg-white shadow-sm ring-1 ring-slate-200" aria-label="Weather map">
          <form class="absolute left-3 right-3 top-3 z-[500] mx-auto flex max-w-lg gap-2 rounded-2xl bg-white/95 p-2 shadow-lg ring-1 ring-slate-200 backdrop-blur sm:left-1/2 sm:right-auto sm:w-[min(32rem,calc(100%-2rem))] sm:-translate-x-1/2" @submit.prevent="searchCoordinates">
            <label class="sr-only" for="map-coordinates">Search coordinates</label>
            <input id="map-coordinates" v-model="coordinateQuery" type="text" inputmode="decimal" placeholder="Coordinates (e.g. 59.91, 10.75)" class="min-w-0 flex-1 rounded-xl bg-transparent px-3 py-2 text-sm text-slate-800 outline-none placeholder:text-slate-400 focus:ring-2 focus:ring-sky-500" @input="searchError = ''" />
            <button type="submit" class="min-h-10 rounded-xl bg-sky-700 px-4 text-sm font-medium text-white transition hover:bg-sky-800 focus:outline-none focus:ring-2 focus:ring-sky-500 focus:ring-offset-2">Go</button>
          </form>
          <p v-if="searchError" class="absolute left-3 right-3 top-[4.5rem] z-[500] mx-auto max-w-lg rounded-xl bg-rose-50 px-3 py-2 text-xs text-rose-700 shadow sm:left-1/2 sm:right-auto sm:w-[min(32rem,calc(100%-2rem))] sm:-translate-x-1/2" role="alert">{{ searchError }}</p>
          <div ref="mapContainer" class="h-[58vh] min-h-[360px] w-full sm:h-[68vh] lg:h-[calc(100vh-12rem)] lg:min-h-[500px]"></div>
        </section>

        <section class="self-start rounded-2xl bg-white p-5 shadow-sm ring-1 ring-slate-200 lg:sticky lg:top-6" aria-live="polite" :aria-busy="loading">
          <div class="flex items-center justify-between gap-2">
            <h2 class="text-base font-semibold text-slate-900">Selected location</h2>
            <span v-if="loading" class="inline-flex items-center gap-2 text-xs font-medium text-sky-700">
              <span class="h-3.5 w-3.5 animate-spin rounded-full border-2 border-sky-200 border-t-sky-600 motion-reduce:animate-none" aria-hidden="true"></span>
              Updating
            </span>
          </div>

          <p v-if="!selectedPosition" class="mt-3 text-sm leading-6 text-slate-500">Select a point on the map or enter coordinates above.</p>

          <template v-else>
            <p class="mt-3 text-xs text-slate-500">{{ selectedPosition.latitude.toFixed(4) }}°, {{ selectedPosition.longitude.toFixed(4) }}°</p>

            <p v-if="errorMessage" class="mt-4 rounded-xl bg-rose-50 p-3 text-sm text-rose-700">{{ errorMessage }}</p>

            <template v-else-if="currentWeather">
              <div class="mt-5 flex items-center justify-between rounded-xl bg-sky-50 p-4">
                <div>
                  <p class="text-4xl font-semibold tracking-tight text-slate-900">{{ formatTemperature(currentWeather.temperature) }}</p>
                  <p class="mt-1 text-sm text-slate-600">{{ currentWeather.weatherSymbol || 'Current weather' }}</p>
                </div>
                <span class="text-4xl" aria-hidden="true">{{ getWeatherEmoji(currentWeather.weatherSymbol) }}</span>
              </div>

              <dl class="mt-4 grid grid-cols-2 gap-3">
                <div class="rounded-xl bg-slate-50 p-3">
                  <dt class="text-xs text-slate-500">Wind</dt>
                  <dd class="mt-1 text-sm font-semibold text-slate-800">{{ formatWind(currentWeather.windSpeed) }}</dd>
                </div>
                <div v-if="currentWeather.humidity != null" class="rounded-xl bg-slate-50 p-3">
                  <dt class="text-xs text-slate-500">Humidity</dt>
                  <dd class="mt-1 text-sm font-semibold text-slate-800">{{ currentWeather.humidity }}%</dd>
                </div>
              </dl>
            </template>
          </template>
        </section>
      </div>
    </div>
  </div>
</template>
