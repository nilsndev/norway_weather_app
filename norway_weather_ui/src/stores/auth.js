import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import api from '../services/api'

const TOKEN_KEY = 'auth_token'

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem(TOKEN_KEY))
  const user = ref(null)
  const loading = ref(false)
  const isAuthenticated = computed(() => Boolean(token.value && user.value))
  let initialization

  function clearAuth() {
    token.value = null
    user.value = null
    localStorage.removeItem(TOKEN_KEY)
  }

  async function login(email, password) {
    loading.value = true
    try {
      const { data } = await api.post('/auth/login', { email, password })
      token.value = data.token
      localStorage.setItem(TOKEN_KEY, data.token)
      await fetchCurrentUser()
      return user.value
    } catch (error) {
      if (error.response?.status !== 401) clearAuth()
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
    if (!token.value) return Promise.resolve()
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

  function logout() {
    clearAuth()
  }

  return {
    token,
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
