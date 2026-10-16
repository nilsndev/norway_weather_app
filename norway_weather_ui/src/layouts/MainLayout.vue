<script setup>
import Sidebar from '../components/Sidebar.vue'

</script>
<template>
    <div>
        <Sidebar />
        <main>
            <div class="p-4 sm:ml-64">
                <RouterView />
            </div>
        </main>
      </div>
</template>