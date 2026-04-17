<template>
    <div class="catalog-page">
        <!-- Шапка -->
        <div class="header">
            <div class="logo-area">
                <div class="logo-icon">
                    <svg width="60" height="60" viewBox="0 0 69 69" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M34.5 20.125C34.5 17.075 33.2884 14.1499 31.1317 11.9933C28.9751 9.8366 26.05 8.625 23 8.625H5.75V51.75H25.875C28.1625 51.75 30.3563 52.6587 31.9738 54.2762C33.5913 55.8937 34.5 58.0875 34.5 60.375M34.5 20.125V60.375M34.5 20.125C34.5 17.075 35.7116 14.1499 37.8683 11.9933C40.0249 9.8366 42.95 8.625 46 8.625H63.25V51.75H43.125C40.8375 51.75 38.6437 52.6587 37.0262 54.2762C35.4087 55.8937 34.5 58.0875 34.5 60.375" stroke="#8B7FFF" stroke-width="4" stroke-linecap="round" stroke-linejoin="round" />
                    </svg>
                </div>
                <div class="header-text">
                    <h1>Каталог конспектов</h1>
                    <p>Делитесь конспектами и находите нужные материалы</p>
                </div>
            </div>

            <div class="avatar-menu" @click.stop="toggleMenu">
                <div class="avatar">
                    <svg width="56" height="56" viewBox="0 0 56 56" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <rect width="56" height="56" rx="28" fill="#2A2348" />
                        <path fill-rule="evenodd" clip-rule="evenodd" d="M36.4003 22.4C36.4003 27.0392 32.6395 30.8 28.0003 30.8C23.3612 30.8 19.6003 27.0392 19.6003 22.4C19.6003 17.7608 23.3612 14 28.0003 14C32.6395 14 36.4003 17.7608 36.4003 22.4ZM33.6003 22.4C33.6003 25.4928 31.0931 28 28.0003 28C24.9075 28 22.4003 25.4928 22.4003 22.4C22.4003 19.3072 24.9075 16.8 28.0003 16.8C31.0931 16.8 33.6003 19.3072 33.6003 22.4Z" fill="#6057FD" />
                        <path d="M28.0003 35C18.9362 35 11.2133 40.3598 8.27148 47.8689C8.98814 48.5805 9.74308 49.2536 10.5329 49.8849C12.7236 42.9908 19.5957 37.8 28.0003 37.8C36.4049 37.8 43.2771 42.9908 45.4678 49.8849C46.2576 49.2536 47.0126 48.5805 47.7292 47.8689C44.7874 40.3598 37.0645 35 28.0003 35Z" fill="#6057FD" />
                    </svg>
                </div>
                <div v-if="isMenuOpen" class="dropdown-menu">
                    <template v-if="authStore.isAuthenticated">
                        <div class="user-name">{{ authStore.user?.fullName }}</div>
                        <div class="menu-item" @click="goToProfile">Мои конспекты</div>
                        <div class="menu-item" @click="goToAddNote">Добавить конспект</div>
                        <div class="menu-item logout" @click="handleLogout">Выйти</div>
                    </template>
                    <template v-else>
                        <div class="menu-item" @click="goToLogin">Войти</div>
                        <div class="menu-item" @click="goToRegister">Зарегистрироваться</div>
                    </template>
                </div>
            </div>
        </div>

        <!-- Поиск -->
        <div class="search-bar">
            <span class="search-icon"></span>
            <input type="text" v-model="searchQuery" placeholder="Поиск конспектов..." />
        </div>

        <!-- Фильтры -->
        <div class="filters">
            <div class="filter-item">
                <select v-model="filters.university">
                    <option value="">Все вузы</option>
                    <option v-for="uni in universities" :key="uni" :value="uni">{{ uni }}</option>
                </select>
            </div>
            <div class="filter-item">
                <select v-model="filters.subject">
                    <option value="">Все предметы</option>
                    <option v-for="subj in subjects" :key="subj" :value="subj">{{ subj }}</option>
                </select>
            </div>
            <div class="filter-item">
                <input type="text" v-model="filters.teacher" placeholder="Все преподаватели" />
            </div>
        </div>

        <div class="results-count">Показано {{ filteredNotes.length }} результатов</div>

        <div class="notes-grid">
            <div v-for="note in filteredNotes" :key="note.id" class="note-card">
                <div class="card-icon">
                    <div class="file-icon"></div>
                </div>
                <div class="card-content">
                    <h3>{{ note.title }}</h3>
                    <div class="subject">{{ note.subject }}</div>
                    <div class="teacher-university">
                        <div>Преподаватель: {{ note.teacher }}</div>
                        <div>Вуз: {{ note.university }}</div>
                    </div>
                    <div class="card-footer">
                        <div class="rating">
                            <span class="star"></span>
                            <span>{{ note.rating }}</span>
                        </div>
                        <div class="downloads">{{ note.downloadsCount }} скачиваний</div>
                        <button class="open-btn" @click="openNote(note.id)">Открыть</button>
                    </div>
                </div>
                <div class="like-btn" @click.stop="toggleFavorite(note)">
                    <div class="heart" :class="{ liked: favoritesStore.isFavorite(note.id) }"></div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
    import { ref, reactive, computed, onMounted, onUnmounted } from 'vue'
    import { useRouter } from 'vue-router'
    import { useAuthStore } from '@/stores/auth'
    import { useFavoritesStore } from '@/stores/favorites'
    import api from '@/services/api'

    const router = useRouter()
    const authStore = useAuthStore()
    const favoritesStore = useFavoritesStore()

    const isMenuOpen = ref(false)
    const searchQuery = ref('')
    const filters = reactive({
        university: '',
        subject: '',
        teacher: ''
    })

    const universities = ref([])
    const subjects = ref([])
    const allTeachers = ref([])

    // Конспекты загружаем из API
    const notes = ref([])

    // Фильтрация конспектов
    const filteredNotes = computed(() => {
        return notes.value.filter(note => {
            const matchSearch = note.title.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
                note.subject.toLowerCase().includes(searchQuery.value.toLowerCase())
            const matchUniversity = !filters.university || note.university === filters.university
            const matchSubject = !filters.subject || note.subject === filters.subject
            const matchTeacher = !filters.teacher || note.teacher.toLowerCase().includes(filters.teacher.toLowerCase())
            return matchSearch && matchUniversity && matchSubject && matchTeacher
        })
    })

    // Загрузка конспектов с сервера
    const loadNotes = async () => {
        try {
            const response = await api.get('/notes')
            notes.value = response.data
        } catch (error) {
            console.error('Ошибка загрузки конспектов:', error)
        }
    }

    // Загрузка фильтров
    const loadUniversities = async () => {
        try {
            const response = await api.get('/lookup/all-universities')
            universities.value = response.data.map(u => u.name)
        } catch (error) {
            console.error('Ошибка загрузки университетов:', error)
        }
    }

    const loadSubjects = async () => {
        try {
            const response = await api.get('/lookup/all-subjects')
            subjects.value = response.data.map(s => s.name)
        } catch (error) {
            console.error('Ошибка загрузки предметов:', error)
        }
    }

    const loadTeachers = async () => {
        try {
            const response = await api.get('/lookup/all-teachers')
            allTeachers.value = response.data.map(t => t.fullName)
        } catch (error) {
            console.error('Ошибка загрузки преподавателей:', error)
        }
    }

    // Методы меню
    const toggleMenu = () => {
        isMenuOpen.value = !isMenuOpen.value
    }

    const closeMenu = () => {
        isMenuOpen.value = false
    }

    const goToLogin = () => {
        router.push('/login')
        closeMenu()
    }

    const goToRegister = () => {
        router.push('/register')
        closeMenu()
    }

    const goToProfile = () => {
        router.push('/profile')
        closeMenu()
    }

    const goToAddNote = () => {
        router.push('/add-note')
        closeMenu()
    }

    const handleLogout = () => {
        authStore.logout()
        closeMenu()
        router.push('/login')
    }

    const openNote = (id) => {
        router.push(`/note/${id}`)
    }

    const toggleFavorite = async (note) => {
        const isNowFavorite = await favoritesStore.toggleFavorite(note)
        const currentNote = notes.value.find(n => n.id === note.id)
        if (currentNote) {
            currentNote.isFavorite = isNowFavorite
        }
    }

    const handleClickOutside = (event) => {
        if (!event.target.closest('.avatar-menu')) {
            isMenuOpen.value = false
        }
    }

    onMounted(() => {
        document.addEventListener('click', handleClickOutside)
        loadNotes()
        loadUniversities()
        loadSubjects()
        loadTeachers()
    })

    onUnmounted(() => {
        document.removeEventListener('click', handleClickOutside)
    })
