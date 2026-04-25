<template>
    <div class="note-details-page">
        <!-- Шапка -->
        <div class="header">
            <div class="logo-area" @click="goToCatalog">
                <div class="logo-icon">
                    <svg width="55" height="50" viewBox="0 0 69 69" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M34.5 20.125C34.5 17.075 33.2884 14.1499 31.1317 11.9933C28.9751 9.8366 26.05 8.625 23 8.625H5.75V51.75H25.875C28.1625 51.75 30.3563 52.6587 31.9738 54.2762C33.5913 55.8937 34.5 58.0875 34.5 60.375M34.5 20.125V60.375M34.5 20.125C34.5 17.075 35.7116 14.1499 37.8683 11.9933C40.0249 9.8366 42.95 8.625 46 8.625H63.25V51.75H43.125C40.8375 51.75 38.6437 52.6587 37.0262 54.2762C35.4087 55.8937 34.5 58.0875 34.5 60.375" stroke="#6C63FF" stroke-width="4" stroke-linecap="round" stroke-linejoin="round" />
                    </svg>
                </div>
                <span class="logo-text">Каталог конспектов</span>
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

        <!-- Стрелка назад -->
        <div class="back-button" @click="goBack">
            <svg width="49" height="49" viewBox="0 0 49 49" fill="none" xmlns="http://www.w3.org/2000/svg">
                <circle cx="24.5" cy="24.5" r="23" stroke="#A0A0B0" stroke-width="2.5" />
                <polyline points="27 17 18 24.5 27 32" stroke="#A0A0B0" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" />
            </svg>
        </div>

        <!-- Основной контент -->
        <div class="content" v-if="note.id">
            <div class="main-column">
                <h1 class="note-title">{{ note.title }}</h1>

                <div class="note-meta">
                    <div class="meta-item">
                        <div class="book-icon-small"></div>
                        <span>{{ note.subject }}</span>
                    </div>
                    <div class="meta-item">
                        <div class="uni-icon-small"></div>
                        <span>{{ note.university }}</span>
                    </div>
                    <div class="meta-item">
                        <div class="user-icon-small"></div>
                        <span>{{ note.teacher }}</span>
                    </div>
                    <div class="meta-item">
                        <div class="calendar-icon-small"></div>
                        <span>{{ formatDate(note.uploadedAt) }}</span>
                    </div>
                </div>

                <div class="description-block">
                    <h3>Описание</h3>
                    <p>{{ note.description }}</p>
                </div>

                <div class="file-block">
                    <h3>Файл конспекта</h3>
                    <div class="file-card">
                        <div class="file-icon-large"></div>
                        <div class="file-info">
                            <div class="file-name">{{ getFileName(note.filePath) }}</div>
                            <div class="file-type">PDF файл</div>
                        </div>
                        <button class="view-file-btn"
                                @click="viewFile">
                            Посмотреть файл
                        </button>
                        <button class="download-file-btn"
                                @click="downloadFile">
                            Скачать конспект
                        </button>
                        <p v-if="!authStore.isAuthenticated" class="file-auth-hint">
                            Для просмотра и скачивания войдите в аккаунт
                        </p>
                    </div>
                </div>
            </div>

            <div class="side-column">
                <div class="rating-block">
                    <div class="rating-header">
                        <h3>Рейтинг</h3>
                    </div>
                    <div class="rating-value">{{ displayRating }}</div>
                    <div class="rating-stars-static">
                        <span v-for="star in maxRating"
                              :key="star"
                              class="star"
                              :class="{ filled: isDisplayStarFilled(star) }"></span>
                    </div>
                    <div class="downloads-count">{{ note.downloadsCount }} скачиваний</div>
                    <div class="rating-line"></div>
                    <p class="rating-text">Оцените конспект</p>
                    <div class="rating-stars-input" @mouseleave="setHoverRating(0)">
                        <button v-for="star in maxRating"
                                :key="`input-${star}`"
                                type="button"
                                class="star-button"
                                :disabled="isRatingSaving"
                                @mouseenter="setHoverRating(star)"
                                @focus="setHoverRating(star)"
                                @click="rateNote(star)">
                            <span class="star interactive"
                                  :class="{ filled: isInputStarFilled(star) }"></span>
                        </button>
                    </div>
                </div>

                <div class="complaint-block">
                    <div class="complaint-label">
                        <div class="flag-icon"></div>
                        <span>Пожаловаться</span>
                    </div>
                    <button class="report-btn" @click="openComplaintModal">Сообщить о проблеме</button>
                </div>
            </div>
        </div>

        <div v-else class="loading">
            <p>Загрузка...</p>
        </div>

        <!-- Модальное окно жалобы -->
        <div v-if="showComplaintModal" class="modal-overlay" @click.self="closeComplaintModal">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="flag-icon"></div>
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
                            <div v-for="reason in complaintReasons"
                                 :key="reason"
                                 class="select-option"
                                 @click="selectReason(reason)">
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
        padding: 32px 80px;
        max-width: 1440px;
        margin: 0 auto;
        font-family: 'Inter', system-ui, -apple-system, sans-serif;
    }

    .header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 40px;
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

    .back-button {
        cursor: pointer;
        margin-bottom: 24px;
        width: 49px;
        transition: transform 0.2s;
    }

    .back-button:hover {
        transform: translateX(-2px);
    }

    .back-button:hover circle,
    .back-button:hover polyline {
        stroke: #6C63FF;
    }

    .content {
        display: flex;
        gap: 40px;
    }

    .main-column {
        flex: 1;
    }

    .side-column {
        width: 340px;
    }

    .loading {
        text-align: center;
        padding: 100px;
        color: #FFFFFF;
        font-size: 24px;
    }

    .note-title {
        font-size: 36px;
        font-weight: 700;
        color: #FFFFFF;
        margin-bottom: 24px;
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

    .book-icon-small {
        width: 24px;
        height: 24px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%236C63FF" stroke-width="1.6"%3E%3Cpath d="M4 6h16v12H4z"/%3E%3Cpath d="M8 6v12"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .uni-icon-small {
        width: 24px;
        height: 24px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%236C63FF" stroke-width="1.6"%3E%3Cpath d="M12 3L2 9l10 6 10-6-10-6zM2 9v6l10 6 10-6V9"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .user-icon-small {
        width: 24px;
        height: 24px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%236C63FF" stroke-width="1.6"%3E%3Ccircle cx="12" cy="8" r="4"/%3E%3Cpath d="M5 20v-2a7 7 0 0 1 14 0v2"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .calendar-icon-small {
        width: 24px;
        height: 24px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%236C63FF" stroke-width="1.6"%3E%3Crect x="3" y="4" width="18" height="18" rx="2" ry="2"/%3E%3Cline x1="16" y1="2" x2="16" y2="6"/%3E%3Cline x1="8" y1="2" x2="8" y2="6"/%3E%3Cline x1="3" y1="10" x2="21" y2="10"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .description-block {
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 16px;
        padding: 24px;
        margin-bottom: 32px;
        transition: border-color 0.2s, box-shadow 0.2s;
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

    .file-block h3 {
        font-size: 20px;
        font-weight: 700;
        color: #FFFFFF;
        margin-bottom: 16px;
    }

    .file-block {
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 16px;
        padding: 24px;
        transition: border-color 0.2s, box-shadow 0.2s;
    }

    .file-card {
        background: #0F0F14;
        border: 1px solid #2A2A35;
        border-radius: 12px;
        padding: 24px;
        display: flex;
        align-items: center;
        gap: 24px;
        flex-wrap: wrap;
        transition: border-color 0.2s, box-shadow 0.2s;
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
    }

    .view-file-btn, .download-file-btn {
        min-width: 260px;
        height: 56px;
        padding: 14px 28px;
        border-radius: 14px;
        font-size: 20px;
        font-weight: 700;
        cursor: pointer;
        transition: all 0.2s, border-color 0.2s, box-shadow 0.2s;
    }

    .view-file-btn {
        background: #6C63FF;
        border: 1px solid #6C63FF;
        color: #FFFFFF;
    }

        .view-file-btn:hover {
            background: #594FFE;
            transform: translateY(-2px);
        }

    .download-file-btn {
        background: #6C63FF;
        border: 1px solid #6C63FF;
        color: #FFFFFF;
        display: flex;
        align-items: center;
        gap: 8px;
    }

        .download-file-btn::before {
            content: '';
            width: 24px;
            height: 24px;
            background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="white"%3E%3Cpath d="M12 16l-4-4h3V4h2v8h3l-4 4zM5 20h14v-2H5v2z"/%3E%3C/svg%3E') no-repeat center;
            background-size: contain;
        }

        .download-file-btn:hover {
            background: #594FFE;
            transform: translateY(-2px);
        }

    .file-auth-hint {
        width: 100%;
        margin-top: 6px;
        color: #A0A0B0;
        font-size: 14px;
        line-height: 1.4;
    }

    .rating-block {
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 16px;
        padding: 24px;
        margin-bottom: 24px;
        transition: border-color 0.2s, box-shadow 0.2s;
    }

    .rating-header {
        margin-bottom: 10px;
        text-align: left;
    }

        .rating-header h3 {
            font-size: 20px;
            font-weight: 700;
            color: #FFFFFF;
        }

    .rating-value {
        font-size: 64px;
        line-height: 1;
        font-weight: 700;
        color: #FFFFFF;
        text-align: center;
        margin-bottom: 10px;
    }

    .rating-stars-static,
    .rating-stars-input {
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 4px;
        flex-wrap: nowrap;
    }

    .rating-stars-static {
        margin-bottom: 10px;
    }

    .rating-stars-input {
        margin-top: 2px;
    }

    .star {
        display: block;
        width: 22px;
        height: 22px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23686D88" stroke-width="1.5"%3E%3Cpolygon points="12 2 15 9 22 9 16 14 18 21 12 17 6 21 8 14 2 9 9 9 12 2"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

        .star.filled {
            background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="%23FFE100"%3E%3Cpolygon points="12 2 15 9 22 9 16 14 18 21 12 17 6 21 8 14 2 9 9 9 12 2"/%3E%3C/svg%3E') no-repeat center;
            background-size: contain;
        }

    .star-button {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        width: 22px;
        height: 22px;
        border: none;
        background: transparent;
        padding: 0;
        line-height: 0;
        cursor: pointer;
    }

        .star-button:disabled {
            cursor: wait;
            opacity: 0.75;
        }

    .star.interactive {
        transition: transform 0.18s ease, filter 0.2s ease;
    }

    .star-button:hover .star.interactive,
    .star-button:focus-visible .star.interactive {
        transform: scale(1.14);
        filter: drop-shadow(0 0 4px rgba(255, 225, 0, 0.45));
    }

    .rating-text {
        font-size: 16px;
        color: #A0A0B0;
        margin-bottom: 12px;
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
        margin: 12px 0 16px;
    }

    .complaint-block {
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 16px;
        padding: 24px;
        display: flex;
        flex-direction: column;
        gap: 16px;
        transition: border-color 0.2s, box-shadow 0.2s;
    }

    .complaint-label {
        display: flex;
        align-items: center;
        gap: 8px;
        cursor: default;
    }

    .flag-icon {
        width: 24px;
        height: 24px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23FFFFFF" stroke-width="2"%3E%3Cpath d="M4 15s1-1 4-1 5 2 8 2 4-1 4-1V3s-1 1-4 1-5-2-8-2-4 1-4 1z"/%3E%3Cline x1="4" y1="22" x2="4" y2="15"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .complaint-label span {
        font-size: 20px;
        font-weight: 700;
        color: #FFFFFF;
    }

    .report-btn {
        background: #6C63FF;
        border: 1px solid #6C63FF;
        border-radius: 14px;
        padding: 14px;
        font-size: 20px;
        font-weight: 700;
        color: #FFFFFF;
        cursor: pointer;
        transition: all 0.2s;
        text-align: center;
    }

        .report-btn:hover {
            background: #594FFE;
            transform: translateY(-2px);
        }

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
        transition: border-color 0.2s, box-shadow 0.2s;
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
        transition: border-color 0.2s, box-shadow 0.2s;
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
        transition: all 0.2s, border-color 0.2s, box-shadow 0.2s;
    }

    .cancel-btn {
        background: #0F0F14;
        border: 1px solid #2A2A35;
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
        .note-details-page {
            padding: 20px 40px;
        }

        .content {
            flex-direction: column;
        }

        .side-column {
            width: 100%;
        }

        .file-card {
            flex-direction: column;
            text-align: center;
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
            font-size: 42px;
        }

        .star {
            width: 24px;
            height: 24px;
        }

        .rating-text {
            font-size: 15px;
        }

        .downloads-count {
            font-size: 15px;
        }

        .complaint-btn, .report-btn {
            font-size: 16px;
            padding: 10px;
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
