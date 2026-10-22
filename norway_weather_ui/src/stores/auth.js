import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import api from '../services/api'

export const useAuthStore = defineStore('auth', () => {
  const user = ref(null)
  const loading = ref(false)
  const isAuthenticated = computed(() => Boolean(user.value))
  let initialization

  function clearAuth() {
    user.value = null
  }

  async function login(email, password) {
    loading.value = true
    try {
      await api.post('/auth/login', { email, password })
      await fetchCurrentUser()
      return user.value
    } catch (error) {
      clearAuth()
      throw error
    } finally {
      loading.value = false
    }
  }

  async function register(email, password) {
    loading.value = true
    try {
      await api.post('/auth/register', { email, password })
    } finally {
      loading.value = false
    }
  }

  async function fetchCurrentUser() {
    const { data } = await api.get('/auth/me')
    user.value = data
    return data
  }

  function initializeAuth() {
    if (!initialization) {
      initialization = fetchCurrentUser()
        .catch((error) => {
          if (error.response?.status === 401) clearAuth()
          else throw error
        })
        .finally(() => {
          initialization = null
        })
    }
    return initialization
  }

  async function logout() {
    try {
      await api.post('/auth/logout')
    } catch {
      // Clear local UI state even when the server cannot be reached.
    } finally {
      clearAuth()
    }
  }

  return {
    user,
    loading,
    isAuthenticated,
    login,
    register,
    fetchCurrentUser,
    initializeAuth,
    logout,
  }
})
