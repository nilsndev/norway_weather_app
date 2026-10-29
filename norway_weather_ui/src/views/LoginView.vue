<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useLocale } from '../i18n'
import LanguageSelector from '../components/LanguageSelector.vue'

const auth = useAuthStore()
const router = useRouter()
const email = ref('')
const password = ref('')
const errorMessage = ref('')
const { t } = useLocale()

async function submit() {
  errorMessage.value = ''
  try {
    await auth.login(email.value, password.value)
    await router.push('/')
  } catch (error) {
    errorMessage.value = error.response?.status === 401 ? t('incorrectCredentials') : t('signInFailed')
  }
}
</script>

<template>
  <main class="flex min-h-screen items-center justify-center bg-slate-50 px-4 py-12">
    <section class="w-full max-w-md rounded-2xl bg-white p-6 shadow-sm ring-1 ring-slate-200 sm:p-8">
      <div class="mb-5 flex justify-end"><LanguageSelector /></div>
      <h1 class="text-2xl font-bold tracking-tight text-slate-900">{{ t('signInTitle') }}</h1>
      <p class="mt-2 text-sm text-slate-600">{{ t('signInDescription') }}</p>
      <form class="mt-8 space-y-5" @submit.prevent="submit">
        <label class="block text-sm font-medium text-slate-700">{{ t('email') }}<input v-model="email" required type="email" autocomplete="email" class="mt-2 w-full rounded-xl border border-slate-300 px-3 py-2.5 text-slate-900 outline-none transition focus:border-sky-600 focus:ring-2 focus:ring-sky-100" /></label>
        <label class="block text-sm font-medium text-slate-700">{{ t('password') }}<input v-model="password" required type="password" autocomplete="current-password" class="mt-2 w-full rounded-xl border border-slate-300 px-3 py-2.5 text-slate-900 outline-none transition focus:border-sky-600 focus:ring-2 focus:ring-sky-100" /></label>
        <p v-if="errorMessage" role="alert" class="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700">{{ errorMessage }}</p>
        <button :disabled="auth.loading" class="min-h-11 w-full rounded-xl bg-sky-700 px-4 py-2.5 font-semibold text-white transition hover:bg-sky-800 focus:outline-none focus:ring-2 focus:ring-sky-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-60">{{ auth.loading ? t('signingIn') : t('signIn') }}</button>
      </form>
      <p class="mt-6 text-center text-sm text-slate-600">{{ t('newHere') }} <RouterLink to="/register" class="font-semibold text-sky-800 hover:underline">{{ t('createAccountLink') }}</RouterLink></p>
    </section>
  </main>
</template>
