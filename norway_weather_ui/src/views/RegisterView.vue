<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const auth = useAuthStore()
const router = useRouter()
const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const errorMessage = ref('')

async function submit() {
  errorMessage.value = ''
  if (password.value !== confirmPassword.value) {
    errorMessage.value = 'Passwords do not match.'
    return
  }
  try {
    await auth.register(email.value, password.value)
    await router.push('/login')
  } catch (error) {
    errorMessage.value = error.response?.data?.message || 'Unable to create your account. Please check your details and try again.'
  }
}
</script>

<template>
  <main class="flex min-h-screen items-center justify-center bg-slate-100 px-4 py-12">
    <section class="w-full max-w-md rounded-2xl bg-white p-8 shadow-lg">
      <h1 class="text-2xl font-bold tracking-tight text-slate-900">Create your account</h1>
      <p class="mt-2 text-sm text-slate-600">Register to access the weather map.</p>
      <form class="mt-8 space-y-5" @submit.prevent="submit">
        <label class="block text-sm font-medium text-slate-700">
          Email
          <input v-model="email" required type="email" autocomplete="email" class="mt-2 w-full rounded-lg border border-slate-300 px-3 py-2.5 text-slate-900 outline-none focus:border-blue-600 focus:ring-2 focus:ring-blue-100" />
        </label>
        <label class="block text-sm font-medium text-slate-700">
          Password
          <input v-model="password" required type="password" autocomplete="new-password" class="mt-2 w-full rounded-lg border border-slate-300 px-3 py-2.5 text-slate-900 outline-none focus:border-blue-600 focus:ring-2 focus:ring-blue-100" />
        </label>
        <label class="block text-sm font-medium text-slate-700">
          Confirm password
          <input v-model="confirmPassword" required type="password" autocomplete="new-password" class="mt-2 w-full rounded-lg border border-slate-300 px-3 py-2.5 text-slate-900 outline-none focus:border-blue-600 focus:ring-2 focus:ring-blue-100" />
        </label>
        <p v-if="errorMessage" role="alert" class="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700">{{ errorMessage }}</p>
        <button :disabled="auth.loading" class="w-full rounded-lg bg-blue-600 px-4 py-2.5 font-semibold text-white hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-60">
          {{ auth.loading ? 'Creating account…' : 'Register' }}
        </button>
      </form>
      <p class="mt-6 text-center text-sm text-slate-600">Already registered? <RouterLink to="/login" class="font-semibold text-blue-700 hover:underline">Sign in</RouterLink></p>
    </section>
  </main>
</template>
