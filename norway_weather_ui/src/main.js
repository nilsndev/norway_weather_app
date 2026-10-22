import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'
import { setUnauthorizedHandler } from './services/api'
import { useAuthStore } from './stores/auth'
import './style.css'

// Remove the legacy bearer token once; authentication now uses an HttpOnly cookie.
localStorage.removeItem('auth_token')

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
app.use(router)

const auth = useAuthStore(pinia)
setUnauthorizedHandler(() => {
  auth.logout()
  if (router.currentRoute.value.path !== '/login') router.push('/login')
})

app.mount('#app')
