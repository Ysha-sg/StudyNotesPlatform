<template>
    <div class="profile-page">
        <div class="page-shell">
            <div class="topbar">
                <button class="back-button" type="button" @click="goBack" aria-label="Назад">
                    <svg width="49" height="49" viewBox="0 0 49 49" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <circle cx="24.5" cy="24.5" r="23" stroke="currentColor" stroke-width="2.5" />
                        <polyline points="27 17 18 24.5 27 32" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" />
                    </svg>
                </button>

                <div class="brand" @click="goToCatalog">
                    <div class="brand-mark">
                        <svg width="60" height="60" viewBox="0 0 69 69" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M34.5 20.125C34.5 17.075 33.2884 14.1499 31.1317 11.9933C28.9751 9.8366 26.05 8.625 23 8.625H5.75V51.75H25.875C28.1625 51.75 30.3563 52.6587 31.9738 54.2762C33.5913 55.8937 34.5 58.0875 34.5 60.375M34.5 20.125V60.375M34.5 20.125C34.5 17.075 35.7116 14.1499 37.8683 11.9933C40.0249 9.8366 42.95 8.625 46 8.625H63.25V51.75H43.125C40.8375 51.75 38.6437 52.6587 37.0262 54.2762C35.4087 55.8937 34.5 58.0875 34.5 60.375" stroke="#8B7FFF" stroke-width="4" stroke-linecap="round" stroke-linejoin="round" />
                        </svg>
                    </div>
                    <span class="brand-text">Каталог конспектов</span>
                </div>
            </div>

            <section class="profile-card">
                <div class="profile-main">
                    <div class="profile-avatar">
                        <span class="profile-avatar-text">{{ avatarInitials }}</span>
                    </div>

                    <div class="profile-content">
                        <h1 class="profile-name">{{ profile.fullName || 'Пользователь' }}</h1>
                        <div class="profile-meta">
                            <div class="meta-item">
                                <span class="meta-icon graduate-icon"></span>
                                <span>{{ profile.university || 'ВУЗ не указан' }}</span>
                            </div>
                            <div class="meta-item">
                                <span class="meta-icon user-icon"></span>
                                <span>{{ profile.email || 'Email не указан' }}</span>
                            </div>
                        </div>
                        <button class="primary-button edit-profile-button" type="button" @click="openEditModal">
                            <span class="button-icon edit-icon"></span>
                            Редактировать профиль
                        </button>
                    </div>
                </div>

                <button class="logout-button" type="button" @click="handleLogout" aria-label="Выйти из профиля">
                    <svg width="42" height="42" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M9 21H5C3.89543 21 3 20.1046 3 19V5C3 3.89543 3.89543 3 5 3H9" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" />
                        <path d="M16 17L21 12L16 7" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" />
                        <path d="M21 12H9" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" />
                    </svg>
                </button>
            </section>

            <nav class="tabs" aria-label="Разделы профиля">
                <button
                    v-for="tab in tabs"
                    :key="tab.key"
                    class="tab-button"
                    :class="{ active: activeTab === tab.key }"
                    type="button"
                    @click="activeTab = tab.key"
                >
                    <span class="tab-icon" :class="tab.icon"></span>
                    <span class="tab-label">{{ tab.label }}</span>
                </button>
            </nav>

            <div v-if="activeTab === 'myNotes'" class="toolbar">
                <button class="secondary-button add-note-button" type="button" @click="goToAddNote">
                    + Добавить
                </button>
            </div>

            <section class="notes-list">
                <article v-for="note in currentNotes" :key="`${activeTab}-${note.id}`" class="note-card">
                    <div class="note-head">
                        <div class="note-file-icon">
                            <span class="note-file-symbol"></span>
                        </div>

                        <div class="note-summary">
                            <div class="note-title-row">
                                <div class="note-title-block">
                                    <h2 class="note-title">{{ note.title }}</h2>
                                    <p class="note-subtitle">{{ note.subject || note.title }}</p>
                                </div>

                                <div class="note-title-side">
                                    <div
                                        v-if="activeTab === 'myNotes'"
                                        class="status-badge"
                                        :class="getStatusClass(note.status)"
                                    >
                                        {{ getStatusText(note.status) }}
                                    </div>

                                    <button
                                        v-else-if="activeTab === 'favorites'"
                                        class="icon-button favorite-indicator"
                                        type="button"
                                        @click="toggleFavoriteNote(note)"
                                        aria-label="Убрать из избранного"
                                    >
                                        <span class="heart liked"></span>
                                    </button>

                                    <button
                                        v-else-if="activeTab === 'history'"
                                        class="icon-button history-indicator"
                                        type="button"
                                        @click="toggleFavoriteNote(note)"
                                        :aria-label="favoritesStore.isFavorite(note.id) ? 'Убрать из избранного' : 'Добавить в избранное'"
                                    >
                                        <span class="heart" :class="{ liked: favoritesStore.isFavorite(note.id) }"></span>
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="note-meta">
                        <div>Преподаватель: {{ formatTeacherName(note.teacher) || 'Не указан' }}</div>
                        <div>Вуз: {{ note.university || 'Не указан' }}</div>
                        <div v-if="activeTab === 'history' && note.downloadedAt" class="download-date">
                            <span class="tab-icon time-icon active"></span>
                            <span>Скачано: {{ formatDate(note.downloadedAt) }}</span>
                        </div>
                    </div>

                    <div
                        v-if="activeTab === 'myNotes' && isRejected(note.status) && note.rejectionReason"
                        class="rejection-reason"
                    >
                        <div class="rejection-title">Причина отклонения:</div>
                        <div>{{ note.rejectionReason }}</div>
                    </div>

                    <div class="note-footer">
                        <div class="note-stats">
                            <div class="stat rating">
                                <span class="star-icon"></span>
                                <span>{{ formatRating(note.rating ?? note.averageRating) }}</span>
                            </div>
                            <div class="stat">{{ note.downloadsCount ?? 0 }} скачиваний</div>
                        </div>

                        <div class="note-actions">
                            <template v-if="activeTab === 'myNotes' && isRejected(note.status)">
                                <button class="ghost-button delete-note-button" type="button" @click="deleteNote(note.id)">Удалить</button>
                                <button class="primary-button action-button" type="button" @click="editNote(note.id)">Исправить</button>
                            </template>
                            <template v-else>
                                <button class="primary-button action-button" type="button" @click="openNote(note.id)">Открыть</button>
                            </template>
                        </div>
                    </div>
                </article>

                <div v-if="currentNotes.length === 0" class="empty-state">
                    <p>{{ emptyStateText }}</p>
                </div>
            </section>
        </div>

        <div v-if="showEditModal" class="modal-overlay" @click.self="closeEditModal">
            <div class="modal-card">
                <h2 class="modal-title">Редактировать профиль</h2>

                <div class="form-group">
                    <label for="fullName">Имя</label>
                    <input id="fullName" v-model="editForm.fullName" type="text" placeholder="Иван Иванов" />
                </div>

                <div class="form-group">
                    <label for="email">Email</label>
                    <input id="email" v-model="editForm.email" type="email" placeholder="ivan.ivanov@example.com" />
                </div>

                <div class="form-group">
                    <label for="university">ВУЗ</label>
                    <div class="custom-select modal-select" :class="{ open: isUniversityOpen }">
                        <button id="university" type="button" class="select-trigger" @click="toggleUniversityDropdown">
                            <span class="select-value" :class="{ placeholder: !selectedUniversityName }">
                                {{ selectedUniversityName || 'Выберите ВУЗ' }}
                            </span>
                            <span class="chevron-icon"></span>
                        </button>
                        <div v-if="isUniversityOpen" class="select-dropdown">
                            <div class="select-search">
                                <input v-model="universitySearch" type="text" placeholder="Начните вводить ВУЗ" @click.stop />
                            </div>
                            <button type="button" class="select-option" :class="{ selected: !editForm.universityId }" @click="selectUniversity(null)">
                                Выберите ВУЗ
                            </button>
                            <button
                                v-for="uni in filteredUniversities"
                                :key="uni.id"
                                type="button"
                                class="select-option"
                                :class="{ selected: editForm.universityId === uni.id }"
                                @click="selectUniversity(uni)"
                            >
                                {{ uni.name }}
                            </button>
                        </div>
                    </div>
                </div>

                <div class="form-group">
                    <label for="newPassword">Пароль</label>
                    <div class="input-with-icon">
                        <input
                            id="newPassword"
                            v-model="editForm.newPassword"
                            :type="showNewPassword ? 'text' : 'password'"
                            placeholder="Введите новый пароль"
                        />
                        <button
                            class="eye-button"
                            type="button"
                            :class="{ off: !showNewPassword }"
                            @click="showNewPassword = !showNewPassword"
                            aria-label="Показать пароль"
                        ></button>
                    </div>
                </div>

                <div class="form-group">
                    <label for="confirmPassword">Повторите пароль</label>
                    <div class="input-with-icon">
                        <input
                            id="confirmPassword"
                            v-model="editForm.confirmPassword"
                            :type="showConfirmPassword ? 'text' : 'password'"
                            placeholder="Повторите пароль"
                        />
                        <button
                            class="eye-button"
                            type="button"
                            :class="{ off: !showConfirmPassword }"
                            @click="showConfirmPassword = !showConfirmPassword"
                            aria-label="Показать пароль повторно"
                        ></button>
                    </div>
                </div>

                <div v-if="editError" class="error-message">{{ editError }}</div>

                <div class="modal-actions">
                    <button class="ghost-button modal-button cancel-profile-button" type="button" @click="closeEditModal">Отмена</button>
                    <button class="primary-button modal-button save-profile-button" type="button" @click="saveProfile">Сохранить</button>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
    import { computed, onMounted, onUnmounted, reactive, ref } from 'vue'
    import { useRouter } from 'vue-router'
    import { useAuthStore } from '@/stores/auth'
    import { useFavoritesStore } from '@/stores/favorites'
    import api from '@/services/api'

    const router = useRouter()
    const authStore = useAuthStore()
    const favoritesStore = useFavoritesStore()

    const tabs = [
        { key: 'myNotes', label: 'Профиль', icon: 'file-icon' },
        { key: 'favorites', label: 'Избранное', icon: 'heart-icon' },
        { key: 'history', label: 'История скачиваний', icon: 'time-icon' }
    ]

    const profile = reactive({
        fullName: '',
        email: '',
        university: ''
    })

    const universities = ref([])
    const activeTab = ref('myNotes')
    const myNotes = ref([])
    const favorites = ref([])
    const downloadHistory = ref([])
    const showEditModal = ref(false)
    const showNewPassword = ref(false)
    const showConfirmPassword = ref(false)
    const editError = ref('')
    const isUniversityOpen = ref(false)
    const universitySearch = ref('')
    const selectedUniversityName = ref('')

    const editForm = reactive({
        fullName: '',
        email: '',
        universityId: null,
        newPassword: '',
        confirmPassword: ''
    })

    const currentNotes = computed(() => {
        switch (activeTab.value) {
            case 'favorites':
                return favorites.value
            case 'history':
                return downloadHistory.value
            default:
                return myNotes.value
        }
    })

    const emptyStateText = computed(() => {
        switch (activeTab.value) {
            case 'favorites':
                return 'В избранном пока нет конспектов'
            case 'history':
                return 'История скачиваний пока пуста'
            default:
                return 'У вас пока нет конспектов'
        }
    })

    const avatarInitials = computed(() => {
        const parts = (profile.fullName || '')
            .trim()
            .split(/\s+/)
            .filter(Boolean)

        if (parts.length === 0) return 'ИИ'
        return parts.slice(0, 2).map((part) => part[0]?.toUpperCase()).join('')
    })

    const filteredUniversities = computed(() => {
        const query = universitySearch.value.trim().toLowerCase()
        if (!query) return universities.value
        return universities.value.filter((uni) => uni.name.toLowerCase().includes(query))
    })

    const getStatusCode = (status) => {
        if (!status) return ''
        if (typeof status === 'string') return status.toLowerCase()
        return String(status.code || status.name || '').toLowerCase()
    }

    const isRejected = (status) => getStatusCode(status) === 'rejected'

    const goToCatalog = () => {
        router.push('/')
    }

    const goBack = () => {
        router.back()
    }

    const goToAddNote = () => {
        router.push('/add-note')
    }

    const handleLogout = () => {
        authStore.logout()
        router.push('/login')
    }

    const openNote = (id) => {
        router.push(`/note/${id}`)
    }

    const editNote = (id) => {
        router.push(`/edit-note/${id}`)
    }

    const deleteNote = async (id) => {
        if (!confirm('Вы уверены, что хотите удалить этот конспект?')) return

        try {
            await api.delete(`/notes/${id}`)
            myNotes.value = myNotes.value.filter((note) => note.id !== id)
        } catch (error) {
            console.error('Ошибка удаления:', error)
            alert('Не удалось удалить конспект')
        }
    }

    const toggleFavoriteNote = async (note) => {
        await favoritesStore.toggleFavorite(note)
        favorites.value = favoritesStore.favorites
    }

    const getStatusText = (status) => {
        switch (getStatusCode(status)) {
            case 'approved':
                return 'Опубликован'
            case 'rejected':
                return 'Отклонен'
            case 'pending':
                return 'На рассмотрении'
            default:
                return 'Черновик'
        }
    }

    const getStatusClass = (status) => {
        switch (getStatusCode(status)) {
            case 'approved':
                return 'approved'
            case 'rejected':
                return 'rejected'
            case 'pending':
                return 'pending'
            default:
                return ''
        }
    }

    const formatDate = (dateString) => {
        if (!dateString) return ''
        const date = new Date(dateString)
        return date.toLocaleDateString('ru-RU', { day: 'numeric', month: 'long', year: 'numeric' })
    }

    const formatRating = (value) => {
        const numericValue = Number(value)
        if (!Number.isFinite(numericValue) || numericValue <= 0) return '0.0'
        return numericValue.toFixed(1)
    }

    const formatTeacherName = (value) => {
        const source = String(value || '').trim()
        if (!source) return ''

        const normalized = source.replace(/\s+/g, ' ')
        const rankMatch = normalized.match(/^(доц\.|проф\.|ассист\.|асс\.|ст\.?\s*преп\.|преп\.)\s+/i)
        const rank = rankMatch ? `${rankMatch[1].trim().replace(/\.$/, '')}. ` : ''
        const namePart = rankMatch ? normalized.slice(rankMatch[0].length).trim() : normalized
        const parts = namePart.split(' ').filter(Boolean)

        if (parts.length < 2) {
            return `${rank}${namePart}`.trim()
        }

        const [lastName, firstName, middleName] = parts
        const initials = [firstName, middleName]
            .filter(Boolean)
            .map((part) => `${part[0].toUpperCase()}.`)
            .join('')

        return `${rank}${lastName} ${initials}`.trim()
    }

    const loadProfile = async () => {
        try {
            const response = await api.get('/profile/me')
            profile.fullName = response.data.fullName
            profile.email = response.data.email
            profile.university = response.data.university
        } catch (error) {
            console.error('Ошибка загрузки профиля:', error)
        }
    }

    const loadUniversities = async () => {
        try {
            const response = await api.get('/lookup/all-universities')
            universities.value = response.data
        } catch (error) {
            console.error('Ошибка загрузки университетов:', error)
        }
    }

    const loadMyNotes = async () => {
        try {
            const response = await api.get('/notes/my')
            myNotes.value = response.data.map((note) => ({
                ...note,
                rating: note.averageRating ?? note.rating ?? 0
            }))
        } catch (error) {
            console.error('Ошибка загрузки конспектов:', error)
        }
    }

    const loadFavorites = async () => {
        await favoritesStore.loadFavorites()
        favorites.value = favoritesStore.favorites
    }

    const loadDownloadHistory = async () => {
        try {
            const response = await api.get('/notes/download-history')
            downloadHistory.value = response.data
        } catch (error) {
            console.error('Ошибка загрузки истории скачиваний:', error)
            downloadHistory.value = []
        }
    }

    const openEditModal = () => {
        editForm.fullName = profile.fullName
        editForm.email = profile.email
        editForm.universityId = universities.value.find((uni) => uni.name === profile.university)?.id ?? null
        selectedUniversityName.value = profile.university || ''
        universitySearch.value = ''
        isUniversityOpen.value = false
        editForm.newPassword = ''
        editForm.confirmPassword = ''
        showNewPassword.value = false
        showConfirmPassword.value = false
        editError.value = ''
        showEditModal.value = true
    }

    const closeEditModal = () => {
        isUniversityOpen.value = false
        showEditModal.value = false
    }

    const toggleUniversityDropdown = () => {
        isUniversityOpen.value = !isUniversityOpen.value
        if (isUniversityOpen.value) {
            universitySearch.value = ''
        }
    }

    const selectUniversity = (university) => {
        editForm.universityId = university?.id ?? null
        selectedUniversityName.value = university?.name ?? ''
        universitySearch.value = ''
        isUniversityOpen.value = false
    }

    const saveProfile = async () => {
        editError.value = ''

        if (editForm.newPassword !== editForm.confirmPassword) {
            editError.value = 'Пароли не совпадают'
            return
        }

        try {
            const updateData = {
                fullName: editForm.fullName,
                email: editForm.email,
                universityId: editForm.universityId
            }

            if (editForm.newPassword) {
                updateData.newPassword = editForm.newPassword
            }

            await api.put('/profile/update', updateData)

            profile.fullName = editForm.fullName
            profile.email = editForm.email

            if (authStore.user) {
                authStore.user.fullName = editForm.fullName
                authStore.user.email = editForm.email
            }

            if (editForm.universityId) {
                const selectedUni = universities.value.find((uni) => uni.id === editForm.universityId)
                if (selectedUni) {
                    profile.university = selectedUni.name
                    if (authStore.user) {
                        authStore.user.universityName = selectedUni.name
                    }
                }
            }

            if (authStore.user) {
                localStorage.setItem('user', JSON.stringify(authStore.user))
            }

            closeEditModal()
        } catch (error) {
            editError.value = error.response?.data?.message || 'Ошибка сохранения'
        }
    }

    const handleDocumentClick = (event) => {
        if (!event.target.closest('.modal-select')) {
            isUniversityOpen.value = false
        }
    }

    onMounted(() => {
        document.addEventListener('click', handleDocumentClick)
        loadProfile()
        loadUniversities()
        loadMyNotes()
        loadFavorites()
        loadDownloadHistory()
    })

    onUnmounted(() => {
        document.removeEventListener('click', handleDocumentClick)
    })
