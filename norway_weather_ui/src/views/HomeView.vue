
<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'
import { getWeatherEmoji } from '../helper/weather'
import { formatWeatherTime } from '../helper/date_time'

const city = ref({
  name: 'Oslo',
  latitude: 59.9139,
  longitude: 10.7522
})

const currentWeather = ref(null)
const forecast = ref([])
const loading = ref(true)
const errorMessage = ref('')

async function getCurrentWeather(latitude, longitude) {
  const query = new URLSearchParams({
    latitude: String(latitude),
    longitude: String(longitude)
  })

  const response = await api.get(`/weather/current?${query}`)
  return response.data
}

async function getForecastWeather(latitude, longitude) {
  const query = new URLSearchParams({
    latitude: String(latitude),
    longitude: String(longitude)
  })

  const response = await api.get(`/weather/forecast?${query}`)
  return response.data
}

function formatHour(time) {
  return formatWeatherTime(time)
}

function formatTemperature(temperature) {
  if (temperature == null) return '--'
  return `${Math.round(temperature)}°`
}

function formatWind(speed) {
  if (speed == null) return '--'
  return `${speed} m/s`
}

onMounted(async () => {
  loading.value = true
  errorMessage.value = ''

  try {
    const [current, forecastResponse] = await Promise.all([
      getCurrentWeather(
        city.value.latitude,
        city.value.longitude
      ),
      getForecastWeather(
        city.value.latitude,
        city.value.longitude
      )
    ])

    currentWeather.value = current

    forecast.value = Array.isArray(forecastResponse)
      ? forecastResponse
      : forecastResponse.data ?? []
  } catch (error) {
    console.error('Error hvile getting weather data', error)
    errorMessage.value = 'Couldnt get weather data. Please try again'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <main class="min-h-screen bg-blue-50 p-6 text-slate-800 md:p-8">

    <div class="mx-auto max-w-5xl">

      <header class="mb-8">
        <p class="text-sm font-medium text-slate-500">
          Weather
        </p>

        <h1 class="mt-1 text-3xl font-bold">
          {{ city.name }}
        </h1>
      </header>

      <div
        v-if="loading"
        class="rounded-2xl bg-white p-8 text-center shadow-sm"
      >
        <p class="animate-pulse text-slate-500">
          Get weather data...
        </p>
      </div>

      <div
        v-else-if="errorMessage"
        class="rounded-2xl bg-white p-6 text-red-600 shadow-sm"
      >
        {{ errorMessage }}
      </div>

      <template v-else-if="currentWeather">

        <section
          class="rounded-3xl bg-gradient-to-br from-sky-500 to-blue-700 p-8 text-white shadow-md"
        >
          <div class="flex items-center justify-between gap-4">

            <div>
              <p class="text-lg text-blue-100">
                Current Weather
              </p>

              <p class="mt-3 text-6xl font-bold">
                {{ formatTemperature(currentWeather.temperature) }}
              </p>

              <p class="mt-3 text-blue-100">
                {{ currentWeather.weatherSymbol }}
              </p>

              <p class="mt-2 text-sm text-blue-100">
                Vind:
                {{ formatWind(currentWeather.windSpeed) }}
              </p>
            </div>

            <div class="text-7xl md:text-8xl">
              {{ getWeatherEmoji(currentWeather.weatherSymbol) }}
            </div>

          </div>
        </section>

        <section class="mt-6 rounded-2xl bg-white p-6 shadow-sm">

          <h2 class="mb-5 text-lg font-bold">
            Forecast
          </h2>

          <div
            v-if="forecast.length"
            class="flex gap-5 overflow-x-auto pb-3"
          >
            <article
              v-for="(item, index) in forecast"
              :key="item.time ?? index"
              class="flex min-w-24 flex-col items-center gap-3 rounded-xl bg-blue-50 px-4 py-5"
            >
              <p class="text-sm text-slate-500">
                {{ formatHour(item.time) }}
              </p>

              <span class="text-3xl">
                {{ getWeatherEmoji(item.weatherSymbol) }}
              </span>

              <p class="text-lg font-bold">
                {{ formatTemperature(item.temperature) }}
              </p>

              <p
                v-if="item.windSpeed != null"
                class="text-xs text-slate-500"
              >
                {{ formatWind(item.windSpeed) }}
              </p>
            </article>
          </div>

          <p v-else class="text-sm text-slate-500">
            No forecasts available
          </p>

        </section>

        <section class="mt-6 rounded-2xl bg-white p-6 shadow-sm">

          <h2 class="mb-5 text-lg font-bold">
            Condition
          </h2>

          <div class="grid grid-cols-2 gap-6 md:grid-cols-3">

            <div class="rounded-xl bg-slate-50 p-4">
              <p class="text-sm text-slate-500">
                🌡️ Temperatur
              </p>
              <p class="mt-2 text-2xl font-bold">
                {{ formatTemperature(currentWeather.temperature) }}
              </p>
            </div>

            <div class="rounded-xl bg-slate-50 p-4">
              <p class="text-sm text-slate-500">
                💨 Wind
              </p>
              <p class="mt-2 text-2xl font-bold">
                {{ formatWind(currentWeather.windSpeed) }}
              </p>
            </div>

            <div class="rounded-xl bg-slate-50 p-4">
              <p class="text-sm text-slate-500">
                ☁️ Symbol
              </p>
              <p class="mt-2 text-lg font-bold">
                {{ currentWeather.weatherSymbol || 'Ukjent' }}
              </p>
            </div>

          </div>

        </section>

      </template>

    </div>
  </main>
</template>
