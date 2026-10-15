<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const auth = useAuthStore()
const router = useRouter()
const email = ref('')
const password = ref('')
const errorMessage = ref('')

async function submit() {
  errorMessage.value = ''
  try {
    await auth.login(email.value, password.value)
    await router.push('/')
  } catch (error) {
    errorMessage.value = error.response?.status === 401
      ? 'The email or password is incorrect.'
      : 'Unable to sign in. Please try again.'
  }
}
</script>

<template>
  <main class="flex min-h-screen items-center justify-center bg-slate-100 px-4 py-12">
    <section class="w-full max-w-md rounded-2xl bg-white p-8 shadow-lg">
      <h1 class="text-2xl font-bold tracking-tight text-slate-900">Sign in</h1>
      <p class="mt-2 text-sm text-slate-600">Sign in to explore the weather map.</p>
      <form class="mt-8 space-y-5" @submit.prevent="submit">
        <label class="block text-sm font-medium text-slate-700">
          Email
          <input v-model="email" required type="email" autocomplete="email" class="mt-2 w-full rounded-lg border border-slate-300 px-3 py-2.5 text-slate-900 outline-none focus:border-blue-600 focus:ring-2 focus:ring-blue-100" />
        </label>
        <label class="block text-sm font-medium text-slate-700">
          Password
          <input v-model="password" required type="password" autocomplete="current-password" class="mt-2 w-full rounded-lg border border-slate-300 px-3 py-2.5 text-slate-900 outline-none focus:border-blue-600 focus:ring-2 focus:ring-blue-100" />
        </label>
        <p v-if="errorMessage" role="alert" class="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700">{{ errorMessage }}</p>
        <button :disabled="auth.loading" class="w-full rounded-lg bg-blue-600 px-4 py-2.5 font-semibold text-white hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-60">
          {{ auth.loading ? 'Signing in…' : 'Sign in' }}
        </button>
      </form>
      <p class="mt-6 text-center text-sm text-slate-600">New here? <RouterLink to="/register" class="font-semibold text-blue-700 hover:underline">Create an account</RouterLink></p>
    </section>
  </main>
</template>
