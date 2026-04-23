import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const Login = () => import('@/views/Login.vue')
const Register = () => import('@/views/Register.vue')
const Catalog = () => import('@/views/Catalog.vue')
const NoteDetails = () => import('@/views/NoteDetails.vue')
const Profile = () => import('@/views/Profile.vue')
const AddNote = () => import('@/views/AddNote.vue')
const EditNote = () => import('@/views/AddNote.vue')
const ModerationDashboard = () => import('@/views/ModerationDashboard.vue')
const ModerationReview = () => import('@/views/ModerationReview.vue')

const routes = [
    { path: '/login', name: 'Login', component: Login, meta: { requiresGuest: true } },
    { path: '/register', name: 'Register', component: Register, meta: { requiresGuest: true } },
    { path: '/', name: 'Catalog', component: Catalog, meta: { requiresAuth: false } },
    { path: '/note/:id', name: 'NoteDetails', component: NoteDetails, meta: { requiresAuth: false } },
    { path: '/profile', name: 'Profile', component: Profile, meta: { requiresAuth: true, requiresStudentFlow: true } },
    { path: '/add-note', name: 'AddNote', component: AddNote, meta: { requiresAuth: true, requiresStudentFlow: true } },
    { path: '/edit-note/:id', name: 'EditNote', component: EditNote, meta: { requiresAuth: true, requiresStudentFlow: true } },
    { path: '/moderation', name: 'ModerationDashboard', component: ModerationDashboard, meta: { requiresAuth: true, requiresModerator: true } },
    { path: '/moderation/note/:id', name: 'ModerationReviewNote', component: ModerationReview, props: { mode: 'note' }, meta: { requiresAuth: true, requiresModerator: true } },
    { path: '/moderation/complaint/:id', name: 'ModerationReviewComplaint', component: ModerationReview, props: { mode: 'complaint' }, meta: { requiresAuth: true, requiresModerator: true } }
]

const router = createRouter({
    history: createWebHistory(),
    routes
})

const STUDENT_FLOW_ROUTE_NAMES = new Set([
    'Catalog',
    'NoteDetails',
    'Profile',
    'AddNote',
    'EditNote'
])

router.beforeEach((to) => {
    const authStore = useAuthStore()

    if (to.meta.requiresAuth && !authStore.isAuthenticated) {
        return { name: 'Login' }
    }

    if (to.meta.requiresGuest && authStore.isAuthenticated) {
        return authStore.hasModeratorAccess ? { name: 'ModerationDashboard' } : { name: 'Catalog' }
    }

    if (to.meta.requiresModerator && !authStore.hasModeratorAccess) {
        return authStore.isAuthenticated ? { name: 'Catalog' } : { name: 'Login' }
    }

    if (to.meta.requiresStudentFlow && authStore.hasModeratorAccess) {
        return { name: 'ModerationDashboard' }
    }

    if (authStore.hasModeratorAccess && STUDENT_FLOW_ROUTE_NAMES.has(to.name)) {
        return { name: 'ModerationDashboard' }
    }

    return true
})

export default router
