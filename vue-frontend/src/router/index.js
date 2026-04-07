import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const Login = () => import('@/views/Login.vue')
const Register = () => import('@/views/Register.vue')
const Catalog = () => import('@/views/Catalog.vue')

const routes = [
    { path: '/login', name: 'Login', component: Login, meta: { requiresGuest: true } },
    { path: '/register', name: 'Register', component: Register, meta: { requiresGuest: true } },
    { path: '/', name: 'Catalog', component: Catalog, meta: { requiresAuth: true } }
]

const router = createRouter({
    history: createWebHistory(),
    routes
})

router.beforeEach((to, from, next) => {
    const authStore = useAuthStore()

    if (to.meta.requiresAuth && !authStore.isAuthenticated) {
        next({ name: 'Login' })
    }
    else if (to.meta.requiresGuest && authStore.isAuthenticated) {
        next({ name: 'Catalog' })
    }
    else {
        next()
    }
})

export default router