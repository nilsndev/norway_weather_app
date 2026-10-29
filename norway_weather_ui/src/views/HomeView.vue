<script setup>
import { onMounted, ref } from 'vue'
import api from '../services/api'
import LocationSearch from '../components/LocationSearch.vue'
import { getWeatherEmoji } from '../helper/weather'
import { formatWeatherTime } from '../helper/date_time'
import { useLocale, isNorwayLocation } from '../i18n'

const { t } = useLocale()
const city = ref({ name: 'Oslo', latitude: 59.9139, longitude: 10.7522 })
const searchText = ref('')
const currentWeather = ref(null)
const forecast = ref([])
const loading = ref(false)
const errorMessage = ref('')
let requestId = 0

async function loadCity(place) {
  if (!isNorwayLocation(place.latitude, place.longitude)) { errorMessage.value = t('outsideArea'); return }
  city.value = place
  searchText.value = place.displayName || place.name
  const id = ++requestId
  loading.value = true
  errorMessage.value = ''
  currentWeather.value = null
  try {
    const params = { latitude: String(place.latitude), longitude: String(place.longitude) }
    const [current, forecastResponse] = await Promise.all([api.get('/weather/current', { params }), api.get('/weather/forecast', { params })])
    if (id !== requestId) return
    currentWeather.value = current.data
    forecast.value = Array.isArray(forecastResponse.data) ? forecastResponse.data : forecastResponse.data.data ?? []
  } catch (error) {
    console.error('Failed to load weather', error)
    if (id === requestId) errorMessage.value = t('weatherFailed')
  } finally {
    if (id === requestId) loading.value = false
  }
}

onMounted(() => loadCity(city.value))
function formatTemperature(value) { return value == null ? '—' : `${Math.round(value)}°` }
</script>

<template>
  <main class="min-h-screen p-4 text-slate-800 sm:p-6 lg:p-8"><div class="mx-auto max-w-5xl">
    <header class="mb-6"><p class="text-sm font-medium text-slate-500">Norway Weather</p><h1 class="mt-1 text-2xl font-semibold tracking-tight text-slate-900">{{ t('homeTitle', { city: city.name }) }}</h1><p class="mt-1 text-sm text-slate-500">{{ t('homeDescription') }}</p></header>
    <div class="mb-5"><LocationSearch v-model="searchText" :placeholder="t('searchPlace')" @select="loadCity" /></div>
    <div v-if="loading" class="rounded-2xl bg-white p-8 text-center text-slate-500 shadow-sm ring-1 ring-slate-200" role="status">{{ t('loadingWeather') }}</div>
    <div v-else-if="errorMessage" class="rounded-2xl bg-white p-6 text-rose-700 shadow-sm ring-1 ring-slate-200" role="alert">{{ errorMessage }}</div>
    <template v-else-if="currentWeather">
      <section class="rounded-2xl bg-gradient-to-br from-sky-600 to-blue-700 p-6 text-white shadow-sm sm:p-8"><div class="flex items-center justify-between gap-4"><div><p class="text-lg text-blue-100">{{ t('currentWeather') }}</p><p class="mt-3 text-6xl font-bold">{{ formatTemperature(currentWeather.temperature) }}</p><p class="mt-2 text-blue-100">{{ currentWeather.weatherSymbol }}</p></div><div class="text-7xl" aria-hidden="true">{{ getWeatherEmoji(currentWeather.weatherSymbol) }}</div></div></section>
      <section class="mt-5 rounded-2xl bg-white p-5 shadow-sm ring-1 ring-slate-200 sm:p-6"><h2 class="mb-5 text-lg font-bold">{{ t('forecast') }}</h2><div v-if="forecast.length" class="flex gap-4 overflow-x-auto pb-3"><article v-for="(item, index) in forecast" :key="item.time ?? index" class="flex min-w-24 flex-col items-center gap-3 rounded-xl bg-sky-50 px-4 py-5"><p class="text-sm text-slate-500">{{ formatWeatherTime(item.time) }}</p><span class="text-3xl">{{ getWeatherEmoji(item.weatherSymbol) }}</span><p class="text-lg font-bold">{{ formatTemperature(item.temperature) }}</p><p v-if="item.windSpeed != null" class="text-xs text-slate-500">{{ item.windSpeed }} m/s</p></article></div><p v-else class="text-sm text-slate-500">{{ t('noForecast') }}</p></section>
      <section class="mt-5 rounded-2xl bg-white p-5 shadow-sm ring-1 ring-slate-200 sm:p-6"><h2 class="mb-5 text-lg font-bold">{{ t('conditions') }}</h2><dl class="grid grid-cols-2 gap-4 sm:grid-cols-3"><div class="rounded-xl bg-slate-50 p-4"><dt class="text-sm text-slate-500">{{ t('temperature') }}</dt><dd class="mt-2 text-2xl font-bold">{{ formatTemperature(currentWeather.temperature) }}</dd></div><div class="rounded-xl bg-slate-50 p-4"><dt class="text-sm text-slate-500">{{ t('wind') }}</dt><dd class="mt-2 text-2xl font-bold">{{ currentWeather.windSpeed == null ? '—' : `${currentWeather.windSpeed} m/s` }}</dd></div><div class="rounded-xl bg-slate-50 p-4"><dt class="text-sm text-slate-500">{{ t('humidity') }}</dt><dd class="mt-2 text-2xl font-bold">{{ currentWeather.humidity == null ? '—' : `${currentWeather.humidity}%` }}</dd></div></dl></section>
    </template>
  </div></main>
</template>
