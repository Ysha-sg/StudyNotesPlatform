﻿
<template>
    <div class="note-details-page">
        <!-- Шапка -->
        <div class="header">
            <div class="header-left">
                <div class="back-button" @click="goBack">
                    <svg width="49" height="49" viewBox="0 0 49 49" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <circle cx="24.5" cy="24.5" r="23" stroke="#A0A0B0" stroke-width="2.5" />
                        <polyline points="27 17 18 24.5 27 32" stroke="#A0A0B0" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" />
                    </svg>
                </div>
                <div class="logo-area" @click="goToCatalog">
                    <div class="logo-icon">
                        <svg width="55" height="50" viewBox="0 0 69 69" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M34.5 20.125C34.5 17.075 33.2884 14.1499 31.1317 11.9933C28.9751 9.8366 26.05 8.625 23 8.625H5.75V51.75H25.875C28.1625 51.75 30.3563 52.6587 31.9738 54.2762C33.5913 55.8937 34.5 58.0875 34.5 60.375M34.5 20.125V60.375M34.5 20.125C34.5 17.075 35.7116 14.1499 37.8683 11.9933C40.0249 9.8366 42.95 8.625 46 8.625H63.25V51.75H43.125C40.8375 51.75 38.6437 52.6587 37.0262 54.2762C35.4087 55.8937 34.5 58.0875 34.5 60.375" stroke="#8B7FFF" stroke-width="4" stroke-linecap="round" stroke-linejoin="round" />
                        </svg>
                    </div>
                    <span class="logo-text">Каталог конспектов</span>
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
                        <div class="menu-item" @click="goToProfile">Профиль</div>
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

        <!-- Контент с выравниванием -->
        <div class="content-wrapper">
            <!-- Заголовок и мета-информация -->
            <div class="note-header" v-if="note.id">
                <h1 class="note-title">{{ note.title }}</h1>
                <div class="note-meta">
                    <div class="meta-item">
                        <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M12 7C12 5.93913 11.5786 4.92172 10.8284 4.17157C10.0783 3.42143 9.06087 3 8 3H2V18H9C9.79565 18 10.5587 18.3161 11.1213 18.8787C11.6839 19.4413 12 20.2044 12 21M12 7V21M12 7C12 5.93913 12.4214 4.92172 13.1716 4.17157C13.9217 3.42143 14.9391 3 16 3H22V18H15C14.2044 18 13.4413 18.3161 12.8787 18.8787C12.3161 19.4413 12 20.2044 12 21" stroke="#6C63FF" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round" />
                        </svg>
                        <span>{{ note.subject }}</span>
                    </div>
                    <div class="meta-item">
                        <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <g clip-path="url(#clip0_62_174)">
                                <path d="M24 9.41999C24.0005 9.09417 23.9147 8.77403 23.7514 8.49212C23.588 8.21021 23.353 7.97658 23.07 7.81499L12.96 1.81499C12.6687 1.645 12.3374 1.55542 12 1.55542C11.6627 1.55542 11.3314 1.645 11.04 1.81499L0.915025 7.81499C0.635233 7.98101 0.403453 8.21698 0.242466 8.49969C0.0814795 8.78241 -0.00317383 9.10215 -0.00317383 9.42749C-0.00317383 9.75283 0.0814795 10.0726 0.242466 10.3553C0.403453 10.638 0.635233 10.874 0.915025 11.04L3.79502 12.72V16.935C3.79676 17.2644 3.88322 17.5878 4.04609 17.8742C4.20896 18.1605 4.44277 18.4001 4.72503 18.57L11.1 22.245C11.3847 22.4026 11.7047 22.4853 12.03 22.4853C12.3554 22.4853 12.6754 22.4026 12.96 22.245L19.335 18.57C19.6173 18.4001 19.8511 18.1605 20.014 17.8742C20.1768 17.5878 20.2633 17.2644 20.265 16.935V12.675L22.125 11.58V15.66H24V9.41999ZM18.405 16.92L12 20.61L5.67002 16.935V13.83L11.04 17.025C11.3324 17.1921 11.6633 17.2799 12 17.2799C12.3368 17.2799 12.6677 17.1921 12.96 17.025L18.39 13.785L18.405 16.92ZM12 15.405L1.87502 9.40499L12 3.38999L22.125 9.38999L12 15.405Z" fill="#6C63FF" />
                            </g>
                            <defs>
                                <clipPath id="clip0_62_174">
                                    <rect width="24" height="24" fill="white" />
                                </clipPath>
                            </defs>
                        </svg>
                        <span>{{ note.university }}</span>
                    </div>
                    <div class="meta-item">
                        <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M20 21V19C20 17.9391 19.5786 16.9217 18.8284 16.1716C18.0783 15.4214 17.0609 15 16 15H8C6.93913 15 5.92172 15.4214 5.17157 16.1716C4.42143 16.9217 4 17.9391 4 19V21M16 7C16 9.20914 14.2091 11 12 11C9.79086 11 8 9.20914 8 7C8 4.79086 9.79086 3 12 3C14.2091 3 16 4.79086 16 7Z" stroke="#6C63FF" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round" />
                        </svg>
                        <span>{{ note.teacher }}</span>
                    </div>
                    <div class="meta-item">
                        <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M16 2V6M8 2V6M3 10H21M5 4H19C20.1046 4 21 4.89543 21 6V20C21 21.1046 20.1046 22 19 22H5C3.89543 22 3 21.1046 3 20V6C3 4.89543 3.89543 4 5 4Z" stroke="#6C63FF" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
                        </svg>
                        <span>{{ formatDate(note.uploadedAt) }}</span>
                    </div>
                </div>
            </div>

            <!-- Основной контент -->
            <div class="content" v-if="note.id">
                <!-- Левая колонка -->
                <div class="main-column">
                    <!-- Описание -->
                    <div class="description-block">
                        <h3>Описание</h3>
                        <p>{{ note.description }}</p>
                    </div>

                    <!-- Файл конспекта -->
                    <div class="file-block">
                        <h3>Файл конспекта</h3>
                        <div class="file-card">
                            <div class="file-icon-large"></div>
                            <div class="file-info">
                                <div class="file-name">{{ getFileName(note.filePath) }}</div>
                                <div class="file-type">PDF файл</div>
                                <div class="file-buttons">
                                    <button class="view-file-btn" @click="viewFile">
                                        <svg width="24" height="24" viewBox="0 0 32 32" fill="none" xmlns="http://www.w3.org/2000/svg">
                                            <path d="M23.5 17.5C27.642 17.5 31 21.5 31 23.5C31 25.5 27.642 29.5 23.5 29.5C19.358 29.5 16 25.5 16 23.5C16 21.5 19.358 17.5 23.5 17.5ZM24.5 3C25.6935 3 26.8381 3.47411 27.682 4.31802C28.5259 5.16193 29 6.30653 29 7.5V17.964C28.3804 17.4931 27.7095 17.0938 27 16.774V11H5V24.5C5 25.163 5.26339 25.7989 5.73223 26.2678C6.20107 26.7366 6.83696 27 7.5 27H15.942C16.5209 27.753 17.1975 28.4256 17.954 29H7.5C6.30653 29 5.16193 28.5259 4.31802 27.682C3.47411 26.8381 3 25.6935 3 24.5V7.5C3 6.30653 3.47411 5.16193 4.31802 4.31802C5.16193 3.47411 6.30653 3 7.5 3H24.5ZM23.5 19.5C22.4391 19.5 21.4217 19.9214 20.6716 20.6716C19.9214 21.4217 19.5 22.4391 19.5 23.5C19.5 24.5609 19.9214 25.5783 20.6716 26.3284C21.4217 27.0786 22.4391 27.5 23.5 27.5C24.5609 27.5 25.5783 27.0786 26.3284 26.3284C27.0786 25.5783 27.5 24.5609 27.5 23.5C27.5 22.4391 27.0786 21.4217 26.3284 20.6716C25.5783 19.9214 24.5609 19.5 23.5 19.5ZM23.5 21C24.163 21 24.7989 21.2634 25.2678 21.7322C25.7366 22.2011 26 22.837 26 23.5C26 24.163 25.7366 24.7989 25.2678 25.2678C24.7989 25.7366 24.163 26 23.5 26C22.837 26 22.2011 25.7366 21.7322 25.2678C21.2634 24.7989 21 24.163 21 23.5C21 22.837 21.2634 22.2011 21.7322 21.7322C22.2011 21.2634 22.837 21 23.5 21ZM7.5 5C6.83696 5 6.20107 5.26339 5.73223 5.73223C5.26339 6.20107 5 6.83696 5 7.5V9H27V7.5C27 6.83696 26.7366 6.20107 26.2678 5.73223C25.7989 5.26339 25.163 5 24.5 5H7.5Z" fill="white" />
                                        </svg>
                                        Посмотреть файл
                                    </button>
                                    <button class="download-file-btn" @click="downloadFile">
                                        <svg width="32" height="32" viewBox="0 0 32 32" fill="none" xmlns="http://www.w3.org/2000/svg">
                                            <path d="M15.9999 21.3333L9.33325 14.6666L11.1999 12.7333L14.6666 16.2V5.33331H17.3333V16.2L20.7999 12.7333L22.6666 14.6666L15.9999 21.3333ZM7.99992 26.6666C7.26659 26.6666 6.63881 26.4055 6.11659 25.8833C5.59436 25.3611 5.33325 24.7333 5.33325 24V20H7.99992V24H23.9999V20H26.6666V24C26.6666 24.7333 26.4055 25.3611 25.8833 25.8833C25.361 26.4055 24.7333 26.6666 23.9999 26.6666H7.99992Z" fill="white" />
                                        </svg>
                                        Скачать конспект
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Правая колонка -->
                <div class="side-column">
                    <!-- Рейтинг -->
                    <div class="rating-block">
                        <div class="rating-header">
                            <h3>Рейтинг</h3>
                        </div>
                        <div class="rating-value">{{ note.rating }}</div>
                        <div class="rating-stars-filled">
                            <span v-for="star in 10" :key="'filled-' + star" class="star-filled" :class="{ active: star <= note.rating }"></span>
                        </div>
                        <div class="downloads-count">{{ note.downloadsCount }} скачиваний</div>
                        <div class="rating-line"></div>
                        <div class="rating-text">Оцените конспект</div>
                        <div class="rating-stars-empty">
                            <span v-for="star in 10" :key="'empty-' + star" class="star-empty" :class="{ active: star <= tempRating }" @click="rateNote(star)"></span>
                        </div>
                    </div>

                    <!-- Пожаловаться и Сообщить о проблеме -->
                    <div class="complaint-block">
                        <div class="complaint-label">
                            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                <path d="M7 14V20C7 20.2833 6.904 20.521 6.712 20.713C6.52 20.905 6.28267 21.0007 6 21C5.71733 20.9993 5.48 20.9033 5.288 20.712C5.096 20.5207 5 20.2833 5 20V5C5 4.71667 5.096 4.47933 5.288 4.288C5.48 4.09667 5.71733 4.00067 6 4H13.175C13.4083 4 13.6167 4.075 13.8 4.225C13.9833 4.375 14.1 4.56667 14.15 4.8L14.4 6H19C19.2833 6 19.521 6.096 19.713 6.288C19.905 6.48 20.0007 6.71733 20 7V15C20 15.2833 19.904 15.521 19.712 15.713C19.52 15.905 19.2827 16.0007 19 16H13.825C13.5917 16 13.3833 15.925 13.2 15.775C13.0167 15.625 12.9 15.4333 12.85 15.2L12.6 14H7Z" fill="white" />
                            </svg>
                            <span>Пожаловаться</span>
                        </div>
                        <button class="report-btn" @click="openComplaintModal">Сообщить о проблеме</button>
                    </div>
                </div>
            </div>

            <div v-else class="loading">
                <p>Загрузка...</p>
            </div>
        </div>

        <!-- Модальное окно жалобы -->
        <div v-if="showComplaintModal" class="modal-overlay" @click.self="closeComplaintModal">
            <div class="modal-content">
                <div class="modal-header">
                    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M7 14V20C7 20.2833 6.904 20.521 6.712 20.713C6.52 20.905 6.28267 21.0007 6 21C5.71733 20.9993 5.48 20.9033 5.288 20.712C5.096 20.5207 5 20.2833 5 20V5C5 4.71667 5.096 4.47933 5.288 4.288C5.48 4.09667 5.71733 4.00067 6 4H13.175C13.4083 4 13.6167 4.075 13.8 4.225C13.9833 4.375 14.1 4.56667 14.15 4.8L14.4 6H19C19.2833 6 19.521 6.096 19.713 6.288C19.905 6.48 20.0007 6.71733 20 7V15C20 15.2833 19.904 15.521 19.712 15.713C19.52 15.905 19.2827 16.0007 19 16H13.825C13.5917 16 13.3833 15.925 13.2 15.775C13.0167 15.625 12.9 15.4333 12.85 15.2L12.6 14H7Z" fill="#6C63FF" />
                    </svg>
                    <h2>Пожаловаться на конспект</h2>
                    <div class="flag-bg"></div>
                </div>
                <p class="modal-description">Укажите причину жалобы. Модераторы рассмотрят ваше обращение в ближайшее время.</p>

                <div class="form-group">
                    <label>Причина жалобы*</label>
                    <div class="custom-select" :class="{ open: isReasonOpen }">
                        <div class="select-trigger" @click="toggleReasonDropdown">
                            <span>{{ selectedReason || 'Выберите причину' }}</span>
                            <span class="chevron-down"></span>
                        </div>
                        <div v-if="isReasonOpen" class="select-dropdown">
                            <div v-for="reason in complaintReasons" :key="reason" class="select-option" @click="selectReason(reason)">
                                {{ reason }}
                            </div>
                        </div>
                    </div>
                </div>

                <div class="form-group">
                    <label>Комментарий (необязательно)</label>
                    <textarea v-model="complaintComment" placeholder="Опишите проблему подробнее..."></textarea>
                </div>

                <div class="modal-buttons">
                    <button class="cancel-btn" @click="closeComplaintModal">Отмена</button>
                    <button class="submit-btn" @click="submitComplaint">Отправить жалобу</button>
                </div>
            </div>
        </div>

        <!-- Уведомление об успешной отправке -->
        <div v-if="showNotification" class="notification">
            <div class="notification-content">
                <div class="check-icon"></div>
                <span>Жалоба отправлена</span>
                <div class="close-icon" @click="showNotification = false"></div>
            </div>
        </div>
    </div>
