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
                    <div class="avatar-placeholder"></div>
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
            <div class="filter-item">
                <select v-model="filters.university">
                    <option value="">Все вузы</option>
                    <option v-for="uni in universities" :key="uni" :value="uni">{{ uni }}</option>
                </select>
                <span class="chevron"></span>
            </div>
            <div class="filter-item">
                <select v-model="filters.subject">
                    <option value="">Все предметы</option>
                    <option v-for="subj in subjects" :key="subj" :value="subj">{{ subj }}</option>
                </select>
                <span class="chevron"></span>
            </div>
            <div class="filter-item">
                <input type="text" v-model="filters.teacher" placeholder="Все преподаватели" />
            </div>
        </div>

        <!-- Результаты -->
        <div class="results-count">Показано {{ filteredNotes.length }} результатов</div>

        <!-- Сетка карточек -->
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
                <div class="like-btn" @click="toggleLike(note.id)">
                    <div class="heart" :class="{ liked: note.isLiked }"></div>
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
  teacher: ''
})

// Данные для фильтров
const universities = ['МГУ', 'СПбГУ', 'МФТИ', 'НИУ ВШЭ', 'ПГНИУ', 'ПГГПУ']
const subjects = ['Базы данных', 'Алгоритмы', 'Педагогика', 'Дискретная математика']

// Тестовые конспекты (потом заменим на API)
const notes = ref([
  {
    id: 1,
    title: 'Введение в базы данных',
    subject: 'Базы данных',
    teacher: 'Проф. Иванов А.С.',
    university: 'ПГНИУ',
    rating: 9.8,
    downloads: 1250,
    isLiked: false
  },
  {
    id: 2,
    title: 'Алгоритмы и структуры данных',
    subject: 'Алгоритмы',
    teacher: 'Доц. Смирнов И.В.',
    university: 'СПбГУ',
    rating: 9.6,
    downloads: 980,
    isLiked: false
  },
  {
    id: 3,
    title: 'Педагогика: основы обучения',
    subject: 'Педагогика',
    teacher: 'Проф. Кузнецова Е.В.',
    university: 'ПГГПУ',
    rating: 9.9,
    downloads: 760,
    isLiked: false
  },
  {
    id: 4,
    title: 'Дискретная математика',
    subject: 'Дискретная математика',
    teacher: 'Доц. Орлов Д.М.',
    university: 'ПГНИУ',
    rating: 9.7,
    downloads: 1100,
    isLiked: false
  }
])

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

// Методы
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

const toggleLike = (id) => {
  const note = notes.value.find(n => n.id === id)
  if (note) {
    note.isLiked = !note.isLiked
  }
}

// Закрытие меню при клике вне
const handleClickOutside = (event) => {
  if (!event.target.closest('.avatar-menu')) {
    isMenuOpen.value = false
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
        background: #2A2348;
        border-radius: 50%;
        display: flex;
        align-items: center;
        justify-content: center;
    }

    .avatar-placeholder {
        width: 40px;
        height: 40px;
        background: #6057FD;
        border-radius: 50%;
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

    /* Результаты */
    .results-count {
        font-size: 18px;
        color: #A0A0B0;
        margin-bottom: 24px;
    }

    /* Сетка карточек */
    .notes-grid {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(620px, 1fr));
        gap: 24px;
    }

    /* Карточка */
    .note-card {
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 20px;
        padding: 20px;
        display: flex;
        position: relative;
        transition: transform 0.2s, box-shadow 0.2s;
    }

        .note-card:hover {
            transform: translateY(-4px);
            box-shadow: 0px 8px 4px rgba(108, 99, 255, 0.15);
        }

    .card-icon {
        width: 62px;
        height: 66px;
        background: #2A2348;
        border-radius: 10px;
        display: flex;
        align-items: center;
        justify-content: center;
        margin-right: 20px;
        flex-shrink: 0;
    }

    .file-icon {
        width: 45px;
        height: 50px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%236C63FF" stroke-width="2"%3E%3Cpath d="M13 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9z"/%3E%3Cpolyline points="13 2 13 9 20 9"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .card-content {
        flex: 1;
    }

        .card-content h3 {
            font-size: 26px;
            font-weight: 700;
            color: #FFFFFF;
            margin: 0 0 8px 0;
        }

    .subject {
        font-size: 18px;
        font-weight: 700;
        color: #A0A0B0;
        margin-bottom: 16px;
    }

    .teacher-university {
        font-size: 18px;
        font-weight: 700;
        color: #9A9A9F;
        line-height: 1.5;
        margin-bottom: 16px;
    }

    .card-footer {
        display: flex;
        align-items: center;
        gap: 24px;
        padding-top: 16px;
        border-top: 1px solid #2A2A35;
    }

    .rating {
        display: flex;
        align-items: center;
        gap: 8px;
    }

    .star {
        width: 24px;
        height: 24px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="%23FFE100"%3E%3Cpath d="M12 17.27L18.18 21l-1.64-7.03L22 9.24l-7.19-.61L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21z"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .rating span {
        font-size: 20px;
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
        border-radius: 14px;
        padding: 8px 24px;
        font-size: 20px;
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
        top: 20px;
        right: 20px;
        cursor: pointer;
    }

    .heart {
        width: 32px;
        height: 32px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%237F8499" stroke-width="2"%3E%3Cpath d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
        transition: all 0.2s;
    }

        .heart.liked {
            background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="%23EF4444" stroke="%23EF4444" stroke-width="1"%3E%3Cpath d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/%3E%3C/svg%3E') no-repeat center;
            background-size: contain;
        }

    /* Адаптивность */
    @media (max-width: 1200px) {
        .catalog-page {
            padding: 20px 40px;
        }

        .notes-grid {
            grid-template-columns: 1fr;
        }
    }

    @media (max-width: 768px) {
        .catalog-page {
            padding: 16px;
        }

        .header {
            flex-direction: column;
            gap: 16px;
        }

        .logo-area {
            flex-wrap: wrap;
        }

        .header-text h1 {
            font-size: 28px;
        }

        .header-text p {
            font-size: 16px;
        }

        .filters {
            flex-direction: column;
            gap: 12px;
        }

        .filter-item select,
        .filter-item input {
            font-size: 16px;
            padding: 10px 16px;
        }

        .search-bar input {
            font-size: 16px;
            padding: 12px 16px 12px 44px;
        }

        .note-card {
            flex-direction: column;
        }

        .card-icon {
            margin-bottom: 16px;
        }

        .card-content h3 {
            font-size: 20px;
        }

        .card-footer {
            flex-wrap: wrap;
        }

        .open-btn {
            margin-left: 0;
        }
    }
</style>