<script setup>
import { ref } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const auth = useAuthStore()
const menuOpen = ref(false)

async function logout() {
  auth.logout()
  menuOpen.value = false
  await router.push('/login')
}
</script>

<template>
  <div>
    <button
      type="button"
      class="fixed left-4 top-4 z-50 inline-flex min-h-11 min-w-11 items-center justify-center rounded-xl border border-slate-200 bg-white text-slate-700 shadow-sm transition hover:bg-slate-50 focus:outline-none focus:ring-2 focus:ring-sky-500 sm:hidden"
      :aria-expanded="menuOpen"
      aria-controls="app-sidebar"
      @click="menuOpen = !menuOpen"
    >
      <span class="sr-only">{{ menuOpen ? 'Close menu' : 'Open menu' }}</span>
      <svg class="h-5 w-5" viewBox="0 0 24 24" fill="none" aria-hidden="true">
        <path v-if="!menuOpen" d="M4 7h16M4 12h16M4 17h16" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
        <path v-else d="m6 6 12 12M18 6 6 18" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
      </svg>
    </button>

    <button
      v-if="menuOpen"
      type="button"
      class="fixed inset-0 z-40 bg-slate-950/30 sm:hidden"
      aria-label="Close navigation menu"
      @click="menuOpen = false"
    />

    <aside
      id="app-sidebar"
      class="fixed inset-y-0 left-0 z-40 flex w-64 flex-col border-r border-slate-200 bg-white px-4 py-6 shadow-xl transition-transform duration-200 motion-reduce:transition-none sm:translate-x-0 sm:shadow-none"
      :class="menuOpen ? 'translate-x-0' : '-translate-x-full'"
      aria-label="Main navigation"
    >
      <RouterLink to="/" class="mb-9 flex items-center gap-3 px-2" @click="menuOpen = false">
        <span>
          <span class="block text-base font-semibold tracking-tight text-slate-900">Norway Weather</span>
          <span class="block text-xs text-slate-500">Forecast and conditions</span>
        </span>
      </RouterLink>

      <nav class="flex-1" aria-label="Main">
        <p class="mb-3 px-3 text-xs font-semibold uppercase tracking-wider text-slate-400">Explore</p>
        <ul class="space-y-1.5">
          <li>
            <RouterLink to="/" class="group flex min-h-11 items-center gap-3 rounded-xl px-3 text-sm font-medium text-slate-600 transition duration-150 hover:scale-[1.01] hover:bg-slate-50 hover:text-slate-900 motion-reduce:transform-none" exact-active-class="!bg-sky-50 !text-sky-800" @click="menuOpen = false">
              <svg class="h-5 w-5 text-slate-400 transition group-hover:text-sky-700" viewBox="0 0 24 24" fill="none" aria-hidden="true"><path d="m3 10 9-7 9 7v10a1 1 0 0 1-1 1h-6v-7h-4v7H4a1 1 0 0 1-1-1V10Z" stroke="currentColor" stroke-width="1.8" stroke-linejoin="round" /></svg>
              Home
            </RouterLink>
          </li>
          <li>
            <RouterLink to="/cities" class="group flex min-h-11 items-center gap-3 rounded-xl px-3 text-sm font-medium text-slate-600 transition duration-150 hover:scale-[1.01] hover:bg-slate-50 hover:text-slate-900 motion-reduce:transform-none" active-class="!bg-sky-50 !text-sky-800" @click="menuOpen = false">
              <svg class="h-5 w-5 text-slate-400 transition group-hover:text-sky-700" viewBox="0 0 24 24" fill="none" aria-hidden="true"><path d="M4 20V8l6-4v16m0-12 6-3v15m0-10 4-2v12H4" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" /><path d="M7 9h.01M7 13h.01M13 9h.01M13 13h.01M13 17h.01" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" /></svg>
              Cities
            </RouterLink>
          </li>
          <li>
            <RouterLink to="/map" class="group flex min-h-11 items-center gap-3 rounded-xl px-3 text-sm font-medium text-slate-600 transition duration-150 hover:scale-[1.01] hover:bg-slate-50 hover:text-slate-900 motion-reduce:transform-none" active-class="!bg-sky-50 !text-sky-800" @click="menuOpen = false">
              <svg class="h-5 w-5 text-slate-400 transition group-hover:text-sky-700" viewBox="0 0 24 24" fill="none" aria-hidden="true"><path d="m3 6 6-3 6 3 6-3v15l-6 3-6-3-6 3V6Z" stroke="currentColor" stroke-width="1.8" stroke-linejoin="round" /><path d="M9 3v15m6-12v15" stroke="currentColor" stroke-width="1.8" /></svg>
              Map
            </RouterLink>
          </li>
        </ul>
      </nav>

      <div class="border-t border-slate-100 pt-4">
        <button type="button" class="flex min-h-11 w-full items-center gap-3 rounded-xl px-3 text-sm font-medium text-slate-500 transition hover:bg-rose-50 hover:text-rose-700 focus:outline-none focus:ring-2 focus:ring-rose-300" @click="logout">
          <svg class="h-5 w-5" viewBox="0 0 24 24" fill="none" aria-hidden="true"><path d="M10 17l5-5-5-5m5 5H3m9-9h6a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2h-6" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" /></svg>
          Log out
        </button>
      </div>
    </aside>
  </div>
</template>
