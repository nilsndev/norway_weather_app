import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'

import MainLayout from '../layouts/MainLayout.vue'
import HomeView from '../views/HomeView.vue'
import MapView from '../views/MapView.vue'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),

  routes: [
    {
      path: '/',
      component: MainLayout,
      meta: { requiresAuth: true },
      children: [
        {
          path: '/',
          name: 'home',
          component: HomeView,
        },
        {
          path: '/map',
          name: 'map',
          component: MapView,
        },
      ],
    },

    {
      path: '/login',
      name: 'login',
      component: LoginView,
      meta: { guestOnly: true },
    },

    {
      path: '/register',
      name: 'register',
      component: RegisterView,
      meta: { guestOnly: true },
    },
  ],
})

router.beforeEach(async (to) => {
  const auth = useAuthStore()

  await auth.initializeAuth()

  if (to.meta.requiresAuth && !auth.isAuthenticated) {
    return '/login'
  }

  if (to.meta.guestOnly && auth.isAuthenticated) {
    return '/'
  }
})

export default router