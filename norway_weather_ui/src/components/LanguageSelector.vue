<script setup>
import { useLocale } from '../i18n'
const { locale, t, setLocale } = useLocale()
</script>

<template>
  <label class="flex items-center gap-2 text-sm text-slate-600">
    <span>{{ t('language') }}</span>
    <select :value="locale" class="rounded-lg border border-slate-200 bg-white px-2 py-1.5" @change="setLocale($event.target.value)">
      <option value="en">English</option><option value="no">Norsk</option><option value="de">Deutsch</option>
    </select>
  </label>
</template>
