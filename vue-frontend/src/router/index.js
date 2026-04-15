import { createRouter, createWebHistory } from 'vue-router'

const Login = () => import('@/views/Login.vue')
const Register = () => import('@/views/Register.vue')
const Catalog = () => import('@/views/Catalog.vue')
const NoteDetails = () => import('@/views/NoteDetails.vue')

const routes = [
    { path: '/login', name: 'Login', component: Login },
    { path: '/register', name: 'Register', component: Register },
    { path: '/', name: 'Catalog', component: Catalog },
    { path: '/note/:id', name: 'NoteDetails', component: NoteDetails }
]

const router = createRouter({
    history: createWebHistory(),
    routes
})

export default router