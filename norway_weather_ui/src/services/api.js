import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  withCredentials: true,
})

let unauthorizedHandler

export function setUnauthorizedHandler(handler) {
  unauthorizedHandler = handler
}

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const authEndpoint = /^\/auth\/(login|register|logout)(\?|$)/.test(error.config?.url || '')
    if (error.response?.status === 401 && !authEndpoint) {
      unauthorizedHandler?.()
    }
    return Promise.reject(error)
  },
)

export default api
