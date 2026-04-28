<template>
    <div class="note-details-page">
        <div class="page-shell">
            <div class="topbar">
                <div class="topbar-left">
                    <button class="back-button" type="button" @click="goBack" aria-label="Назад">
                        <svg width="49" height="49" viewBox="0 0 49 49" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <circle cx="24.5" cy="24.5" r="23" stroke="currentColor" stroke-width="2.5" />
                            <polyline points="27 17 18 24.5 27 32" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" />
                        </svg>
                    </button>

                    <div class="brand" @click="goToCatalog">
                        <div class="brand-mark">
                            <svg width="55" height="50" viewBox="0 0 69 69" fill="none" xmlns="http://www.w3.org/2000/svg">
                                <path d="M34.5 20.125C34.5 17.075 33.2884 14.1499 31.1317 11.9933C28.9751 9.8366 26.05 8.625 23 8.625H5.75V51.75H25.875C28.1625 51.75 30.3563 52.6587 31.9738 54.2762C33.5913 55.8937 34.5 58.0875 34.5 60.375M34.5 20.125V60.375M34.5 20.125C34.5 17.075 35.7116 14.1499 37.8683 11.9933C40.0249 9.8366 42.95 8.625 46 8.625H63.25V51.75H43.125C40.8375 51.75 38.6437 52.6587 37.0262 54.2762C35.4087 55.8937 34.5 58.0875 34.5 60.375" stroke="#6C63FF" stroke-width="4" stroke-linecap="round" stroke-linejoin="round" />
                            </svg>
                        </div>
                        <span class="brand-text">Каталог конспектов</span>
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

            <div v-if="note.id" class="content">
                <div class="content-main">
                    <h1 class="note-title">{{ note.title }}</h1>

                    <div class="note-meta">
                        <div class="meta-item">
                            <span class="meta-icon book-icon"></span>
                            <span>{{ note.subject }}</span>
                        </div>
                        <div class="meta-item">
                            <span class="meta-icon university-icon"></span>
                            <span>{{ note.university }}</span>
                        </div>
                        <div class="meta-item">
                            <span class="meta-icon teacher-icon"></span>
                            <span>{{ note.teacher }}</span>
                        </div>
                        <div class="meta-item">
                            <span class="meta-icon calendar-icon"></span>
                            <span>{{ formatDate(note.uploadedAt) }}</span>
                        </div>
                    </div>

                    <div class="details-layout">
                        <div class="main-column">
                            <section class="panel description-panel">
                                <h2 class="panel-title">Описание</h2>
                                <p class="description-text">{{ note.description }}</p>
                            </section>

                            <section class="panel file-panel">
                                <h2 class="panel-title">Файл конспекта</h2>
                                <div class="file-card">
                                    <div class="file-card-head">
                                        <div class="file-icon-wrap">
                                            <span class="file-icon-large"></span>
                                        </div>
                                        <div class="file-info">
                                            <div class="file-name">{{ getFileName(note.filePath) }}</div>
                                            <div class="file-type">PDF файл</div>
                                        </div>
                                    </div>

                                    <div class="file-actions">
                                        <button class="primary-button file-action-button" type="button" @click="viewFile">
                                            <span class="action-icon preview-icon"></span>
                                            Посмотреть файл
                                        </button>
                                        <button class="primary-button file-action-button secondary-action" type="button" @click="downloadFile">
                                            <span class="action-icon download-icon"></span>
                                            Скачать конспект
                                        </button>
                                    </div>

                                    <p v-if="!authStore.isAuthenticated" class="file-auth-hint">
                                        Для просмотра и скачивания войдите в аккаунт
                                    </p>
                                </div>
                            </section>
                        </div>

                        <aside class="side-column">
                            <section class="panel rating-panel">
                                <h2 class="panel-title">Рейтинг</h2>
                                <div class="rating-value">{{ displayRating }}</div>
                                <div class="rating-stars-static">
                                    <span
                                        v-for="star in maxRating"
                                        :key="star"
                                        class="rating-star static-star"
                                        :class="{ filled: isDisplayStarFilled(star) }"
                                    ></span>
                                </div>
                                <div class="downloads-count">{{ note.downloadsCount }} скачиваний</div>
                                <div class="rating-divider"></div>
                                <p class="rating-caption">Оцените конспект</p>
                                <div class="rating-stars-input" @mouseleave="setHoverRating(0)">
                                    <button
                                        v-for="star in maxRating"
                                        :key="`input-${star}`"
                                        type="button"
                                        class="star-button"
                                        :disabled="isRatingSaving"
                                        @mouseenter="setHoverRating(star)"
                                        @focus="setHoverRating(star)"
                                        @click="rateNote(star)"
                                    >
                                        <span class="rating-star interactive-star" :class="{ filled: isInputStarFilled(star) }"></span>
                                    </button>
                                </div>
                            </section>

                            <section class="panel complaint-panel">
                                <div class="complaint-head">
                                    <span class="flag-icon"></span>
                                    <span>Пожаловаться</span>
                                </div>
                                <button class="primary-button report-button" type="button" @click="openComplaintModal">
                                    Сообщить о проблеме
                                </button>
                            </section>
                        </aside>
                    </div>
                </div>
            </div>

            <div v-else class="loading-state">
                <p>Загрузка...</p>
            </div>
        </div>

        <div v-if="showComplaintModal" class="modal-overlay" @click.self="closeComplaintModal">
            <div class="modal-card complaint-modal">
                <div class="modal-header">
                    <span class="modal-flag-icon"></span>
                    <h2 class="modal-title">Пожаловаться на конспект</h2>
                </div>
                <p class="modal-description">
                    Укажите причину жалобы. Модераторы рассмотрят ваше обращение в ближайшее время.
                </p>

                <div class="form-group">
                    <label>Причина жалобы*</label>
                    <div class="custom-select" :class="{ open: isReasonOpen }">
                        <div class="select-trigger" @click="toggleReasonDropdown">
                            <span :class="{ placeholder: !selectedReason }">{{ selectedReason || 'Выберите причину' }}</span>
                            <span class="chevron-down"></span>
                        </div>
                        <div v-if="isReasonOpen" class="select-dropdown">
                            <div
                                v-for="reason in complaintReasons"
                                :key="reason"
                                class="select-option"
                                @click="selectReason(reason)"
                            >
                                {{ reason }}
                            </div>
                        </div>
                    </div>
                </div>

                <div class="form-group">
                    <label>Комментарий (необязательно)</label>
                    <textarea v-model="complaintComment" placeholder="Опишите проблему подробнее..."></textarea>
                </div>

                <div class="modal-actions">
                    <button class="ghost-button modal-button" type="button" @click="closeComplaintModal">Отмена</button>
                    <button class="primary-button modal-button" type="button" @click="submitComplaint" :disabled="isComplaintSubmitting">
                        {{ isComplaintSubmitting ? 'Отправка...' : 'Отправить жалобу' }}
                    </button>
                </div>
            </div>
        </div>

        <div v-if="showNotification" class="notification">
            <div class="notification-content">
                <span class="notification-check"></span>
                <span>Жалоба отправлена</span>
                <span class="notification-close" @click="showNotification = false"></span>
            </div>
        </div>
    </div>
