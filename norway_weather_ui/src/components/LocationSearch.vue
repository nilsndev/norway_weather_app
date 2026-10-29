<script setup>
import { ref, watch } from 'vue'
import api from '../services/api'
import { useLocale } from '../i18n'

const props = defineProps({ modelValue: { type: String, default: '' }, placeholder: { type: String, default: 'Search a place' } })
const emit = defineEmits(['update:modelValue', 'select'])
const { t } = useLocale()
const query = ref(props.modelValue)
const results = ref([])
const loading = ref(false)
const error = ref('')
const searchError = ref('')
let timer
let requestId = 0
let lastSelectedKey = ''

watch(() => props.modelValue, (value) => { query.value = value })
watch(query, (value) => {
  emit('update:modelValue', value)
  searchError.value = ''
  clearTimeout(timer)
  requestId += 1
  loading.value = false
  results.value = []
  error.value = ''
  const term = value.trim()
  if (term.length < 2) return
  const id = requestId
  timer = setTimeout(async () => {
    loading.value = true
    try {
      const { data } = await api.get('/city/search', { params: { query: term } })
      if (id === requestId) {
        const seen = new Set()
        results.value = (Array.isArray(data) ? data : []).filter((place) => {
          if (place.countryCode !== 'no') return false
          const key = `${place.latitude},${place.longitude}`
          if (seen.has(key)) return false
          seen.add(key)
          return true
        })
      }
    } catch {
      if (id === requestId) error.value = t('searchFailed')
    } finally {
      if (id === requestId) loading.value = false
    }
  }, 300)
})

function choose(place) {
  const key = `${place.latitude},${place.longitude}`
  if (key === lastSelectedKey) return
  lastSelectedKey = key
  searchError.value = ''
  query.value = ''
  results.value = []
  emit('select', { name: place.displayName.split(',')[0], displayName: place.displayName, latitude: place.latitude, longitude: place.longitude })
}

function submitSearch() {
  if (results.value.length) choose(results.value[0])
  else if (query.value.trim().length >= 2 && !loading.value) searchError.value = t('noResults')
}
</script>

<template>
  <div class="relative">
    <form class="flex gap-2" @submit.prevent="submitSearch">
      <label class="sr-only" for="location-search">{{ t('searchPlace') }}</label>
      <input id="location-search" v-model="query" type="search" autocomplete="off" :placeholder="placeholder" class="min-w-0 flex-1 rounded-xl border border-slate-200 bg-white px-4 py-3 text-sm text-slate-800 outline-none placeholder:text-slate-400 focus:border-sky-500 focus:ring-2 focus:ring-sky-200" @keydown.esc="results = []" />
      <button type="submit" class="min-h-11 rounded-xl bg-sky-700 px-4 text-sm font-medium text-white transition hover:bg-sky-800 focus:outline-none focus:ring-2 focus:ring-sky-500 focus:ring-offset-2">{{ t('search') }}</button>
    </form>
    <p v-if="loading" class="mt-1 text-xs text-slate-500" role="status">Searching…</p>
    <p v-if="error" class="mt-1 text-xs text-rose-700" role="alert">{{ error }}</p>
    <p v-if="searchError" class="mt-1 text-xs text-rose-700" role="alert">{{ searchError }}</p>
    <ul v-if="results.length" class="absolute inset-x-0 top-full z-30 mt-1 max-h-64 overflow-y-auto rounded-xl border border-slate-200 bg-white p-1 shadow-xl" role="listbox">
      <li v-for="place in results" :key="`${place.latitude}-${place.longitude}`">
        <button type="button" class="w-full rounded-lg px-3 py-2 text-left text-sm text-slate-700 hover:bg-sky-50 focus:bg-sky-50 focus:outline-none" @click="choose(place)">{{ place.displayName }}</button>
      </li>
    </ul>
  </div>
</template>
