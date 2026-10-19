<script setup>
import { ref } from 'vue'
import api from '../services/api'
import { getWeatherEmoji } from '../helper/weather'
import LocationSearch from '../components/LocationSearch.vue'
import WeatherDetailModal from '../components/WeatherDetailModal.vue'

const cities = ref([
  { name: 'Oslo', latitude: 59.9139, longitude: 10.7522 },
  { name: 'Bergen', latitude: 60.3913, longitude: 5.3221 },
  { name: 'Stavanger', latitude: 58.97, longitude: 5.7331 },
  { name: 'Trondheim', latitude: 63.4305, longitude: 10.3951 },
  { name: 'Drammen', latitude: 59.7439, longitude: 10.2045 },
])
const loading = ref(true)
const searchText = ref('')
const selectedCity = ref(null)

async function loadWeather(city) {
  try {
    const { data } = await api.get('/weather/current', { params: { latitude: String(city.latitude), longitude: String(city.longitude) } })
    Object.assign(city, data)
  } catch (error) { console.error(`Failed to get weather for ${city.name}`, error) }
}

function addCity(place) {
  const existing = cities.value.find((city) => Math.abs(city.latitude - place.latitude) < 0.001 && Math.abs(city.longitude - place.longitude) < 0.001)
  searchText.value = ''
  if (existing) { selectedCity.value = existing; return }
  const city = { ...place, id: `${place.latitude}-${place.longitude}`, temperature: null, weatherSymbol: '', windSpeed: null, humidity: null, loadingWeather: true }
  // Keep the five built-in cities and one replaceable search result.
  if (cities.value.length > 5) cities.value.splice(5, 1, city)
  else cities.value.push(city)
  // Read it back through the ref so updates from the async request are reactive.
  const addedCity = cities.value[5]
  selectedCity.value = addedCity
  loadWeather(addedCity).finally(() => { addedCity.loadingWeather = false })
}

loadWeather(cities.value[0]).finally(() => { loading.value = false })
Promise.all(cities.value.slice(1).map(loadWeather))
</script>

<template>
  <div class="min-h-screen p-4 sm:p-6 lg:p-8"><div class="mx-auto max-w-5xl">
    <header class="mb-5"><p class="text-sm font-medium text-slate-500">Norway Weather</p><h1 class="mt-1 text-2xl font-semibold tracking-tight text-slate-900">Cities in Norway</h1><p class="mt-1 text-sm text-slate-500">Search for another city or place to add it.</p></header>
    <div class="mb-5"><LocationSearch v-model="searchText" placeholder="Search for a city or place" @select="addCity" /></div>
    <div v-if="loading" class="rounded-2xl bg-white p-8 text-center text-sm text-slate-500 shadow-sm ring-1 ring-slate-200" role="status">Loading city weather…</div>
    <ul v-else class="grid gap-3 sm:grid-cols-2 xl:grid-cols-3"><li v-for="city in cities" :key="city.id ?? city.name"><button type="button" class="flex min-h-28 w-full items-center justify-between gap-4 rounded-2xl bg-white p-5 text-left shadow-sm ring-1 ring-slate-200 transition hover:shadow-md focus:outline-none focus:ring-2 focus:ring-sky-500" :aria-label="`Show weather details for ${city.name}`" @click="selectedCity = city"><div><h2 class="font-semibold text-slate-900">{{ city.name }}</h2><p class="mt-1 text-sm text-slate-500">View current conditions</p></div><div class="flex items-center gap-3"><span class="text-3xl" aria-hidden="true">{{ getWeatherEmoji(city.weatherSymbol) }}</span><span class="min-w-12 text-right text-xl font-semibold text-slate-800">{{ city.temperature == null ? '—' : `${Math.round(city.temperature)}°` }}</span></div></button></li></ul>
    <WeatherDetailModal :city="selectedCity" @close="selectedCity = null" />
  </div></div>
</template>
