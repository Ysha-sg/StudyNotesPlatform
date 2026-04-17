import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const Login = () => import('@/views/Login.vue')
const Register = () => import('@/views/Register.vue')
const Catalog = () => import('@/views/Catalog.vue')
const NoteDetails = () => import('@/views/NoteDetails.vue')
const Profile = () => import('@/views/Profile.vue')
const AddNote = () => import('@/views/AddNote.vue')
const EditNote = () => import('@/views/AddNote.vue')  // 👈 ДОБАВЬ ЭТУ СТРОКУ

const routes = [
    { path: '/login', name: 'Login', component: Login, meta: { requiresGuest: true } },
    { path: '/register', name: 'Register', component: Register, meta: { requiresGuest: true } },
    { path: '/', name: 'Catalog', component: Catalog, meta: { requiresAuth: true } },
    { path: '/note/:id', name: 'NoteDetails', component: NoteDetails, meta: { requiresAuth: false } },
    { path: '/profile', name: 'Profile', component: Profile, meta: { requiresAuth: true } },
    { path: '/add-note', name: 'AddNote', component: AddNote, meta: { requiresAuth: true } },
    { path: '/edit-note/:id', name: 'EditNote', component: EditNote, meta: { requiresAuth: true } }  // 👈 ДОБАВЬ ЭТУ СТРОКУ
]

const router = createRouter({
    history: createWebHistory(),
    routes
})

router.beforeEach((to, from) => {
    const authStore = useAuthStore()

    if (to.meta.requiresAuth && !authStore.isAuthenticated) {
        return { name: 'Login' }
    }

    if (to.meta.requiresGuest && authStore.isAuthenticated) {
        return { name: 'Catalog' }
    }

    return true
})

export default router