</template>
<script setup>
    import { computed, ref, onMounted, onUnmounted } from 'vue'
    import { useRouter, useRoute } from 'vue-router'
    import { useAuthStore } from '@/stores/auth'
    import api from '@/services/api'

    const router = useRouter()
    const route = useRoute()
    const authStore = useAuthStore()

    const isMenuOpen = ref(false)

    const note = ref({
        id: null,
        title: '',
        subject: '',
        university: '',
        teacher: '',
        uploadedAt: '',
        description: '',
        filePath: '',
        rating: 0,
        userRating: 0,
        downloadsCount: 0
    })
    const maxRating = 10
    const hoverRating = ref(0)
    const isRatingSaving = ref(false)

    const safeRating = computed(() => {
        const numeric = Number(note.value.rating)
        if (!Number.isFinite(numeric)) return 0
        return Math.max(0, Math.min(maxRating, numeric))
    })

    const safeUserRating = computed(() => {
        const numeric = Number(note.value.userRating)
        if (!Number.isFinite(numeric)) return 0
        return Math.max(0, Math.min(maxRating, numeric))
    })

    const displayRating = computed(() => safeRating.value.toFixed(1))

    const showComplaintModal = ref(false)
    const isReasonOpen = ref(false)
    const selectedReason = ref('')
    const complaintComment = ref('')
    const complaintReasons = ['Нарушение правил', 'Не соответствует описанию', 'Неправильный предмет', 'Плохое качество']
    const showNotification = ref(false)
    let notificationTimeout = null

    // Сохранение в историю просмотров (localStorage)
    const saveToHistory = (noteData) => {
        const history = JSON.parse(localStorage.getItem('downloadHistory') || '[]')
        const existingIndex = history.findIndex(item => item.id === noteData.id)
        if (existingIndex !== -1) {
            history.splice(existingIndex, 1)
        }
        history.unshift({
            id: noteData.id,
            title: noteData.title,
            subject: noteData.subject,
            teacher: noteData.teacher,
            university: noteData.university,
            rating: noteData.rating,
            downloadsCount: noteData.downloadsCount,
            downloadedAt: new Date().toISOString()
        })
        if (history.length > 20) history.pop()
        localStorage.setItem('downloadHistory', JSON.stringify(history))
    }

    const loadNote = async () => {
        try {
            const id = route.params.id
            const response = await api.get(`/notes/${id}`)
            note.value = {
                ...response.data,
                userRating: Number(response.data?.userRating ?? 0)
            }
            saveToHistory(note.value)
        } catch (error) {
            console.error('Ошибка загрузки конспекта:', error)
        }
    }

    const toggleMenu = () => {
        isMenuOpen.value = !isMenuOpen.value
    }

    const closeMenu = () => {
        isMenuOpen.value = false
    }

    const goToCatalog = () => {
        router.push('/')
    }

    const goBack = () => {
        router.back()
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

    const formatDate = (dateString) => {
        if (!dateString) return ''
        const date = new Date(dateString)
        return date.toLocaleDateString('ru-RU', { day: 'numeric', month: 'long', year: 'numeric' })
    }

    const getFileName = (filePath) => {
        if (!filePath) return ''
        return filePath.split('/').pop()
    }

    const getDownloadFileName = (fallbackName, contentDisposition) => {
        if (!contentDisposition) return fallbackName

        const utf8Match = contentDisposition.match(/filename\*=UTF-8''([^;]+)/i)
        if (utf8Match?.[1]) {
            return decodeURIComponent(utf8Match[1])
        }

        const fallbackMatch = contentDisposition.match(/filename="?([^";]+)"?/i)
        return fallbackMatch?.[1] || fallbackName
    }

    const redirectToLogin = () => {
        router.push({
            path: '/login',
            query: {
                redirect: route.fullPath
            }
        })
    }

    const viewFile = async () => {
        if (!note.value.id) return
        if (!authStore.isAuthenticated) {
            redirectToLogin()
            return
        }

        const previewWindow = window.open('', '_blank')
        if (!previewWindow) {
            alert('Разрешите всплывающие окна в браузере, чтобы открыть файл')
            return
        }

        try {
            const response = await api.get(`/notes/${note.value.id}/file`, {
                responseType: 'blob'
            })

            const blobUrl = URL.createObjectURL(response.data)
            previewWindow.location.href = blobUrl

            setTimeout(() => {
                URL.revokeObjectURL(blobUrl)
            }, 60_000)
        } catch (error) {
            previewWindow.close()
            if (error.response?.status === 401 || error.response?.status === 403) {
                redirectToLogin()
                return
            }
            console.error('Ошибка просмотра файла:', error)
            alert('Не удалось открыть файл')
        }
    }

    const downloadFile = async () => {
        if (!note.value.id) return
        if (!authStore.isAuthenticated) {
            redirectToLogin()
            return
        }

        try {
            const response = await api.get(`/notes/${note.value.id}/download`, {
                responseType: 'blob'
            })

            const fallbackName = getFileName(note.value.filePath) || `note-${note.value.id}.pdf`
            const downloadName = getDownloadFileName(
                fallbackName,
                response.headers['content-disposition']
            )

            const url = URL.createObjectURL(response.data)
            const link = document.createElement('a')
            link.href = url
            link.download = downloadName
            document.body.appendChild(link)
            link.click()
            document.body.removeChild(link)
            URL.revokeObjectURL(url)
            note.value.downloadsCount += 1
        } catch (error) {
            if (error.response?.status === 401 || error.response?.status === 403) {
                redirectToLogin()
                return
            }
            console.error('Ошибка скачивания файла:', error)
            alert('Не удалось скачать файл')
        }
    }

    const rateNote = async (rating) => {
        try {
            if (!note.value.id) return
            if (!authStore.isAuthenticated) {
                redirectToLogin()
                return
            }

            isRatingSaving.value = true
            const normalized = Math.max(1, Math.min(maxRating, Number(rating) || 1))
            const response = await api.post(`/notes/${note.value.id}/rate`, { rating: normalized })
            note.value.rating = Number(response.data?.rating ?? normalized)
            note.value.userRating = Number(response.data?.userRating ?? normalized)
            await loadNote()
            hoverRating.value = 0
        } catch (error) {
            console.error('Ошибка при оценке:', error)
            const message =
                error.response?.data?.title ||
                error.response?.data?.detail ||
                'Не удалось поставить оценку'
            alert(message)
        } finally {
            isRatingSaving.value = false
        }
    }

    const setHoverRating = (rating) => {
        hoverRating.value = Math.max(0, Math.min(maxRating, Number(rating) || 0))
    }

    const isDisplayStarFilled = (star) => {
        return star <= Math.round(safeRating.value)
    }

    const isInputStarFilled = (star) => {
        const activeRating = hoverRating.value > 0 ? hoverRating.value : Math.round(safeUserRating.value)
        return star <= activeRating
    }

    const openComplaintModal = () => {
        showComplaintModal.value = true
        selectedReason.value = ''
        complaintComment.value = ''
    }

    const closeComplaintModal = () => {
        showComplaintModal.value = false
        isReasonOpen.value = false
    }

    const toggleReasonDropdown = () => {
        isReasonOpen.value = !isReasonOpen.value
    }

    const selectReason = (reason) => {
        selectedReason.value = reason
        isReasonOpen.value = false
    }

    const submitComplaint = () => {
        if (!selectedReason.value) {
            alert('Пожалуйста, выберите причину жалобы')
            return
        }

        console.log('Жалоба:', {
            noteId: note.value.id,
            reason: selectedReason.value,
            comment: complaintComment.value
        })

        closeComplaintModal()
        showNotification.value = true

        if (notificationTimeout) clearTimeout(notificationTimeout)
        notificationTimeout = setTimeout(() => {
            showNotification.value = false
        }, 3000)
    }

    const handleClickOutside = (event) => {
        if (!event.target.closest('.avatar-menu')) {
            isMenuOpen.value = false
        }
        if (!event.target.closest('.custom-select')) {
            isReasonOpen.value = false
        }
    }

    onMounted(() => {
        document.addEventListener('click', handleClickOutside)
        loadNote()
    })

    onUnmounted(() => {
        document.removeEventListener('click', handleClickOutside)
        if (notificationTimeout) clearTimeout(notificationTimeout)
    })
