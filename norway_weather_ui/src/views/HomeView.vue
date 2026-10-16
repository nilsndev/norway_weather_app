<script setup>
</script>

<template>
  <div class="p-8">

    <section class="flex items-center justify-between">
      <div>
        <h1 class="text-4xl font-bold">
          Oslo
        </h1>

        <p class="mt-2 text-sm text-slate-400">
          Chance of rain: 0%
        </p>

        <p class="mt-12 text-6xl font-bold">
          21°
        </p>
      </div>

      <div class="flex items-center justify-center">
        <img
          src="https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTFiZPhVCR7pSlGp19zWCvgiio90VNneLXFNkeaKaAyMw&s=10"
          alt="Sunny"
          class="h-40 w-40"
        />
      </div>
    </section>

    <section class="mt-8 rounded-2xl bg-slate-200 p-6">

        <h2 class="mb-6 text-sm font-semibold uppercase text-slate-400">
          Today's forecast
        </h2>
  
        <div class="flex justify-between">
  
          <div class="flex flex-1 flex-col items-center gap-3 border-r border-slate-700">
            <p class="text-sm text-slate-400">06:00</p>
            <span class="text-4xl">☁️</span>
            <p class="text-xl font-bold">15°</p>
          </div>
  
          <div class="flex flex-1 flex-col items-center gap-3 border-r border-slate-700">
            <p class="text-sm text-slate-400">09:00</p>
            <span class="text-4xl">🌤️</span>
            <p class="text-xl font-bold">17°</p>
          </div>
  
          <div class="flex flex-1 flex-col items-center gap-3 border-r border-slate-700">
            <p class="text-sm text-slate-400">12:00</p>
            <span class="text-4xl">☀️</span>
            <p class="text-xl font-bold">21°</p>
          </div>
  
          <div class="flex flex-1 flex-col items-center gap-3 border-r border-slate-700">
            <p class="text-sm text-slate-400">15:00</p>
            <span class="text-4xl">☀️</span>
            <p class="text-xl font-bold">22°</p>
          </div>
  
          <div class="flex flex-1 flex-col items-center gap-3">
            <p class="text-sm text-slate-400">18:00</p>
            <span class="text-4xl">🌤️</span>
            <p class="text-xl font-bold">19°</p>
          </div>
  
        </div>
      </section>
  
  
      <section class="mt-4 rounded-2xl bg-slate-200 p-6">
  
        <h2 class="mb-6 text-sm font-semibold uppercase text-slate-400">
          Air conditions
        </h2>
  
        <div class="grid grid-cols-2 gap-8">
  
          <div>
            <p class="text-slate-400">
              🌡️ Temperature
            </p>
  
            <p class="mt-1 text-3xl font-bold">
              21°
            </p>
          </div>
  
          <div>
            <p class="text-slate-400">
              💨 Wind
            </p>
  
            <p class="mt-1 text-3xl font-bold">
              4.2 km/h
            </p>
          </div>
  
          <div>
            <p class="text-slate-400">
              💧 Precipitation
            </p>
  
            <p class="mt-1 text-3xl font-bold">
              0 mm
            </p>
          </div>
  
          <div>
            <p class="text-slate-400">
              💦 Humidity
            </p>
  
            <p class="mt-1 text-3xl font-bold">
              67%
            </p>
          </div>
  
        </div>
      </section>

  </div>
</template>