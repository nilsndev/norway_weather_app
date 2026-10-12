
<script setup>
import { onMounted, onBeforeUnmount, ref } from 'vue'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'

const mapContainer = ref(null)
const selectedPosition = ref(null)
const loading = ref(false)
const errorMessage = ref('')

const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL || 'https://localhost:7039/api/weather'

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

    const [currentResponse, forecastResponse] = await Promise.all([
      fetch(`${API_BASE_URL}/current?${query}`),
      fetch(`${API_BASE_URL}/forecast?${query}`),
    ])

    if (!currentResponse.ok) {
      throw new Error(`Current weather request failed: ${currentResponse.status}`)
    }

    if (!forecastResponse.ok) {
      throw new Error(`Forecast request failed: ${forecastResponse.status}`)
    }

    const currentWeather = await currentResponse.json()
    const forecast = await forecastResponse.json()

    console.log('Current weather:', currentWeather)
    console.log('24-hour forecast:', forecast)
  } catch (error) {
    console.error('Failed to fetch weather:', error)
    errorMessage.value =
      'Could not fetch weather. Check the API URL and backend connection.'
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  map = L.map(mapContainer.value).setView([64.5, 11], 4)

  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution:
      '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
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
  <main class="weather-app">
    <header class="header">
      <div>
        <h1>Weather Explorer</h1>
        <p>Click anywhere on the map to get weather information.</p>
      </div>
      <span class="status" :class="{ active: selectedPosition }">
        {{ selectedPosition ? 'Location selected' : 'Select a location' }}
      </span>
    </header>

    <section class="map-container">
      <div ref="mapContainer" class="map"></div>
    </section>

    <section class="details">
      <h2>Selected location</h2>

      <p v-if="!selectedPosition">
        Click on the map to select a location.
      </p>

      <div v-else class="coordinates">
        <div>
          <span>Latitude</span>
          <strong>{{ selectedPosition.latitude.toFixed(5) }}</strong>
        </div>
        <div>
          <span>Longitude</span>
          <strong>{{ selectedPosition.longitude.toFixed(5) }}</strong>
        </div>
      </div>

      <p v-if="loading" class="message">
        Fetching weather data...
      </p>

      <p v-if="errorMessage" class="error">
        {{ errorMessage }}
      </p>

      <p class="hint">
        Weather responses are currently logged in the browser console.
        Open Developer Tools → Console to inspect them.
      </p>
    </section>
  </main>
</template>

<style scoped>
.weather-app {
  max-width: 1100px;
  margin: 0 auto;
  padding: 32px 20px;
  color: #172033;
  font-family: Arial, sans-serif;
}

.header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  margin-bottom: 24px;
}

h1 {
  margin: 0 0 8px;
  font-size: 30px;
}

.header p,
.details p {
  color: #64748b;
}

.status {
  padding: 8px 12px;
  border-radius: 20px;
  background: #e2e8f0;
  font-size: 13px;
  white-space: nowrap;
}

.status.active {
  background: #dcfce7;
  color: #166534;
}

.map-container {
  overflow: hidden;
  border: 1px solid #e2e8f0;
  border-radius: 16px;
}

.map {
  height: 520px;
  width: 100%;
}

.details {
  margin-top: 24px;
  padding: 22px;
  border: 1px solid #e2e8f0;
  border-radius: 16px;
}

.details h2 {
  margin-top: 0;
}

.coordinates {
  display: flex;
  flex-wrap: wrap;
  gap: 40px;
}

.coordinates div {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.coordinates span {
  color: #64748b;
  font-size: 13px;
}

.message {
  margin-top: 20px;
}

.error {
  color: #b91c1c !important;
}

.hint {
  margin-bottom: 0;
  font-size: 13px;
}

@media (max-width: 600px) {
  .header {
    align-items: flex-start;
    flex-direction: column;
  }

  .map {
    height: 400px;
  }
}
</style>
