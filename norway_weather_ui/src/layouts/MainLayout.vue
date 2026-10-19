<script setup>
import Sidebar from '../components/Sidebar.vue'
</script>
<template>
  <div class="min-h-screen bg-slate-50 text-slate-800">
    <Sidebar />
    <main class="min-h-screen pt-16 sm:ml-64 sm:pt-0">
      <RouterView v-slot="{ Component, route }">
        <Transition mode="out-in" enter-active-class="transition duration-200 ease-out motion-reduce:transition-none" enter-from-class="translate-y-1 opacity-0" enter-to-class="translate-y-0 opacity-100" leave-active-class="transition duration-100 ease-in motion-reduce:transition-none" leave-from-class="opacity-100" leave-to-class="opacity-0">
          <component :is="Component" :key="route.path" />
        </Transition>
      </RouterView>
    </main>
  </div>
</template>
