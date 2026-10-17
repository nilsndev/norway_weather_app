<script setup>
    import api from '../services/api'
    import {ref, onMounted} from 'vue'
    import { getWeatherEmoji } from '../helper/weather'

    const showList = ref(false);

   

    const cities = ref([
        {
            id: 1, 
            name: 'Oslo',
            latitude: 59.9139,
            longitude: 10.7522,
            temperature: 0,
            weatherSymbol: ''
        },
        {
            id: 2,
            name: 'Bergen',
            latitude: 60.3913,
            longitude: 5.3221,
            temperature: 0,
            weatherSymbol: ''
        },
        {
            id: 3,
            name: 'Stavanger',
            latitude: 58.9700,
            longitude: 5.7331,
            temperature: 0,
            weatherSymbol: ''
        },
        {
            id: 4,
            name: 'Trondheim',
            latitude: 63.4305,
            longitude: 10.3951,
            temperature: 0,
            weatherSymbol: ''
        },
        {
            id: 5,
            name: 'Drammen',
            latitude: 59.7439,
            longitude: 10.2045,
            temperature: 0,
            weatherSymbol: ''
        }
    ])

    async function getWeather(latitude, longitude) {
        const query = new URLSearchParams({
            latitude: String(latitude),
            longitude: String(longitude)
        })

        const currentResponse = await api.get(`/weather/current?${query}`);
        return currentResponse.data
    }

    async function getCitiesWeather() {
        for (const city of cities.value) {
            try {
                const tempData = await getWeather(
                    city.latitude,
                    city.longitude
                )

                city.temperature = tempData.temperature
                city.weatherSymbol = tempData.weatherSymbol

            } catch (error) {
                console.error(`Failed to get weather data for ${city.name}`, error)
            }
        }

        showList.value = true;
    }

    onMounted( async () => {
        await getCitiesWeather();
    })
</script>
<template>
    <div class="min-h-screen bg-blue-50 p-6">

        <h1 class="mb-6 text-2xl font-bold text-gray-800">
            Weather in Norway
        </h1>
        <div v-if="!showList">
            <button type="button" class="inline-flex items-center text-body bg-neutral-primary-soft border border-default hover:bg-neutral-secondary-medium hover:text-heading focus:ring-4 focus:ring-neutral-tertiary-soft shadow-xs font-medium leading-5 rounded-base text-sm px-4 py-2.5 focus:outline-none">
                <svg aria-hidden="true" class="w-4 h-4 text-neutral-tertiary animate-spin fill-brand me-2" viewBox="0 0 100 101" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z" fill="currentColor"/>
                    <path d="M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z" fill="currentFill"/>
                </svg>
                Loading...
            </button>
        </div>

        <ul class="space-y-3" v-if="showList">

            <li
                v-for="city in cities"
                :key="city.id"
                class="flex items-center justify-between rounded-xl bg-white p-5 shadow-sm"
            >
                <div class="flex-1">
                    <h2 class="text-lg font-semibold text-gray-800">
                        {{ city.name }}
                    </h2>
                </div>

                <div class="flex items-center gap-3">
                    <span class="text-3xl">
                        {{ getWeatherEmoji(city.weatherSymbol)}}
                    </span>

                    <span class="text-xl font-medium text-red-600">
                        {{ city.temperature }}
                    </span>
                </div>

            </li>

        </ul>

    </div>
</template>