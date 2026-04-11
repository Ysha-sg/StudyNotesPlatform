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

            <!-- Аватар с выпадающим меню -->
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
                        <div class="menu-item" @click="goToMyNotes">Мои конспекты</div>
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
            <!-- Фильтр по вузу -->
            <div class="filter-item">
                <select v-model="filters.university">
                    <option value="">Все вузы</option>
                    <option v-for="uni in universities" :key="uni" :value="uni">{{ uni }}</option>
                </select>
                <span class="chevron"></span>
            </div>

            <!-- Фильтр по предмету -->
            <div class="filter-item">
                <select v-model="filters.subject">
                    <option value="">Все предметы</option>
                    <option v-for="subj in subjects" :key="subj" :value="subj">{{ subj }}</option>
                </select>
                <span class="chevron"></span>
            </div>

            <!-- Фильтр по преподавателю (комбобокс) -->
            <div class="filter-item combobox">
                <input type="text"
                       v-model="filters.teacherInput"
                       @input="onTeacherInput"
                       @focus="showTeacherDropdown = true"
                       placeholder="Все преподаватели" />
                <span class="chevron" @click="toggleTeacherDropdown"></span>
                <div v-if="showTeacherDropdown && filteredTeachers.length > 0" class="dropdown-list">
                    <div v-for="teacher in filteredTeachers"
                         :key="teacher"
                         class="dropdown-option"
                         @click="selectTeacher(teacher)">
                        {{ teacher }}
                    </div>
                </div>
            </div>
        </div>

        <!-- Результаты -->
        <div class="results-count">Показано {{ filteredNotes.length }} результатов</div>

        <!-- Сетка карточек — 2 в строку, карточки широкие -->
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
                        <div class="downloads">{{ note.downloads }} скачиваний</div>
                        <button class="open-btn" @click="openNote(note.id)">Открыть</button>
                    </div>
                </div>
                <div class="like-btn" @click.stop="toggleFavorite(note.id)">
                    <div class="heart" :class="{ liked: note.isFavorite }"></div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
    import { ref, reactive, computed, onMounted, onUnmounted } from 'vue'
    import { useRouter } from 'vue-router'
    import { useAuthStore } from '@/stores/auth'

    const router = useRouter()
    const authStore = useAuthStore()

    // Состояние меню
    const isMenuOpen = ref(false)

    // Поиск и фильтры
    const searchQuery = ref('')
    const filters = reactive({
        university: '',
        subject: '',
        teacherInput: '',
        teacher: ''
    })

    // Состояние выпадающего списка преподавателей
    const showTeacherDropdown = ref(false)

    // Данные для фильтров
    const universities = ['МГУ', 'СПбГУ', 'МФТИ', 'НИУ ВШЭ', 'ПГНИУ', 'ПГГПУ']
    const subjects = ['Базы данных', 'Алгоритмы', 'Педагогика', 'Дискретная математика']
    const allTeachers = ['Проф. Иванов А.С.', 'Доц. Смирнов И.В.', 'Проф. Кузнецова Е.В.', 'Доц. Орлов Д.М.']

    // Отфильтрованные преподаватели
    const filteredTeachers = computed(() => {
        if (!filters.teacherInput) return allTeachers
        return allTeachers.filter(t => t.toLowerCase().includes(filters.teacherInput.toLowerCase()))
    })

    // Тестовые конспекты
    const notes = ref([
        {
            id: 1,
            title: 'Введение в базы данных',
            subject: 'Базы данных',
            teacher: 'Проф. Иванов А.С.',
            university: 'ПГНИУ',
            rating: 9.8,
            downloads: 1250,
            isFavorite: false
        },
        {
            id: 2,
            title: 'Алгоритмы и структуры данных',
            subject: 'Алгоритмы',
            teacher: 'Доц. Смирнов И.В.',
            university: 'СПбГУ',
            rating: 9.6,
            downloads: 980,
            isFavorite: false
        },
        {
            id: 3,
            title: 'Педагогика: основы обучения',
            subject: 'Педагогика',
            teacher: 'Проф. Кузнецова Е.В.',
            university: 'ПГГПУ',
            rating: 9.9,
            downloads: 760,
            isFavorite: false
        },
        {
            id: 4,
            title: 'Дискретная математика',
            subject: 'Дискретная математика',
            teacher: 'Доц. Орлов Д.М.',
            university: 'ПГНИУ',
            rating: 9.7,
            downloads: 1100,
            isFavorite: false
        }
    ])

    // Фильтрация
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

    // Методы для фильтра преподавателей
    const onTeacherInput = () => {
        filters.teacher = filters.teacherInput
        showTeacherDropdown.value = true
    }

    const toggleTeacherDropdown = () => {
        showTeacherDropdown.value = !showTeacherDropdown.value
    }

    const selectTeacher = (teacher) => {
        filters.teacherInput = teacher
        filters.teacher = teacher
        showTeacherDropdown.value = false
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

    const goToMyNotes = () => {
        router.push('/my-notes')
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

    const toggleFavorite = (id) => {
        const note = notes.value.find(n => n.id === id)
        if (note) {
            note.isFavorite = !note.isFavorite
        }
    }

    // Закрытие меню при клике вне
    const handleClickOutside = (event) => {
        if (!event.target.closest('.avatar-menu')) {
            isMenuOpen.value = false
        }
        if (!event.target.closest('.combobox')) {
            showTeacherDropdown.value = false
        }
    }

    onMounted(() => {
        document.addEventListener('click', handleClickOutside)
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

    /* Шапка */
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

    /* Аватар и меню */
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
        font-size: 24px;
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

    /* Поиск */
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

    /* Фильтры */
    .filters {
        display: flex;
        gap: 28px;
        margin-bottom: 24px;
    }

    .filter-item {
        position: relative;
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

    .chevron {
        position: absolute;
        right: 20px;
        top: 50%;
        transform: translateY(-50%);
        width: 24px;
        height: 24px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23FFFFFF" stroke-width="3"%3E%3Cpolyline points="6 9 12 15 18 9"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
        pointer-events: none;
    }

    .combobox .chevron {
        pointer-events: auto;
        cursor: pointer;
    }

    .dropdown-list {
        position: absolute;
        top: 100%;
        left: 0;
        right: 0;
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 16px;
        margin-top: 8px;
        z-index: 100;
        max-height: 200px;
        overflow-y: auto;
    }

    .dropdown-option {
        padding: 12px 20px;
        font-size: 20px;
        color: #FFFFFF;
        cursor: pointer;
        transition: background 0.2s;
    }

        .dropdown-option:hover {
            background: #2A2A35;
        }

    /* Результаты */
    .results-count {
        font-size: 18px;
        color: #A0A0B0;
        margin-bottom: 24px;
    }

    /* СЕТКА КАРТОЧЕК — 2 В СТРОКУ, КАРТОЧКИ ШИРОКИЕ */
    .notes-grid {
        display: grid;
        grid-template-columns: repeat(2, 1fr);
        gap: 32px;
    }

    /* КАРТОЧКА — УВЕЛИЧЕННАЯ В 2 РАЗА */
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

    /* Иконка файла — увеличенная */
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

    /* Контент карточки */
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

    /* Футер карточки */
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
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="%23FFE100"%3E%3Cpath d="M12 17.27L18.18 21l-1.64-7.03L22 9.24l-7.19-.61L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21z"/%3E%3C/svg%3E') no-repeat center;
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

    /* Сердечко — увеличенное */
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

    /* Адаптивность */
    @media (max-width: 1400px) {
        .catalog-page {
            padding: 32px 40px;
        }
    }

    @media (max-width: 1100px) {
        .catalog-page {
            padding: 24px 32px;
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