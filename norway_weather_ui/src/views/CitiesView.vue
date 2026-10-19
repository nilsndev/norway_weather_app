<script setup>
import api from '../services/api'
import { ref, onMounted } from 'vue'
import { getWeatherEmoji } from '../helper/weather'

const loading = ref(true)
const cities = ref([
  { id: 1, name: 'Oslo', latitude: 59.9139, longitude: 10.7522, temperature: null, weatherSymbol: '' },
  { id: 2, name: 'Bergen', latitude: 60.3913, longitude: 5.3221, temperature: null, weatherSymbol: '' },
  { id: 3, name: 'Stavanger', latitude: 58.97, longitude: 5.7331, temperature: null, weatherSymbol: '' },
  { id: 4, name: 'Trondheim', latitude: 63.4305, longitude: 10.3951, temperature: null, weatherSymbol: '' },
  { id: 5, name: 'Drammen', latitude: 59.7439, longitude: 10.2045, temperature: null, weatherSymbol: '' },
])

async function getWeather(city) {
  const query = new URLSearchParams({
    latitude: String(city.latitude),
    longitude: String(city.longitude),
  })

  try {
    const { data } = await api.get(`/weather/current?${query}`)
    city.temperature = data.temperature
    city.weatherSymbol = data.weatherSymbol
  } catch (error) {
    console.error(`Failed to get weather data for ${city.name}`, error)
  }
}

onMounted(async () => {
  await Promise.all(cities.value.map(getWeather))
  loading.value = false
})
</script>

<template>
  <div class="min-h-screen p-4 sm:p-6 lg:p-8">
    <div class="mx-auto max-w-5xl">
      <header class="mb-6">
        <p class="text-sm font-medium text-slate-500">Norway Weather</p>
        <h1 class="mt-1 text-2xl font-semibold tracking-tight text-slate-900">Cities in Norway</h1>
      </header>

      <div v-if="loading" class="rounded-2xl bg-white p-8 text-center text-sm text-slate-500 shadow-sm ring-1 ring-slate-200" role="status">
        <span class="inline-flex items-center gap-3">
          <span class="h-4 w-4 animate-spin rounded-full border-2 border-sky-200 border-t-sky-600 motion-reduce:animate-none" aria-hidden="true"></span>
          Loading city weather…
        </span>
      </div>

      <ul v-else class="grid gap-3 sm:grid-cols-2 xl:grid-cols-3">
        <li v-for="city in cities" :key="city.id" class="flex items-center justify-between gap-4 rounded-2xl bg-white p-5 shadow-sm ring-1 ring-slate-200 transition hover:shadow-md">
          <div>
            <h2 class="font-semibold text-slate-900">{{ city.name }}</h2>
            <p class="mt-1 text-sm text-slate-500">Current conditions</p>
          </div>
          <div class="flex items-center gap-3">
            <span class="text-3xl" aria-hidden="true">{{ getWeatherEmoji(city.weatherSymbol) }}</span>
            <span class="min-w-12 text-right text-xl font-semibold text-slate-800">
              {{ city.temperature == null ? '—' : `${Math.round(city.temperature)}°` }}
            </span>
          </div>
        </li>
      </ul>
    </div>
  </div>
</template>
