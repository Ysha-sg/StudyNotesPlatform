import { defineStore } from 'pinia'
import axios from 'axios'

export const useAuthStore = defineStore('auth', {
    state: () => ({
        user: JSON.parse(localStorage.getItem('user')) || null,
        accessToken: localStorage.getItem('accessToken') || null,
        refreshToken: localStorage.getItem('refreshToken') || null,
    }),

    getters: {
        isAuthenticated: (state) => !!state.accessToken,
    },

    actions: {
        async login(email, password) {
            try {
                const response = await axios.post('/api/auth/login', { email, password })
                const { token, refreshToken, fullName, email: userEmail, universityName } = response.data

                this.accessToken = token
                this.refreshToken = refreshToken
                this.user = { fullName, email: userEmail, universityName }

                localStorage.setItem('accessToken', token)
                localStorage.setItem('refreshToken', refreshToken)
                localStorage.setItem('user', JSON.stringify(this.user))

                return { success: true }
            } catch (error) {
                return {
                    success: false,
                    message: error.response?.data?.message || 'Ошибка входа'
                }
            }
        },

        async register(fullName, email, password, universityName) {
            try {
                const response = await axios.post('/api/auth/register', {
                    fullName,
                    email,
                    password,
                    universityName
                })
                const { token, refreshToken } = response.data

                this.accessToken = token
                this.refreshToken = refreshToken
                this.user = { fullName, email, universityName }

                localStorage.setItem('accessToken', token)
                localStorage.setItem('refreshToken', refreshToken)
                localStorage.setItem('user', JSON.stringify(this.user))

                return { success: true }
            } catch (error) {
                return {
                    success: false,
                    message: error.response?.data?.message || 'Ошибка регистрации'
                }
            }
        },

        logout() {
            this.user = null
            this.accessToken = null
            this.refreshToken = null
            localStorage.clear()
        }
    }
})