</script>

<style scoped>
    .note-details-page {
        min-height: 100vh;
        background: #0F0F14;
        font-family: 'Inter', system-ui, -apple-system, sans-serif;
        display: flex;
        flex-direction: column;
        align-items: center;
    }

    /* Шапка */
    .header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        width: 100%;
        max-width: 1440px;
        margin: 0 auto;
        padding: 56px 80px 0 80px; /* Изменено: верхний отступ 56px, убран нижний margin */
        margin-bottom: 0; /* Убран margin-bottom */
    }

    .header-left {
        display: flex;
        align-items: center;
        gap: 20px;
    }

    .back-button {
        cursor: pointer;
        width: 49px;
        height: 49px;
    }

    .logo-area {
        display: flex;
        align-items: center;
        gap: 12px;
        cursor: pointer;
    }

    .logo-icon {
        width: 55px;
        height: 50px;
        background: #2A2348;
        border-radius: 10px;
        display: flex;
        align-items: center;
        justify-content: center;
    }

        .logo-icon svg {
            width: 45px;
            height: 40px;
        }

    .logo-text {
        font-size: 24px;
        font-weight: 700;
        color: #FFFFFF;
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

    .content-wrapper {
        display: flex;
        flex-direction: column;
        width: 100%;
        max-width: 1440px;
        margin: 0 auto;
        margin-top: 28px; /* Добавлен отступ сверху для контента (133px - 56px шапки - 49px высота кнопки назад = 28px) */
    }

    /* Заголовок и мета-информация */
    .note-header {
        width: 800px;
        margin-left: 187px;
        margin-bottom: 32px;
    }

    .note-title {
        font-size: 36px;
        font-weight: 700;
        color: #FFFFFF;
        margin-bottom: 16px;
        text-align: left;
    }

    .note-meta {
        display: flex;
        flex-wrap: wrap;
        gap: 24px;
        margin-bottom: 32px;
    }

    .meta-item {
        display: flex;
        align-items: center;
        gap: 8px;
        font-size: 20px;
        font-weight: 500;
        color: #A0A0B0;
    }

    /* Основные колонки */
    .content {
        display: flex;
        justify-content: flex-start;
        gap: 0;
        width: 100%;
    }

    /* Левая колонка (800px, отступ слева 187px) */
    .main-column {
        width: 800px;
        margin-left: 187px;
    }

    /* Правая колонка (339px, отступ от левой колонки 33px) */
    .side-column {
        width: 339px;
        margin-left: 33px;
    }

    .loading {
        text-align: center;
        padding: 100px;
        color: #FFFFFF;
        font-size: 24px;
    }

    /* Описание */
    .description-block {
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 16px;
        padding: 24px;
        margin-bottom: 32px;
    }

        .description-block h3 {
            font-size: 20px;
            font-weight: 700;
            color: #FFFFFF;
            margin-bottom: 16px;
        }

        .description-block p {
            font-size: 16px;
            line-height: 1.6;
            color: #DFDFEA;
        }

    /* Файл конспекта */
    .file-block h3 {
        font-size: 20px;
        font-weight: 700;
        color: #FFFFFF;
        margin-bottom: 16px;
    }

    .file-card {
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 16px;
        padding: 24px;
        display: flex;
        align-items: flex-start;
        gap: 24px;
        flex-wrap: wrap;
    }

    .file-icon-large {
        width: 62px;
        height: 66px;
        background: #2A2348;
        border-radius: 10px;
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%236C63FF" stroke-width="2"%3E%3Cpath d="M13 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9z"/%3E%3Cpolyline points="13 2 13 9 20 9"/%3E%3C/svg%3E');
        background-repeat: no-repeat;
        background-position: center;
        background-size: 45px 50px;
    }

    .file-info {
        flex: 1;
    }

    .file-name {
        font-size: 20px;
        font-weight: 500;
        color: #FFFFFF;
        margin-bottom: 8px;
    }

    .file-type {
        font-size: 16px;
        font-weight: 500;
        color: #A0A0B0;
        margin-bottom: 16px;
    }

    .file-buttons {
        display: flex;
        flex-direction: column;
        gap: 16px;
        width: 100%;
        margin-top: 8px;
    }

    .view-file-btn, .download-file-btn {
        width: 100%;
        height: 56px;
        padding: 0;
        border-radius: 14px;
        font-size: 20px;
        font-weight: 700;
        cursor: pointer;
        transition: all 0.2s;
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 12px;
        background: #6C63FF;
        border: none;
        color: #FFFFFF;
    }

        .view-file-btn:hover, .download-file-btn:hover {
            background: #594FFE;
            transform: translateY(-2px);
        }

        .view-file-btn svg, .download-file-btn svg {
            width: 24px;
            height: 24px;
        }

    /* Рейтинг */
    .rating-block {
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 16px;
        padding: 24px;
        margin-bottom: 24px;
        text-align: center;
    }

    .rating-header h3 {
        font-size: 20px;
        font-weight: 700;
        color: #FFFFFF;
        text-align: left;
        margin-bottom: 16px;
    }

    .rating-value {
        font-size: 48px;
        font-weight: 700;
        color: #FFFFFF;
        text-align: center;
        margin-bottom: 12px;
    }

    .rating-stars-filled {
        display: flex;
        justify-content: center;
        gap: 8px;
        margin-bottom: 12px;
    }

    .star-filled {
        width: 17px;
        height: 17px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23686D88" stroke-width="1.5"%3E%3Cpolygon points="12 2 15 9 22 9 16 14 18 21 12 17 6 21 8 14 2 9 9 9 12 2"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

        .star-filled.active {
            background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="%23FFE100"%3E%3Cpolygon points="12 2 15 9 22 9 16 14 18 21 12 17 6 21 8 14 2 9 9 9 12 2"/%3E%3C/svg%3E') no-repeat center;
            background-size: contain;
        }

    .downloads-count {
        font-size: 16px;
        color: #A0A0B0;
        text-align: center;
        margin-bottom: 16px;
    }

    .rating-line {
        height: 1px;
        background: #2A2A35;
        margin: 16px 0;
    }

    .rating-text {
        font-size: 16px;
        color: #A0A0B0;
        text-align: center;
        margin-bottom: 16px;
    }

    .rating-stars-empty {
        display: flex;
        justify-content: center;
        gap: 8px;
    }

    .star-empty {
        width: 24px;
        height: 24px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23686D88" stroke-width="1.5"%3E%3Cpolygon points="12 2 15 9 22 9 16 14 18 21 12 17 6 21 8 14 2 9 9 9 12 2"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
        cursor: pointer;
        transition: all 0.1s;
    }

        .star-empty.active {
            background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="%23FFE100"%3E%3Cpolygon points="12 2 15 9 22 9 16 14 18 21 12 17 6 21 8 14 2 9 9 9 12 2"/%3E%3C/svg%3E') no-repeat center;
            background-size: contain;
        }

        .star-empty:hover {
            transform: scale(1.1);
        }

    /* Пожаловаться */
    .complaint-block {
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 16px;
        padding: 24px;
        display: flex;
        flex-direction: column;
        gap: 16px;
        align-items: flex-start;
    }

    .complaint-label {
        display: flex;
        align-items: center;
        justify-content: flex-start;
        gap: 8px;
        cursor: default;
    }

        .complaint-label span {
            font-size: 20px;
            font-weight: 500;
            color: #FFFFFF;
        }

    .report-btn {
        background: #6C63FF;
        border: none;
        border-radius: 14px;
        padding: 14px;
        font-size: 20px;
        font-weight: 700;
        color: #FFFFFF;
        cursor: pointer;
        transition: all 0.2s;
        text-align: center;
        width: 100%;
    }

        .report-btn:hover {
            background: #594FFE;
            transform: translateY(-2px);
        }

    /* Модальное окно */
    .modal-overlay {
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(0, 0, 0, 0.5);
        backdrop-filter: blur(10px);
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 200;
    }

    .modal-content {
        width: 665px;
        max-width: 90%;
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 20px;
        padding: 32px;
    }

    .modal-header {
        display: flex;
        align-items: center;
        gap: 16px;
        margin-bottom: 16px;
        position: relative;
    }

    .flag-bg {
        position: absolute;
        width: 46px;
        height: 46px;
        left: -3px;
        top: -3px;
        background: rgba(108, 99, 255, 0.2);
        border-radius: 12px;
    }

    .modal-header h2 {
        font-size: 32px;
        font-weight: 600;
        color: #FFFFFF;
    }

    .modal-description {
        font-size: 18px;
        color: #A0A0B0;
        margin-bottom: 32px;
    }

    .modal-content .form-group {
        margin-bottom: 24px;
    }

        .modal-content .form-group label {
            display: block;
            font-size: 20px;
            font-weight: 400;
            color: #FFFFFF;
            margin-bottom: 12px;
        }

    .custom-select {
        position: relative;
    }

    .select-trigger {
        width: 100%;
        padding: 16px 20px;
        background: #0F0F14;
        border: 1px solid #2A2A35;
        border-radius: 14px;
        display: flex;
        justify-content: space-between;
        align-items: center;
        cursor: pointer;
        color: #FFFFFF;
        font-size: 20px;
    }

    .chevron-down {
        width: 24px;
        height: 24px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%237F8499" stroke-width="3"%3E%3Cpolyline points="6 9 12 15 18 9"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
        transition: transform 0.2s;
    }

    .custom-select.open .chevron-down {
        transform: rotate(180deg);
    }

    .select-dropdown {
        position: absolute;
        top: 100%;
        left: 0;
        right: 0;
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 12px;
        margin-top: 8px;
        z-index: 100;
        max-height: 200px;
        overflow-y: auto;
    }

    .select-option {
        padding: 12px 20px;
        font-size: 20px;
        color: #FFFFFF;
        cursor: pointer;
        transition: background 0.2s;
    }

        .select-option:hover {
            background: #2A2A35;
        }

    textarea {
        width: 100%;
        padding: 16px 20px;
        background: #0F0F14;
        border: 1px solid #2A2A35;
        border-radius: 14px;
        color: #FFFFFF;
        font-size: 20px;
        font-family: 'Inter', sans-serif;
        resize: vertical;
        min-height: 154px;
    }

        textarea::placeholder {
            color: #A0A0B0;
        }

        textarea:focus {
            outline: none;
            border-color: #6C63FF;
        }

    .modal-buttons {
        display: flex;
        gap: 16px;
        justify-content: flex-end;
        margin-top: 24px;
    }

    .cancel-btn, .submit-btn {
        padding: 14px 28px;
        border-radius: 14px;
        font-size: 24px;
        font-weight: 600;
        cursor: pointer;
        transition: all 0.2s;
    }

    .cancel-btn {
        background: #0F0F14;
        border: 1px solid #151516;
        color: #FFFFFF;
    }

    .submit-btn {
        background: #594FFE;
        border: none;
        color: #FFFFFF;
    }

        .submit-btn:hover {
            background: #6C63FF;
            box-shadow: 0px 8px 16px rgba(108, 99, 255, 0.45);
            transform: translateY(-2px);
        }

    /* Уведомление */
    .notification {
        position: fixed;
        bottom: 30px;
        right: 30px;
        z-index: 300;
        animation: slideIn 0.3s ease;
    }

    .notification-content {
        display: flex;
        align-items: center;
        gap: 12px;
        background: #1A1A22;
        box-shadow: 0px 0px 20px rgba(108, 99, 255, 0.45);
        border-radius: 12px;
        padding: 16px 24px;
    }

    .check-icon {
        width: 24px;
        height: 24px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="%2322C55E"%3E%3Cpath d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .notification-content span {
        font-size: 20px;
        font-weight: 500;
        color: #FFFFFF;
    }

    .notification-content .close-icon {
        width: 24px;
        height: 24px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23EF4444" stroke-width="2"%3E%3Cline x1="18" y1="6" x2="6" y2="18"/%3E%3Cline x1="6" y1="6" x2="18" y2="18"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
        cursor: pointer;
    }

    @keyframes slideIn {
        from {
            transform: translateX(100%);
            opacity: 0;
        }

        to {
            transform: translateX(0);
            opacity: 1;
        }
    }

    @media (max-width: 1100px) {
        .header {
            padding: 0 40px;
        }

        .note-header,
        .main-column {
            width: 100%;
            margin-left: 0;
            padding: 0 40px;
        }

        .side-column {
            width: 100%;
            margin-left: 0;
            padding: 0 40px;
        }

        .content {
            flex-direction: column;
            align-items: center;
        }

        .file-card {
            flex-direction: column;
            align-items: center;
            text-align: center;
        }

        .file-buttons {
            align-items: center;
        }

        .modal-content {
            width: 90%;
            padding: 24px;
        }
    }

    @media (max-width: 768px) {
        .note-details-page {
            padding: 16px;
        }

        .header {
            padding: 0 16px;
        }

        .note-header,
        .main-column {
            padding: 0 16px;
        }

        .side-column {
            padding: 0 16px;
        }

        .note-title {
            font-size: 28px;
        }

        .note-meta {
            flex-direction: column;
            gap: 12px;
        }

        .meta-item {
            font-size: 16px;
        }

        .rating-header h3 {
            font-size: 18px;
        }

        .rating-value {
            font-size: 36px;
        }

        .star-filled,
        .star-empty {
            width: 20px;
            height: 20px;
        }

        .modal-header h2 {
            font-size: 24px;
        }

        .modal-buttons {
            flex-direction: column;
        }

        .cancel-btn, .submit-btn {
            text-align: center;
        }
    }
</style>