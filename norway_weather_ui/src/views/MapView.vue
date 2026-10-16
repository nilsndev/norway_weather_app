<script setup>
import { onMounted, onBeforeUnmount, ref } from 'vue'
import { useRouter } from 'vue-router'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import { useAuthStore } from '../stores/auth'
import api from '../services/api'

const auth = useAuthStore()
const router = useRouter()
const mapContainer = ref(null)
const selectedPosition = ref(null)
const loading = ref(false)
const errorMessage = ref('')

let map
let marker

async function selectPosition(latitude, longitude) {
  selectedPosition.value = { latitude, longitude }
  errorMessage.value = ''

  if (marker) {
    marker.setLatLng([latitude, longitude])
  } else {
    marker = L.circleMarker([latitude, longitude], {
      radius: 8,
      color: '#ffffff',
      weight: 2,
      fillColor: '#2563eb',
      fillOpacity: 1,
    }).addTo(map)
  }

  loading.value = true
  try {
    const query = new URLSearchParams({
      latitude: String(latitude),
      longitude: String(longitude),
    })
    /*
    const [currentResponse, forecastResponse] = await Promise.all([
      api.get(`/weather/current?${query}`),
      api.get(`/weather/forecast?${query}`),
    ])
    */

    const currentResponse = await api.get(`/weather/current?${query}`);
    const currentWeather = currentResponse.data
    console.log('Current weather:', currentWeather)
    //const forecast = forecastResponse.data
    //console.log('24-hour forecast:', forecast)
    
  } catch (error) {
    console.error('Failed to fetch weather:', error)
    errorMessage.value = 'Could not fetch weather. Check the API URL and backend connection.'
  } finally {
    loading.value = false
  }
}

async function logout() {
  auth.logout()
  await router.push('/login')
}

onMounted(() => {
  map = L.map(mapContainer.value).setView([64.5, 11], 4)
  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
    maxZoom: 19,
  }).addTo(map)
  map.on('click', (event) => {
    selectPosition(event.latlng.lat, event.latlng.lng)
  })
})

onBeforeUnmount(() => {
  if (map) {
    map.remove()
    map = undefined
  }
})
</script>

<template>
  <div class="min-h-screen bg-slate-50 text-slate-800">
    <nav class="border-b border-slate-200 bg-white">
      <div class="mx-auto flex max-w-6xl flex-wrap items-center justify-between gap-3 px-4 py-3 sm:px-6">
        <span class="font-semibold text-slate-900">Weather Explorer</span>
        <div class="flex items-center gap-4 text-sm">
          <span class="text-slate-600">{{ auth.user?.email }}</span>
          <button class="rounded-lg border border-slate-300 px-3 py-1.5 font-medium text-slate-700 hover:bg-slate-100" @click="logout">Log out</button>
        </div>
      </div>
    </nav>
    <main class="mx-auto max-w-6xl px-4 py-8 sm:px-6">
      <header class="mb-6 flex flex-col items-start justify-between gap-3 sm:flex-row sm:items-center">
        <div>
          <h1 class="text-3xl font-bold tracking-tight text-slate-900">Weather Explorer</h1>
          <p class="mt-1 text-slate-600">Click anywhere on the map to get weather information.</p>
        </div>
        <span class="rounded-full bg-slate-200 px-3 py-2 text-sm text-slate-700" :class="{ 'bg-green-100 text-green-800': selectedPosition }">
          {{ selectedPosition ? 'Location selected' : 'Select a location' }}
        </span>
      </header>
      <section class="overflow-hidden rounded-2xl border border-slate-200 bg-white">
        <div ref="mapContainer" class="h-[400px] w-full sm:h-[520px]"></div>
      </section>
      <section class="mt-6 rounded-2xl border border-slate-200 bg-white p-6">
        <h2 class="text-lg font-semibold text-slate-900">Selected location</h2>
        <p v-if="!selectedPosition" class="mt-2 text-slate-600">Click on the map to select a location.</p>
        <div v-else class="mt-4 flex flex-wrap gap-10">
          <div class="flex flex-col gap-1"><span class="text-sm text-slate-500">Latitude</span><strong>{{ selectedPosition.latitude.toFixed(5) }}</strong></div>
          <div class="flex flex-col gap-1"><span class="text-sm text-slate-500">Longitude</span><strong>{{ selectedPosition.longitude.toFixed(5) }}</strong></div>
        </div>
        <p v-if="loading" class="mt-5 text-slate-600">Fetching weather data...</p>
        <p v-if="errorMessage" class="mt-4 text-red-700">{{ errorMessage }}</p>
        <p class="mt-4 text-sm text-slate-500">Weather responses are currently logged in the browser console. Open Developer Tools → Console to inspect them.</p>
      </section>
    </main>
  </div>
</template>
