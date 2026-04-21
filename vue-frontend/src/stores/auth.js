import { defineStore } from 'pinia'
import axios from 'axios'

const ROLE_CLAIM_KEYS = [
    'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
    'https://schemas.microsoft.com/ws/2008/06/identity/claims/role',
    'role',
    'roles'
]

const decodeBase64Url = (value) => {
    const normalized = value.replace(/-/g, '+').replace(/_/g, '/')
    const padded = normalized + '='.repeat((4 - normalized.length % 4) % 4)
    return atob(padded)
}

const parseTokenPayload = (token) => {
    if (!token) return null
    const parts = token.split('.')
    if (parts.length < 2) return null

    try {
        const json = decodeBase64Url(parts[1])
        return JSON.parse(json)
    } catch {
        return null
    }
}

const extractRole = (token) => {
    const payload = parseTokenPayload(token)
    if (!payload) return null

    for (const key of ROLE_CLAIM_KEYS) {
        const value = payload[key]
        if (Array.isArray(value) && value.length > 0) return String(value[0]).toLowerCase()
        if (typeof value === 'string' && value.length > 0) return value.toLowerCase()
    }

    return null
}

const buildUser = (fullName, email, universityName, token, fallbackRole = null) => {
    const role = extractRole(token) || fallbackRole || 'student'
    return { fullName, email, universityName, role }
}

const storedToken = localStorage.getItem('accessToken')
const storedUserRaw = localStorage.getItem('user')
const storedUser = storedUserRaw ? JSON.parse(storedUserRaw) : null

if (storedUser && !storedUser.role && storedToken) {
    storedUser.role = extractRole(storedToken) || 'student'
    localStorage.setItem('user', JSON.stringify(storedUser))
}

export const useAuthStore = defineStore('auth', {
    state: () => ({
        user: storedUser,
        accessToken: storedToken,
        refreshToken: localStorage.getItem('refreshToken') || null
    }),

    getters: {
        isAuthenticated: (state) => !!state.accessToken,
        role: (state) => state.user?.role || extractRole(state.accessToken) || null,
        hasModeratorAccess: (state) => {
            const role = state.user?.role || extractRole(state.accessToken)
            return role === 'moderator' || role === 'admin'
        }
    },

    actions: {
        async login(email, password) {
            try {
                const response = await axios.post('/api/auth/login', { email, password })
                const { token, refreshToken, fullName, email: userEmail, universityName } = response.data

                this.accessToken = token
                this.refreshToken = refreshToken || null
                this.user = buildUser(fullName, userEmail, universityName, token)

                localStorage.setItem('accessToken', token)
                if (refreshToken) {
                    localStorage.setItem('refreshToken', refreshToken)
                } else {
                    localStorage.removeItem('refreshToken')
                }
                localStorage.setItem('user', JSON.stringify(this.user))

                return { success: true }
            } catch (error) {
                return {
                    success: false,
                    message:
                        error.response?.data?.message ||
                        error.response?.data?.title ||
                        'Ошибка входа'
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
                this.refreshToken = refreshToken || null
                this.user = buildUser(fullName, email, universityName, token, 'student')

                localStorage.setItem('accessToken', token)
                if (refreshToken) {
                    localStorage.setItem('refreshToken', refreshToken)
                } else {
                    localStorage.removeItem('refreshToken')
                }
                localStorage.setItem('user', JSON.stringify(this.user))

                return { success: true }
            } catch (error) {
                return {
                    success: false,
                    message:
                        error.response?.data?.message ||
                        error.response?.data?.title ||
                        'Ошибка регистрации'
                }
            }
        },

        logout() {
            this.user = null
            this.accessToken = null
            this.refreshToken = null
            localStorage.removeItem('user')
            localStorage.removeItem('accessToken')
            localStorage.removeItem('refreshToken')
        }
    }
})
