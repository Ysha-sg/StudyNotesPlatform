import axios from 'axios'

const api = axios.create({
    baseURL: '/api',
    headers: {
        'Content-Type': 'application/json'
    }
})

const redirectToLogin = () => {
    const currentPath = window.location.pathname + window.location.search + window.location.hash
    const safePath = currentPath.startsWith('/') ? currentPath : '/'
    const encoded = encodeURIComponent(safePath)
    window.location.href = `/login?redirect=${encoded}`
}

// Перехватчик запросов — добавляем токен авторизации
api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('accessToken')
        if (token) {
            config.headers.Authorization = `Bearer ${token}`
        }
        return config
    },
    (error) => {
        return Promise.reject(error)
    }
)

// Перехватчик ответов — обработка ошибок
api.interceptors.response.use(
    (response) => {
        return response
    },
    async (error) => {
        const originalRequest = error.config

        // Если ошибка 401 (Unauthorized) и это не запрос на обновление токена
        if (error.response?.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true

            const refreshToken = localStorage.getItem('refreshToken')
            if (refreshToken) {
                try {
                    const response = await axios.post('/api/auth/refresh', { refreshToken })
                    const { accessToken, refreshToken: newRefreshToken } = response.data

                    localStorage.setItem('accessToken', accessToken)
                    localStorage.setItem('refreshToken', newRefreshToken)

                    originalRequest.headers.Authorization = `Bearer ${accessToken}`
                    return api(originalRequest)
                } catch (refreshError) {
                    // Если не удалось обновить токен — разлогиниваем
                    localStorage.clear()
                    redirectToLogin()
                    return Promise.reject(refreshError)
                }
            } else {
                localStorage.clear()
                redirectToLogin()
            }
        }

        return Promise.reject(error)
    }
)

export default api