</script>

<style scoped>
    :global(html) {
        scrollbar-gutter: stable;
    }

    .profile-page {
        min-height: 100vh;
        background:
            radial-gradient(circle at top right, rgba(120, 107, 255, 0.14), transparent 26%),
            linear-gradient(180deg, #111117 0%, #0d0d12 100%);
        color: #f3f2fb;
        font-family: Inter, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif;
    }

    .page-shell {
        max-width: 1320px;
        margin: 0 auto;
        padding: 24px 40px 72px;
    }

    .topbar {
        display: inline-flex;
        align-items: center;
        gap: 18px;
        margin-bottom: 24px;
    }

    .back-button {
        width: 52px;
        height: 52px;
        border: none;
        background: transparent;
        color: #b1afc5;
        cursor: pointer;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        transition: color 0.2s ease, transform 0.2s ease;
        padding: 0;
        flex-shrink: 0;
        transform: translateY(-2px);
    }

    .back-button:hover {
        color: #6C63FF;
        transform: translate(-2px, -2px);
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
        box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.04);
    }

    .brand-mark svg {
        width: 44px;
        height: 44px;
    }

    .brand-text {
        font-size: 24px;
        font-weight: 700;
        letter-spacing: -0.01em;
        color: #f3f2fb;
    }

    .profile-card,
    .tabs,
    .note-card,
    .empty-state,
    .modal-card {
        background: rgba(28, 28, 38, 0.96);
        border: 1px solid #2a2a36;
        box-shadow: 0 18px 40px rgba(7, 7, 10, 0.26);
    }

    .profile-card {
        border-radius: 22px;
        padding: 28px 30px;
        display: flex;
        align-items: flex-start;
        justify-content: space-between;
        gap: 20px;
        margin-bottom: 16px;
    }

    .profile-main {
        display: flex;
        align-items: center;
        gap: 24px;
        min-width: 0;
    }

    .profile-avatar {
        width: 120px;
        height: 120px;
        border-radius: 50%;
        background: linear-gradient(160deg, #7668ff 16%, #453cb0 84%);
        display: flex;
        align-items: center;
        justify-content: center;
        flex-shrink: 0;
    }

    .profile-avatar-text {
        display: block;
        color: #ffffff;
        font-size: 46px;
        font-weight: 800;
        line-height: 1;
        letter-spacing: -0.1em;
        transform: translate(-3px, -1px);
    }

    .profile-content {
        min-width: 0;
    }

    .profile-name {
        margin: 0 0 8px;
        font-size: 40px;
        font-weight: 750;
        line-height: 1.1;
        color: #fbfbff;
    }

    .profile-meta {
        display: flex;
        align-items: center;
        gap: 16px;
        flex-wrap: wrap;
        margin-bottom: 20px;
        color: #9d9aad;
        font-size: 20px;
        font-weight: 600;
    }

    .meta-item {
        display: inline-flex;
        align-items: center;
        gap: 6px;
        min-width: 0;
    }

    .meta-item span:last-child {
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
    }

    .meta-icon,
    .tab-icon,
    .star-icon,
    .button-icon,
    .note-file-symbol,
    .heart {
        display: inline-block;
        background-repeat: no-repeat;
        background-position: center;
        background-size: contain;
        flex-shrink: 0;
    }

    .meta-icon {
        width: 22px;
        height: 22px;
    }

    .graduate-icon {
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23786BFF" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"%3E%3Cpath d="M22 10L12 5 2 10l10 5 10-5Z"/%3E%3Cpath d="M6 12v5c0 1 3 3 6 3s6-2 6-3v-5"/%3E%3C/svg%3E');
    }

    .user-icon {
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23786BFF" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"%3E%3Cpath d="M20 21a8 8 0 1 0-16 0"/%3E%3Ccircle cx="12" cy="8" r="4"/%3E%3C/svg%3E');
    }

    .logout-button,
    .icon-button {
        border: none;
        background: transparent;
        color: #8e8ba3;
        cursor: pointer;
        transition: color 0.2s ease, transform 0.2s ease;
    }

    .logout-button {
        padding: 10px;
        margin-top: 4px;
    }

    .logout-button:hover,
    .icon-button:hover {
        color: #ffffff;
        transform: translateY(-1px);
    }

    .primary-button,
    .secondary-button,
    .ghost-button {
        border-radius: 11px;
        font-size: 24px;
        font-weight: 700;
        line-height: 1;
        cursor: pointer;
        transition: transform 0.2s ease, box-shadow 0.2s ease, background-color 0.2s ease, border-color 0.2s ease, color 0.2s ease;
    }

    .primary-button {
        border: none;
        background: linear-gradient(90deg, #7264ff 0%, #7e71ff 100%);
        color: #ffffff;
        box-shadow: 0 10px 24px rgba(103, 90, 255, 0.28);
    }

    .primary-button:hover {
        transform: translateY(-1px);
        box-shadow: 0 14px 28px rgba(103, 90, 255, 0.32);
    }

    .secondary-button,
    .ghost-button {
        border: 1px solid #2b2b37;
        background: #1b1b24;
        color: #ffffff;
    }

    .secondary-button:hover,
    .ghost-button:hover {
        background: #222230;
        border-color: #3a3a4d;
    }

    .edit-profile-button {
        min-height: 58px;
        padding: 0 24px;
        display: inline-flex;
        align-items: center;
        gap: 8px;
        font-size: 20px;
        box-shadow: none;
    }

    .edit-icon {
        width: 24px;
        height: 24px;
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23ffffff" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"%3E%3Cpath d="M12 20h9"/%3E%3Cpath d="M16.5 3.5a2.1 2.1 0 1 1 3 3L7 19l-4 1 1-4 12.5-12.5Z"/%3E%3C/svg%3E');
    }

    .tabs {
        display: grid;
        grid-template-columns: repeat(3, minmax(0, 1fr));
        align-items: center;
        border-radius: 20px;
        padding: 0 28px;
        margin-bottom: 18px;
        overflow-x: auto;
    }

    .tab-button {
        position: relative;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        gap: 12px;
        flex: 1 1 0;
        min-width: 0;
        padding: 24px 18px 22px;
        background: transparent;
        border: none;
        color: #8f8c9f;
        font-size: 24px;
        font-weight: 600;
        cursor: pointer;
        transition: color 0.2s ease;
    }

    .tab-label {
        white-space: nowrap;
    }

    .tab-button.active {
        color: #f5f5fc;
    }

    .tab-button.active::after {
        content: '';
        position: absolute;
        left: 18px;
        right: 18px;
        bottom: 0;
        height: 3px;
        border-radius: 999px;
        background: #786bff;
    }

    .tab-icon {
        width: 28px;
        height: 28px;
        opacity: 0.88;
    }

    .file-icon {
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23918da5" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"%3E%3Cpath d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/%3E%3Cpath d="M14 2v6h6"/%3E%3C/svg%3E');
    }

    .heart-icon {
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%237F8499" stroke-width="2"%3E%3Cpath d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/%3E%3C/svg%3E');
    }

    .heart-icon.active,
    .tab-button.active .heart-icon {
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23786BFF" stroke-width="2"%3E%3Cpath d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/%3E%3C/svg%3E');
    }

    .time-icon {
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23918da5" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"%3E%3Ccircle cx="12" cy="12" r="9"/%3E%3Cpath d="M12 7v5l3 2"/%3E%3C/svg%3E');
    }

    .time-icon.active,
    .tab-button.active .time-icon {
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23786BFF" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"%3E%3Ccircle cx="12" cy="12" r="9"/%3E%3Cpath d="M12 7v5l3 2"/%3E%3C/svg%3E');
    }

    .tab-button.active .file-icon {
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23786BFF" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"%3E%3Cpath d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/%3E%3Cpath d="M14 2v6h6"/%3E%3C/svg%3E');
    }

    .toolbar {
        margin-bottom: 16px;
    }

    .add-note-button {
        min-height: 54px;
        padding: 0 24px;
    }

    .notes-list {
        display: flex;
        flex-direction: column;
        gap: 22px;
    }

    .note-card {
        border-radius: 24px;
        padding: 30px 32px 24px;
    }

    .note-head {
        display: flex;
        align-items: flex-start;
        gap: 24px;
    }

    .note-file-icon {
        width: 76px;
        height: 76px;
        border-radius: 12px;
        background: rgba(120, 107, 255, 0.16);
        display: flex;
        align-items: center;
        justify-content: center;
        flex-shrink: 0;
    }

    .note-file-symbol {
        width: 42px;
        height: 42px;
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23786BFF" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"%3E%3Cpath d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/%3E%3Cpath d="M14 2v6h6"/%3E%3Cpath d="M9 13h6"/%3E%3Cpath d="M9 17h6"/%3E%3C/svg%3E');
    }

    .note-summary {
        min-width: 0;
        flex: 1;
    }

    .note-title-row {
        display: flex;
        justify-content: space-between;
        align-items: flex-start;
        gap: 12px;
    }

    .note-title-block {
        min-width: 0;
    }

    .note-title-side {
        min-width: 160px;
        display: flex;
        align-items: flex-start;
        justify-content: flex-end;
        flex-shrink: 0;
    }

    .note-title {
        margin: 0 0 4px;
        font-size: 28px;
        line-height: 1.15;
        font-weight: 760;
        color: #fbfbff;
    }

    .note-subtitle {
        margin: 0;
        color: #7c798d;
        font-size: 18px;
        font-weight: 600;
    }

    .status-badge {
        padding: 10px 18px;
        border-radius: 12px;
        font-size: 18px;
        font-weight: 700;
        white-space: nowrap;
    }

    .status-badge.approved {
        background: rgba(10, 160, 87, 0.22);
        color: #31d17a;
    }

    .status-badge.pending {
        background: rgba(170, 148, 14, 0.22);
        color: #d7cb33;
    }

    .status-badge.rejected {
        background: rgba(180, 43, 56, 0.24);
        color: #ff5f6b;
    }

    .favorite-indicator,
    .history-indicator {
        width: 36px;
        height: 36px;
        padding: 0;
    }

    .heart {
        width: 32px;
        height: 32px;
        transition: transform 0.2s ease;
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%237F8499" stroke-width="2"%3E%3Cpath d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/%3E%3C/svg%3E');
    }

    .icon-button:hover .heart {
        transform: translateY(-1px);
    }

    .heart.liked {
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="%23EF4444" stroke="%23EF4444" stroke-width="1"%3E%3Cpath d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/%3E%3C/svg%3E');
    }

    .note-meta {
        display: grid;
        gap: 6px;
        color: #c0bdd0;
        font-size: 18px;
        font-weight: 600;
        margin-top: 16px;
    }

    .download-date {
        display: inline-flex;
        align-items: center;
        gap: 8px;
        margin-top: 8px;
        color: #d7d4e9;
    }

    .rejection-reason {
        margin-top: 16px;
        padding: 14px 16px;
        border: 1px solid rgba(202, 68, 84, 0.8);
        border-radius: 10px;
        background: rgba(86, 18, 24, 0.45);
        color: #f0ced3;
        font-size: 16px;
        line-height: 1.45;
    }

    .rejection-title {
        color: #ff737d;
        font-weight: 700;
        margin-bottom: 2px;
    }

    .note-footer {
        margin-top: 18px;
        padding-top: 18px;
        border-top: 1px solid #2a2a36;
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 12px;
        flex-wrap: wrap;
    }

    .note-stats {
        display: flex;
        align-items: center;
        gap: 28px;
        flex-wrap: wrap;
    }

    .stat {
        display: inline-flex;
        align-items: center;
        gap: 6px;
        font-size: 18px;
        font-weight: 700;
        color: #b2afc2;
    }

    .star-icon {
        width: 28px;
        height: 28px;
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="%23FFD539"%3E%3Cpath d="M12 2.5l2.93 5.94 6.57.95-4.75 4.63 1.12 6.55L12 17.49 6.13 20.57l1.12-6.55L2.5 9.39l6.57-.95L12 2.5Z"/%3E%3C/svg%3E');
    }

    .rating {
        color: #f2f0f8;
    }

    .note-actions {
        display: flex;
        align-items: center;
        justify-content: flex-end;
        gap: 10px;
        margin-left: auto;
    }

    .delete-note-button {
        min-width: 156px;
        min-height: 52px;
        padding: 0 24px;
        background: #111118;
        border-color: #111118;
    }

    .action-button,
    .modal-button {
        min-width: 156px;
        min-height: 52px;
        padding: 0 24px;
    }

    .action-button {
        background: #6c63ff;
        box-shadow: none;
        transition: background-color 0.2s ease, color 0.2s ease, border-color 0.2s ease;
    }

    .action-button:hover {
        transform: none;
        background: #594FFE;
        box-shadow: none;
    }

    .modal-overlay {
        position: fixed;
        inset: 0;
        background: rgba(6, 6, 10, 0.54);
        backdrop-filter: blur(10px);
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 24px;
        z-index: 50;
    }

    .modal-card {
        width: 100%;
        max-width: 540px;
        border-radius: 26px;
        padding: 30px 30px 26px;
    }

    .modal-title {
        margin: 0 0 24px;
        font-size: 24px;
        font-weight: 760;
        color: #f6f5fd;
    }

    .form-group {
        margin-bottom: 16px;
    }

    .form-group label {
        display: block;
        margin-bottom: 10px;
        color: #9c99ac;
        font-size: 16px;
        font-weight: 600;
    }

    .form-group input,
    .select-trigger {
        width: 100%;
        min-height: 58px;
        border-radius: 14px;
        border: 1px solid #262632;
        background: #111118;
        color: #f2f1fa;
        font-size: 18px;
        font-weight: 600;
        padding: 0 18px;
        outline: none;
        transition: border-color 0.2s ease, box-shadow 0.2s ease;
    }

    .form-group input::placeholder {
        color: #68657b;
    }

    .form-group input:focus {
        border-color: #7468ff;
        box-shadow: 0 0 0 3px rgba(116, 104, 255, 0.12);
    }

    .custom-select {
        position: relative;
    }

    .select-trigger {
        padding: 0 18px;
        display: flex;
        align-items: center;
        justify-content: space-between;
        cursor: pointer;
        transition: border-color 0.2s ease, box-shadow 0.2s ease;
    }

    .custom-select.open .select-trigger,
    .select-trigger:hover {
        border-color: #7468ff;
        box-shadow: 0 0 0 3px rgba(116, 104, 255, 0.12);
    }

    .select-value {
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
    }

    .select-value.placeholder {
        color: #68657b;
    }

    .chevron-icon {
        width: 22px;
        height: 22px;
        flex-shrink: 0;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"%238784A0\" stroke-width=\"2.4\" stroke-linecap=\"round\" stroke-linejoin=\"round\"%3E%3Cpolyline points=\"6 9 12 15 18 9\"/%3E%3C/svg%3E') no-repeat center;
        background-size: 20px;
        transition: transform 0.2s ease;
    }

    .custom-select.open .chevron-icon {
        transform: rotate(180deg);
    }

    .select-dropdown {
        position: absolute;
        top: calc(100% + 4px);
        left: 0;
        right: 0;
        z-index: 30;
        max-height: 264px;
        overflow-y: auto;
        background: #171722;
        border: 1px solid #2A2A35;
        border-radius: 0 0 14px 14px;
        box-shadow: 0 10px 24px rgba(0, 0, 0, 0.3);
        scrollbar-width: thin;
        scrollbar-color: #8B7FFF rgba(255, 255, 255, 0.08);
    }

    .select-search {
        position: sticky;
        top: 0;
        z-index: 1;
        padding: 8px;
        background: #171722;
        border-bottom: 1px solid #2A2A35;
    }

    .select-search input {
        width: 100%;
        min-height: 42px;
        border-radius: 10px;
        border: 1px solid #2A2A35;
        background: #0F0F14;
        color: #FFFFFF;
        font-size: 18px;
        font-weight: 500;
        padding: 0 12px;
        outline: none;
    }

    .select-search input:focus {
        border-color: #7468ff;
        box-shadow: 0 0 0 1px rgba(116, 104, 255, 0.22);
    }

    .select-dropdown::-webkit-scrollbar {
        width: 12px;
    }

    .select-dropdown::-webkit-scrollbar-track {
        margin: 10px 0;
        background: rgba(255, 255, 255, 0.05);
        border-radius: 999px;
    }

    .select-dropdown::-webkit-scrollbar-thumb {
        background: linear-gradient(180deg, #9A8FFF 0%, #6C63FF 100%);
        border: 3px solid #171722;
        border-radius: 999px;
    }

    .select-option {
        width: 100%;
        min-height: 44px;
        padding: 10px 16px;
        border: none;
        background: transparent;
        color: #FFFFFF;
        text-align: left;
        font-size: 18px;
        font-weight: 500;
        cursor: pointer;
        transition: background 0.18s ease;
    }

    .select-option:hover,
    .select-option.selected {
        background: rgba(108, 99, 255, 0.16);
    }

    .input-with-icon {
        position: relative;
    }

    .input-with-icon input {
        padding-right: 48px;
    }

    .eye-button {
        position: absolute;
        top: 50%;
        right: 14px;
        width: 22px;
        height: 22px;
        transform: translateY(-50%);
        border: none;
        background-color: transparent;
        background-repeat: no-repeat;
        background-position: center;
        background-size: contain;
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%238784A0" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"%3E%3Cpath d="M1 12s4-7 11-7 11 7 11 7-4 7-11 7-11-7-11-7Z"/%3E%3Ccircle cx="12" cy="12" r="3"/%3E%3C/svg%3E');
        cursor: pointer;
    }

    .eye-button.off {
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%238784A0" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"%3E%3Cpath d="M17.94 17.94A10.94 10.94 0 0 1 12 19c-7 0-11-7-11-7a21.77 21.77 0 0 1 5.08-5.94"/%3E%3Cpath d="M9.9 4.24A10.73 10.73 0 0 1 12 4c7 0 11 8 11 8a21.2 21.2 0 0 1-2.17 3.19"/%3E%3Cpath d="M14.12 14.12A3 3 0 0 1 9.88 9.88"/%3E%3Cpath d="M3 3l18 18"/%3E%3C/svg%3E');
    }

    .error-message {
        margin-top: 8px;
        padding: 10px 12px;
        border-radius: 10px;
        background: rgba(165, 35, 45, 0.18);
        border: 1px solid rgba(213, 72, 84, 0.4);
        color: #ff9098;
        font-size: 13px;
        font-weight: 600;
    }

    .modal-actions {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 14px;
        margin-top: 20px;
    }

    .save-profile-button {
        box-shadow: none;
    }

    .save-profile-button:hover {
        box-shadow: 0 14px 28px rgba(103, 90, 255, 0.32);
    }

    .cancel-profile-button {
        background: #111118;
        border-color: #2A2A35;
    }

    .cancel-profile-button:hover {
        background: #171720;
        border-color: #3A3A4D;
    }

    .empty-state {
        border-radius: 18px;
        padding: 52px 24px;
        text-align: center;
    }

    .empty-state p {
        margin: 0;
        color: #a39fb3;
        font-size: 18px;
        font-weight: 600;
    }

    @media (max-width: 900px) {
        .page-shell {
            padding: 24px 20px 64px;
        }

        .topbar {
            gap: 14px;
            margin-bottom: 24px;
            align-items: center;
        }

        .back-button {
            transform: none;
        }

        .back-button:hover {
            transform: translateX(-2px);
        }

        .brand {
            gap: 14px;
        }

        .brand-mark {
            width: 50px;
            height: 50px;
        }

        .brand-mark svg {
            width: 39px;
            height: 39px;
        }

        .brand-text {
            font-size: 24px;
        }

        .profile-card {
            flex-direction: column;
            align-items: stretch;
        }

        .logout-button {
            align-self: flex-end;
        }
    }

    @media (max-width: 700px) {
        .profile-main,
        .note-head,
        .note-title-row,
        .note-footer {
            flex-direction: column;
            align-items: flex-start;
        }

        .profile-meta {
            flex-direction: column;
            align-items: flex-start;
            gap: 8px;
        }

        .tabs {
            display: flex;
            justify-content: flex-start;
            padding: 0 14px;
        }

        .tab-button {
            justify-content: flex-start;
            flex: 0 0 auto;
            min-width: fit-content;
            padding: 20px 28px 18px 0;
        }

        .tab-button.active::after {
            left: 0;
            right: 14px;
        }

        .note-title-side {
            width: 100%;
            min-width: 0;
            justify-content: flex-start;
        }

        .note-actions {
            width: 100%;
            justify-content: flex-start;
            margin-left: 0;
        }

        .modal-actions {
            grid-template-columns: 1fr;
        }
    }
</style>