</template>

<script setup>
    import { computed, onMounted, onUnmounted, ref } from 'vue'
    import { useRoute, useRouter } from 'vue-router'
    import { useAuthStore } from '@/stores/auth'
    import api from '@/services/api'

    const router = useRouter()
    const route = useRoute()
    const authStore = useAuthStore()

    const isMenuOpen = ref(false)
    const showComplaintModal = ref(false)
    const isReasonOpen = ref(false)
    const selectedReason = ref('')
    const complaintComment = ref('')
    const showNotification = ref(false)
    const isRatingSaving = ref(false)
    const isComplaintSubmitting = ref(false)
    const hoverRating = ref(0)
    const notificationTimeout = ref(null)
    const maxRating = 10

    const complaintReasons = [
        'Нарушение правил',
        'Не соответствует описанию',
        'Неправильный предмет',
        'Плохое качество'
    ]

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

    const saveToHistory = (noteData) => {
        const history = JSON.parse(localStorage.getItem('downloadHistory') || '[]')
        const existingIndex = history.findIndex((item) => item.id === noteData.id)
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
            const response = await api.get(`/notes/${route.params.id}`)
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
            query: { redirect: route.fullPath }
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

    const isDisplayStarFilled = (star) => star <= Math.round(safeRating.value)

    const isInputStarFilled = (star) => {
        const activeRating = hoverRating.value > 0 ? hoverRating.value : Math.round(safeUserRating.value)
        return star <= activeRating
    }

    const openComplaintModal = () => {
        if (!authStore.isAuthenticated) {
            redirectToLogin()
            return
        }
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

    const submitComplaint = async () => {
        if (!selectedReason.value) {
            alert('Пожалуйста, выберите причину жалобы')
            return
        }

        try {
            isComplaintSubmitting.value = true
            await api.post(`/notes/${note.value.id}/complaints`, {
                reason: selectedReason.value,
                comment: complaintComment.value?.trim() || null
            })

            closeComplaintModal()
            showNotification.value = true
            if (notificationTimeout.value) clearTimeout(notificationTimeout.value)
            notificationTimeout.value = setTimeout(() => {
                showNotification.value = false
            }, 3000)
        } catch (error) {
            console.error('Ошибка отправки жалобы:', error)
            const message =
                error.response?.data?.message ||
                error.response?.data?.detail ||
                error.response?.data?.title ||
                'Не удалось отправить жалобу'
            alert(message)
        } finally {
            isComplaintSubmitting.value = false
        }
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
        if (notificationTimeout.value) clearTimeout(notificationTimeout.value)
    })
</script>

<style scoped>
    .note-details-page {
        min-height: 100vh;
        background:
            radial-gradient(circle at top right, rgba(120, 107, 255, 0.14), transparent 26%),
            linear-gradient(180deg, #111117 0%, #0d0d12 100%);
        color: #f3f2fb;
        font-family: Inter, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif;
    }

    .page-shell {
        max-width: 1440px;
        margin: 0 auto;
        padding: 32px 80px 96px;
    }

    .topbar {
        display: flex;
        align-items: center;
        justify-content: space-between;
        margin-bottom: 30px;
    }

    .topbar-left {
        display: flex;
        align-items: center;
        gap: 22px;
    }

    .back-button {
        width: 49px;
        height: 49px;
        border: none;
        background: transparent;
        color: #a0a0b0;
        cursor: pointer;
        padding: 0;
        transition: color 0.2s ease, transform 0.2s ease;
    }

    .back-button:hover {
        color: #6C63FF;
        transform: translateX(-2px);
    }

    .brand {
        display: inline-flex;
        align-items: center;
        gap: 12px;
        cursor: pointer;
    }

    .brand-mark {
        width: 56px;
        height: 56px;
        border-radius: 10px;
        background: #2A2348;
        display: flex;
        align-items: center;
        justify-content: center;
    }

    .brand-mark svg {
        width: 44px;
        height: 44px;
    }

    .brand-text {
        font-size: 24px;
        font-weight: 700;
        color: #ffffff;
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
        overflow: hidden;
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

    .loading-state {
        padding: 100px 0;
        text-align: center;
        font-size: 24px;
        color: #ffffff;
    }

    .note-title {
        margin: 0 0 14px;
        font-size: 36px;
        line-height: 1.2;
        font-weight: 700;
        color: #ffffff;
    }

    .note-meta {
        display: flex;
        flex-wrap: wrap;
        gap: 18px;
        margin-bottom: 28px;
    }

    .meta-item {
        display: inline-flex;
        align-items: center;
        gap: 8px;
        font-size: 20px;
        font-weight: 500;
        color: #A0A0B0;
    }

    .meta-icon,
    .action-icon,
    .flag-icon,
    .modal-flag-icon,
    .notification-check,
    .notification-close,
    .file-icon-large,
    .rating-star {
        display: inline-block;
        background-repeat: no-repeat;
        background-position: center;
        background-size: contain;
        flex-shrink: 0;
    }

    .meta-icon {
        width: 24px;
        height: 24px;
    }

    .book-icon {
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%236C63FF" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"%3E%3Cpath d="M12 6c0-1.657-1.79-3-4-3H3v15h5c2.21 0 4 1.343 4 3"/%3E%3Cpath d="M12 6c0-1.657 1.79-3 4-3h5v15h-5c-2.21 0-4 1.343-4 3"/%3E%3Cpath d="M12 6v15"/%3E%3C/svg%3E');
    }

    .university-icon {
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%236C63FF" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"%3E%3Cpath d="M22 10L12 5 2 10l10 5 10-5Z"/%3E%3Cpath d="M6 12v5c0 1 3 3 6 3s6-2 6-3v-5"/%3E%3C/svg%3E');
    }

    .teacher-icon {
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%236C63FF" stroke-width="1.9" stroke-linecap="round" stroke-linejoin="round"%3E%3Cpath d="M20 21c0-3.4-3.58-6-8-6s-8 2.6-8 6"/%3E%3Ccircle cx="12" cy="8" r="4"/%3E%3C/svg%3E');
    }

    .calendar-icon {
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%236C63FF" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"%3E%3Crect x="3" y="4" width="18" height="18" rx="2"/%3E%3Cpath d="M16 2v4"/%3E%3Cpath d="M8 2v4"/%3E%3Cpath d="M3 10h18"/%3E%3C/svg%3E');
    }

    .content {
        display: flex;
    }

    .content-main {
        width: 100%;
    }

    .details-layout {
        display: grid;
        grid-template-columns: minmax(0, 1fr) 300px;
        gap: 28px;
        align-items: start;
    }

    .main-column,
    .side-column {
        display: flex;
        flex-direction: column;
        gap: 28px;
    }

    .panel {
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 16px;
    }

    .description-panel {
        min-height: 180px;
        padding: 24px 28px;
    }

    .file-panel {
        padding: 24px 28px 28px;
    }

    .rating-panel {
        padding: 22px 24px 24px;
    }

    .complaint-panel {
        padding: 22px 24px 28px;
    }

    .panel-title {
        margin: 0 0 16px;
        font-size: 20px;
        font-weight: 700;
        color: #ffffff;
    }

    .description-text {
        margin: 0;
        font-size: 18px;
        line-height: 1.55;
        font-weight: 500;
        color: #c4c4d2;
        max-width: 690px;
    }

    .file-card {
        background: #111118;
        border: 1px solid #1f1f2a;
        border-radius: 14px;
        padding: 28px 26px 22px;
    }

    .file-card-head {
        display: flex;
        align-items: center;
        gap: 18px;
        margin-bottom: 26px;
    }

    .file-icon-wrap {
        width: 78px;
        height: 78px;
        border-radius: 14px;
        background: rgba(108, 99, 255, 0.16);
        display: flex;
        align-items: center;
        justify-content: center;
    }

    .file-icon-large {
        width: 40px;
        height: 46px;
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%236C63FF" stroke-width="2"%3E%3Cpath d="M13 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9z"/%3E%3Cpolyline points="13 2 13 9 20 9"/%3E%3Cpath d="M9 13h6"/%3E%3Cpath d="M9 17h6"/%3E%3C/svg%3E');
    }

    .file-info {
        min-width: 0;
    }

    .file-name {
        font-size: 18px;
        line-height: 1.3;
        font-weight: 600;
        color: #ffffff;
        word-break: break-word;
    }

    .file-type {
        margin-top: 4px;
        font-size: 14px;
        font-weight: 500;
        color: #8b8b98;
    }

    .file-actions {
        display: grid;
        gap: 16px;
        max-width: 500px;
        margin: 0 auto;
    }

    .primary-button,
    .ghost-button {
        min-height: 50px;
        border-radius: 14px;
        border: none;
        font-size: 18px;
        font-weight: 700;
        cursor: pointer;
        transition: transform 0.2s ease, box-shadow 0.2s ease, background 0.2s ease, border-color 0.2s ease;
    }

    .primary-button {
        background: linear-gradient(90deg, #6f61ff 0%, #7d71ff 100%);
        color: #ffffff;
        box-shadow: none;
    }

    .primary-button:hover {
        transform: translateY(-1px);
        box-shadow: 0 14px 28px rgba(108, 99, 255, 0.3);
    }

    .ghost-button {
        background: #111118;
        color: #ffffff;
        border: 1px solid #20202b;
    }

    .file-action-button {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        gap: 10px;
        width: 100%;
    }

    .secondary-action {
        background: linear-gradient(90deg, #6f61ff 0%, #7d71ff 100%);
    }

    .action-icon {
        width: 24px;
        height: 24px;
    }

    .preview-icon {
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23ffffff" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"%3E%3Cpath d="M14 3H7a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V8z"/%3E%3Cpath d="M14 3v5h5"/%3E%3Cpath d="M8.5 15.25c1.05-1.55 2.2-2.33 3.5-2.33s2.45.78 3.5 2.33c-1.05 1.55-2.2 2.33-3.5 2.33s-2.45-.78-3.5-2.33Z"/%3E%3Ccircle cx="12" cy="15.25" r="0.9" fill="%23ffffff" stroke="none"/%3E%3C/svg%3E');
    }

    .download-icon {
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23ffffff" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"%3E%3Cpath d="M12 3v12"/%3E%3Cpath d="m7 10 5 5 5-5"/%3E%3Cpath d="M4 21h16"/%3E%3C/svg%3E');
    }

    .file-auth-hint {
        margin: 16px 0 0;
        font-size: 14px;
        text-align: center;
        color: #8f8fa0;
    }

    .rating-value {
        font-size: 56px;
        line-height: 1;
        font-weight: 700;
        color: #ffffff;
        text-align: center;
        margin-bottom: 16px;
    }

    .rating-stars-static,
    .rating-stars-input {
        display: flex;
        justify-content: center;
        flex-wrap: nowrap;
        gap: 2px;
    }

    .rating-stars-static {
        margin-bottom: 8px;
    }

    .downloads-count {
        text-align: center;
        font-size: 16px;
        font-weight: 500;
        color: #a0a0b0;
    }

    .rating-divider {
        height: 1px;
        background: #2A2A35;
        margin: 16px 0 12px;
    }

    .rating-caption {
        margin: 0 0 10px;
        font-size: 16px;
        color: #a0a0b0;
    }

    .star-button {
        border: none;
        background: transparent;
        padding: 0;
        cursor: pointer;
    }

    .star-button:disabled {
        cursor: wait;
    }

    .rating-star {
        width: 22px;
        height: 22px;
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%237A7E96" stroke-width="2"%3E%3Cpolygon points="12 2.5 14.93 8.44 21.5 9.39 16.75 14.02 17.87 20.57 12 17.49 6.13 20.57 7.25 14.02 2.5 9.39 9.07 8.44 12 2.5"/%3E%3C/svg%3E');
    }

    .rating-star.filled {
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="%23FFD539"%3E%3Cpolygon points="12 2.5 14.93 8.44 21.5 9.39 16.75 14.02 17.87 20.57 12 17.49 6.13 20.57 7.25 14.02 2.5 9.39 9.07 8.44 12 2.5"/%3E%3C/svg%3E');
    }

    .complaint-head {
        display: inline-flex;
        align-items: center;
        gap: 10px;
        margin-bottom: 18px;
        font-size: 18px;
        font-weight: 700;
        color: #ffffff;
    }

    .flag-icon,
    .modal-flag-icon {
        width: 22px;
        height: 22px;
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23FFFFFF" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"%3E%3Cpath d="M5 20V5"/%3E%3Cpath d="M5 5h9l-1.2 3.5L17 11H5"/%3E%3C/svg%3E');
    }

    .report-button {
        width: 100%;
    }

    .modal-overlay {
        position: fixed;
        inset: 0;
        background: rgba(6, 6, 10, 0.62);
        backdrop-filter: blur(10px);
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 24px;
        z-index: 200;
    }

    .modal-card {
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 20px;
        box-shadow: 0 22px 60px rgba(0, 0, 0, 0.5);
    }

    .complaint-modal {
        width: 100%;
        max-width: 590px;
        padding: 28px 28px 24px;
    }

    .modal-header {
        display: flex;
        align-items: center;
        gap: 14px;
        margin-bottom: 16px;
    }

    .modal-title {
        margin: 0;
        font-size: 24px;
        line-height: 1.2;
        font-weight: 700;
        color: #ffffff;
    }

    .modal-description {
        margin: 0 0 22px;
        font-size: 16px;
        line-height: 1.45;
        color: #a0a0b0;
        max-width: 470px;
    }

    .form-group {
        margin-bottom: 18px;
    }

    .form-group label {
        display: block;
        margin-bottom: 10px;
        font-size: 16px;
        font-weight: 600;
        color: #ffffff;
    }

    .custom-select {
        position: relative;
    }

    .select-trigger,
    textarea {
        width: 100%;
        border-radius: 12px;
        border: 1px solid #2A2A35;
        background: #111118;
        color: #ffffff;
        font-size: 16px;
        outline: none;
    }

    .select-trigger {
        min-height: 52px;
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 12px;
        padding: 0 18px;
        cursor: pointer;
    }

    .select-trigger span.placeholder {
        color: #8b8b98;
    }

    .chevron-down {
        width: 16px;
        height: 16px;
        border-right: 2px solid #8b8b98;
        border-bottom: 2px solid #8b8b98;
        transform: rotate(45deg) translateY(-3px);
        flex-shrink: 0;
    }

    .select-dropdown {
        position: absolute;
        top: calc(100% + 8px);
        left: 0;
        right: 0;
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 12px;
        overflow: hidden;
        z-index: 10;
    }

    .select-option {
        padding: 14px 18px;
        color: #ffffff;
        cursor: pointer;
        transition: background 0.2s ease;
    }

    .select-option:hover {
        background: #262633;
    }

    textarea {
        min-height: 136px;
        resize: vertical;
        padding: 16px 18px;
        line-height: 1.45;
    }

    textarea::placeholder {
        color: #8b8b98;
    }

    .modal-actions {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 14px;
        margin-top: 8px;
    }

    .modal-button {
        min-height: 52px;
    }

    .notification {
        position: fixed;
        right: 32px;
        bottom: 32px;
        z-index: 300;
    }

    .notification-content {
        display: inline-flex;
        align-items: center;
        gap: 12px;
        padding: 16px 18px;
        border-radius: 18px;
        background: rgba(24, 24, 33, 0.98);
        border: 1px solid #2A2A35;
        box-shadow: 0 0 0 1px rgba(108, 99, 255, 0.16), 0 0 30px rgba(108, 99, 255, 0.22), 0 18px 40px rgba(7, 7, 10, 0.32);
        color: #ffffff;
        font-size: 18px;
        font-weight: 600;
    }

    .notification-check {
        width: 22px;
        height: 22px;
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%2327D36B" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"%3E%3Ccircle cx="12" cy="12" r="9"/%3E%3Cpath d="m8 12 2.5 2.5L16 9"/%3E%3C/svg%3E');
    }

    .notification-close {
        width: 18px;
        height: 18px;
        cursor: pointer;
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23A0A0B0" stroke-width="2"%3E%3Cpath d="M6 6l12 12"/%3E%3Cpath d="M18 6 6 18"/%3E%3C/svg%3E');
    }

    @media (max-width: 1200px) {
        .page-shell {
            padding: 24px 32px 72px;
        }

        .details-layout {
            grid-template-columns: 1fr;
        }

        .side-column {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px;
        }
    }

    @media (max-width: 860px) {
        .page-shell {
            padding: 20px 16px 64px;
        }

        .topbar,
        .topbar-left {
            gap: 14px;
        }

        .brand-text {
            font-size: 20px;
        }

        .content-main {
            width: 100%;
        }

        .note-title {
            font-size: 30px;
        }

        .note-meta {
            gap: 12px;
        }

        .meta-item {
            font-size: 16px;
        }

        .side-column {
            grid-template-columns: 1fr;
        }
    }

    @media (max-width: 640px) {
        .topbar {
            align-items: flex-start;
        }

        .note-meta {
            flex-direction: column;
            align-items: flex-start;
        }

        .file-card-head {
            align-items: flex-start;
        }

        .modal-actions {
            grid-template-columns: 1fr;
        }

        .notification {
            left: 16px;
            right: 16px;
            bottom: 16px;
        }

        .notification-content {
            width: 100%;
            justify-content: space-between;
        }
    }
</style>