</script>

<style scoped>
    .catalog-page {
        min-height: 100vh;
        background: #0A0A0F;
        padding: 32px 80px;
        font-family: 'Inter', system-ui, -apple-system, sans-serif;
    }

    .header {
        display: flex;
        justify-content: space-between;
        align-items: flex-start;
        margin-bottom: 32px;
    }

    .logo-area {
        display: flex;
        gap: 20px;
        align-items: center;
    }

    .logo-icon {
        width: 76px;
        height: 76px;
        background: #2A2348;
        border-radius: 10px;
        display: flex;
        align-items: center;
        justify-content: center;
    }

        .logo-icon svg {
            width: 60px;
            height: 60px;
        }

    .header-text h1 {
        font-size: 40px;
        font-weight: 700;
        color: #FFFFFF;
        margin: 0 0 8px 0;
    }

    .header-text p {
        font-size: 20px;
        font-weight: 400;
        color: #A0A0B0;
        margin: 0;
    }

    .avatar-menu {
        position: relative;
        cursor: pointer;
    }

    .avatar {
        width: 56px;
        height: 56px;
        border-radius: 50%;
        overflow: hidden;
    }

    .dropdown-menu {
        position: absolute;
        top: 70px;
        right: 0;
        width: 320px;
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 14px;
        box-shadow: 0px 8px 24px rgba(0, 0, 0, 0.4);
        z-index: 100;
    }

    .user-name {
        padding: 16px 20px;
        font-size: 24px;
        font-weight: 600;
        color: #FFFFFF;
        border-bottom: 1px solid #2A2A35;
    }

    .menu-item {
        padding: 12px 20px;
        font-size: 20px;
        font-weight: 500;
        color: #FFFFFF;
        cursor: pointer;
        transition: background 0.2s;
    }

        .menu-item:hover {
            background: #2A2A35;
        }

        .menu-item.logout {
            color: #6C63FF;
            border-top: 1px solid #2A2A35;
        }

    .search-bar {
        position: relative;
        width: 100%;
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 16px;
        margin-bottom: 24px;
    }

    .search-icon {
        position: absolute;
        left: 16px;
        top: 50%;
        transform: translateY(-50%);
        width: 24px;
        height: 24px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%237F8499" stroke-width="2"%3E%3Ccircle cx="11" cy="11" r="8"/%3E%3Cline x1="21" y1="21" x2="16.65" y2="16.65"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .search-bar input {
        width: 100%;
        padding: 18px 20px 18px 52px;
        background: transparent;
        border: none;
        font-size: 24px;
        color: #FFFFFF;
    }

        .search-bar input::placeholder {
            color: #7F8499;
        }

        .search-bar input:focus {
            outline: none;
        }

    .filters {
        display: flex;
        gap: 28px;
        margin-bottom: 24px;
    }

    .filter-item {
        flex: 1;
    }

        .filter-item select,
        .filter-item input {
            width: 100%;
            padding: 14px 20px;
            background: #1A1A22;
            border: 1px solid #2A2A35;
            border-radius: 16px;
            font-size: 24px;
            color: #FFFFFF;
            appearance: none;
            cursor: pointer;
        }

        .filter-item input {
            appearance: none;
            cursor: text;
        }

            .filter-item select:focus,
            .filter-item input:focus {
                outline: none;
                border-color: #8B7FFF;
            }

    .results-count {
        font-size: 18px;
        color: #A0A0B0;
        margin-bottom: 24px;
    }

    .notes-grid {
        display: grid;
        grid-template-columns: repeat(2, 1fr);
        gap: 32px;
    }

    .note-card {
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 24px;
        padding: 32px;
        display: flex;
        position: relative;
        transition: transform 0.2s, box-shadow 0.2s;
        min-height: 280px;
    }

        .note-card:hover {
            transform: translateY(-4px);
            box-shadow: 0px 8px 4px rgba(108, 99, 255, 0.15);
        }

    .card-icon {
        width: 100px;
        height: 110px;
        background: #2A2348;
        border-radius: 16px;
        display: flex;
        align-items: center;
        justify-content: center;
        margin-right: 32px;
        flex-shrink: 0;
    }

    .file-icon {
        width: 65px;
        height: 70px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%236C63FF" stroke-width="2"%3E%3Cpath d="M13 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9z"/%3E%3Cpolyline points="13 2 13 9 20 9"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .card-content {
        flex: 1;
        display: flex;
        flex-direction: column;
        justify-content: space-between;
    }

        .card-content h3 {
            font-size: 32px;
            font-weight: 700;
            color: #FFFFFF;
            margin: 0 0 12px 0;
        }

    .subject {
        font-size: 22px;
        font-weight: 700;
        color: #A0A0B0;
        margin-bottom: 20px;
    }

    .teacher-university {
        font-size: 20px;
        font-weight: 700;
        color: #9A9A9F;
        line-height: 1.6;
        margin-bottom: 24px;
    }

    .card-footer {
        display: flex;
        align-items: center;
        gap: 32px;
        padding-top: 20px;
        border-top: 1px solid #2A2A35;
        flex-wrap: wrap;
    }

    .rating {
        display: flex;
        align-items: center;
        gap: 8px;
    }

    .star {
        width: 28px;
        height: 28px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="%23FFE100"%3E%3Cpolygon points="12 2 15 9 22 9 16 14 18 21 12 17 6 21 8 14 2 9 9 9 12 2"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .rating span {
        font-size: 22px;
        font-weight: 700;
        color: #FFFFFF;
    }

    .downloads {
        font-size: 20px;
        font-weight: 700;
        color: #A0A0B0;
    }

    .open-btn {
        background: #6C63FF;
        border: none;
        border-radius: 16px;
        padding: 12px 32px;
        font-size: 22px;
        font-weight: 600;
        color: #FFFFFF;
        cursor: pointer;
        transition: all 0.2s;
        margin-left: auto;
    }

        .open-btn:hover {
            background: #594FFE;
            transform: translateY(-2px);
        }

    .like-btn {
        position: absolute;
        top: 24px;
        right: 24px;
        cursor: pointer;
    }

    .heart {
        width: 36px;
        height: 36px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%237F8499" stroke-width="2"%3E%3Cpath d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
        transition: all 0.2s;
    }

        .heart.liked {
            background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="%23EF4444" stroke="%23EF4444" stroke-width="1"%3E%3Cpath d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/%3E%3C/svg%3E') no-repeat center;
            background-size: contain;
        }

    @media (max-width: 1100px) {
        .catalog-page {
            padding: 20px 40px;
        }

        .notes-grid {
            gap: 24px;
        }

        .note-card {
            padding: 24px;
        }

        .card-content h3 {
            font-size: 28px;
        }

        .subject {
            font-size: 20px;
        }

        .teacher-university {
            font-size: 18px;
        }

        .open-btn {
            padding: 10px 24px;
            font-size: 20px;
        }
    }

    @media (max-width: 900px) {
        .notes-grid {
            grid-template-columns: 1fr;
        }

        .header {
            flex-direction: column;
            gap: 16px;
        }

        .logo-area {
            flex-wrap: wrap;
        }

        .filters {
            flex-direction: column;
            gap: 12px;
        }

        .filter-item select,
        .filter-item input {
            font-size: 18px;
            padding: 12px 16px;
        }

        .search-bar input {
            font-size: 18px;
        }
    }

    @media (max-width: 560px) {
        .catalog-page {
            padding: 16px;
        }

        .header-text h1 {
            font-size: 28px;
        }

        .header-text p {
            font-size: 16px;
        }

        .note-card {
            flex-direction: column;
        }

        .card-icon {
            margin-bottom: 20px;
        }

        .card-footer {
            flex-wrap: wrap;
        }

        .open-btn {
            margin-left: 0;
        }

        .card-content h3 {
            font-size: 24px;
        }

        .subject {
            font-size: 18px;
        }

        .teacher-university {
            font-size: 16px;
        }

        .rating span,
        .downloads {
            font-size: 18px;
        }

        .open-btn {
            font-size: 18px;
            padding: 8px 20px;
        }
    }
</style>