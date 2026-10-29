<script setup>
import { computed } from 'vue'
import { getWeatherEmoji } from '../helper/weather'
import { useLocale } from '../i18n'

const props = defineProps({ city: { type: Object, default: null } })
const emit = defineEmits(['close'])
const { t } = useLocale()
const temperature = computed(() => props.city?.temperature == null ? '—' : `${Math.round(props.city.temperature)}°`)
</script>

<template>
  <div v-if="city" class="fixed inset-0 z-[1000] flex items-end justify-center bg-slate-950/40 p-3 sm:items-center sm:p-6" @click.self="emit('close')" @keydown.esc="emit('close')">
    <section class="w-full max-w-md rounded-2xl bg-white p-5 shadow-2xl sm:p-6" role="dialog" aria-modal="true" :aria-label="t('weatherIn', { city: city.name })">
      <div class="flex items-start justify-between gap-4"><div><p class="text-xs font-medium uppercase tracking-wide text-sky-700">{{ t('currentWeather') }}</p><h2 class="mt-1 text-xl font-semibold text-slate-900">{{ city.name }}</h2><p v-if="city.displayName" class="mt-1 text-xs text-slate-500">{{ city.displayName }}</p></div><button type="button" class="rounded-lg p-2 text-xl leading-none text-slate-500 hover:bg-slate-100" :aria-label="t('closeDetails')" @click="emit('close')">×</button></div>
      <p v-if="city.loadingWeather" class="mt-5 text-center text-sm text-slate-500" role="status">{{ t('loadingWeather') }}</p>
      <p class="mt-5 text-center text-5xl" aria-hidden="true">{{ getWeatherEmoji(city.weatherSymbol) }}</p>
      <p class="mt-2 text-center text-4xl font-semibold text-slate-900">{{ temperature }}</p>
      <p class="mt-1 text-center text-sm text-slate-600">{{ city.weatherSymbol || t('weatherConditions') }}</p>
      <dl class="mt-5 grid grid-cols-2 gap-3"><div class="rounded-xl bg-slate-50 p-3"><dt class="text-xs text-slate-500">{{ t('wind') }}</dt><dd class="mt-1 font-semibold text-slate-800">{{ city.windSpeed == null ? '—' : `${city.windSpeed} m/s` }}</dd></div><div class="rounded-xl bg-slate-50 p-3"><dt class="text-xs text-slate-500">{{ t('humidity') }}</dt><dd class="mt-1 font-semibold text-slate-800">{{ city.humidity == null ? '—' : `${city.humidity}%` }}</dd></div></dl>
      <p v-if="city.latitude != null" class="mt-4 text-center text-xs text-slate-500">{{ Number(city.latitude).toFixed(4) }}°, {{ Number(city.longitude).toFixed(4) }}°</p>
    </section>
  </div>
</template>
