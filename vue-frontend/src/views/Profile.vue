<template>
    <div class="profile-page">
        <!-- Шапка с отступом 56px сверху -->
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
        </div>

        <!-- Карточка профиля -->
        <div class="profile-card">
            <div class="profile-header">
                <div class="avatar-large">
                    <div class="avatar-initials">{{ userInitials }}</div>
                </div>
                <div class="profile-info">
                    <h2 class="profile-name">{{ profile.fullName }}</h2>
                    <div class="profile-details">
                        <div class="detail-item">
                            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                <g clip-path="url(#clip0_university)">
                                    <path d="M24 9.41999C24.0005 9.09417 23.9147 8.77403 23.7514 8.49212C23.588 8.21021 23.353 7.97658 23.07 7.81499L12.96 1.81499C12.6687 1.645 12.3374 1.55542 12 1.55542C11.6627 1.55542 11.3314 1.645 11.04 1.81499L0.915025 7.81499C0.635233 7.98101 0.403453 8.21698 0.242466 8.49969C0.0814795 8.78241 -0.00317383 9.10215 -0.00317383 9.42749C-0.00317383 9.75283 0.0814795 10.0726 0.242466 10.3553C0.403453 10.638 0.635233 10.874 0.915025 11.04L3.79502 12.72V16.935C3.79676 17.2644 3.88322 17.5878 4.04609 17.8742C4.20896 18.1605 4.44277 18.4001 4.72503 18.57L11.1 22.245C11.3847 22.4026 11.7047 22.4853 12.03 22.4853C12.3554 22.4853 12.6754 22.4026 12.96 22.245L19.335 18.57C19.6173 18.4001 19.8511 18.1605 20.014 17.8742C20.1768 17.5878 20.2633 17.2644 20.265 16.935V12.675L22.125 11.58V15.66H24V9.41999ZM18.405 16.92L12 20.61L5.67002 16.935V13.83L11.04 17.025C11.3324 17.1921 11.6633 17.2799 12 17.2799C12.3368 17.2799 12.6677 17.1921 12.96 17.025L18.39 13.785L18.405 16.92ZM12 15.405L1.87502 9.40499L12 3.38999L22.125 9.38999L12 15.405Z" fill="#6C63FF" />
                                </g>
                                <defs>
                                    <clipPath id="clip0_university">
                                        <rect width="24" height="24" fill="white" />
                                    </clipPath>
                                </defs>
                            </svg>
                            <span>{{ profile.university }}</span>
                        </div>
                        <div class="detail-item">
                            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                <g clip-path="url(#clip0_email)">
                                    <path d="M24 9.41999C24.0005 9.09417 23.9147 8.77403 23.7514 8.49212C23.588 8.21021 23.353 7.97658 23.07 7.81499L12.96 1.81499C12.6687 1.645 12.3374 1.55542 12 1.55542C11.6627 1.55542 11.3314 1.645 11.04 1.81499L0.915025 7.81499C0.635233 7.98101 0.403453 8.21698 0.242466 8.49969C0.0814795 8.78241 -0.00317383 9.10215 -0.00317383 9.42749C-0.00317383 9.75283 0.0814795 10.0726 0.242466 10.3553C0.403453 10.638 0.635233 10.874 0.915025 11.04L3.79502 12.72V16.935C3.79676 17.2644 3.88322 17.5878 4.04609 17.8742C4.20896 18.1605 4.44277 18.4001 4.72503 18.57L11.1 22.245C11.3847 22.4026 11.7047 22.4853 12.03 22.4853C12.3554 22.4853 12.6754 22.4026 12.96 22.245L19.335 18.57C19.6173 18.4001 19.8511 18.1605 20.014 17.8742C20.1768 17.5878 20.2633 17.2644 20.265 16.935V12.675L22.125 11.58V15.66H24V9.41999ZM18.405 16.92L12 20.61L5.67002 16.935V13.83L11.04 17.025C11.3324 17.1921 11.6633 17.2799 12 17.2799C12.3368 17.2799 12.6677 17.1921 12.96 17.025L18.39 13.785L18.405 16.92ZM12 15.405L1.87502 9.40499L12 3.38999L22.125 9.38999L12 15.405Z" fill="#6C63FF" />
                                </g>
                                <defs>
                                    <clipPath id="clip0_email">
                                        <rect width="24" height="24" fill="white" />
                                    </clipPath>
                                </defs>
                            </svg>
                            <span>{{ profile.email }}</span>
                        </div>
                    </div>
                </div>
                <button class="logout-icon" @click="handleLogout">
                    <svg width="42" height="42" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" stroke="#7F8499" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
                        <polyline points="16 17 21 12 16 7" stroke="#7F8499" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
                        <line x1="21" y1="12" x2="9" y2="12" stroke="#7F8499" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
                    </svg>
                </button>
            </div>
            <button class="edit-profile-btn" @click="openEditModal">
                <svg width="28" height="28" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M17 3l4 4-7 7H10v-4l7-7z" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
                    <path d="M4 20h16" stroke="white" stroke-width="2" stroke-linecap="round" />
                </svg>
                Редактировать профиль
            </button>
        </div>

        <!-- Табы -->
        <div class="tabs">
            <div class="tab" :class="{ active: activeTab === 'myNotes' }" @click="activeTab = 'myNotes'">
                <div class="tab-icon file-icon"></div>
                <span>Мои конспекты</span>
                <div class="tab-indicator" v-if="activeTab === 'myNotes'"></div>
            </div>
            <div class="tab" :class="{ active: activeTab === 'favorites' }" @click="activeTab = 'favorites'">
                <div class="tab-icon heart-icon" :class="{ active: activeTab === 'favorites' }"></div>
                <span>Избранное</span>
                <div class="tab-indicator" v-if="activeTab === 'favorites'"></div>
            </div>
            <div class="tab" :class="{ active: activeTab === 'history' }" @click="activeTab = 'history'">
                <div class="tab-icon time-icon" :class="{ active: activeTab === 'history' }"></div>
                <span>История скачиваний</span>
                <div class="tab-indicator" v-if="activeTab === 'history'"></div>
            </div>
        </div>

        <!-- Кнопка добавить (только для моих конспектов) - по левому краю -->
        <div v-if="activeTab === 'myNotes'" class="add-note-btn-wrapper">
            <div class="add-note-btn" @click="goToAddNote">
                <span>+ Добавить</span>
            </div>
        </div>

        <!-- Список конспектов -->
        <div class="notes-list">
            <div v-for="note in currentNotes" :key="note.id" class="note-card">
                <div class="note-icon">
                    <div class="file-icon-large"></div>
                </div>
                <div class="note-info">
                    <h3>{{ note.title }}</h3>
                    <div class="note-subject">{{ note.subject }}</div>
                    <div class="note-meta">
                        <div>Преподаватель: {{ note.teacher }}</div>
                        <div>Вуз: {{ note.university }}</div>
                    </div>
                    <div class="note-stats">
                        <div class="rating">
                            <span class="star-small"></span>
                            <span>{{ note.rating }}</span>
                        </div>
                        <div class="downloads">{{ note.downloadsCount }} скачиваний</div>
                    </div>
                    <div v-if="activeTab === 'history' && note.downloadedAt" class="download-date">
                        Скачано: {{ formatDate(note.downloadedAt) }}
                    </div>
                </div>
                <div class="note-actions">
                    <template v-if="activeTab === 'myNotes'">
                        <div class="status-badge" :class="getStatusClass(note.status)">
                            {{ getStatusText(note.status) }}
                        </div>
                        <button class="open-btn" @click="openNote(note.id)">Открыть</button>
                        <button class="fix-btn" @click="editNote(note.id)">Исправить</button>
                        <button class="delete-btn" @click="deleteNote(note.id)">Удалить</button>
                    </template>
                    <template v-else>
                        <button class="open-btn" @click="openNote(note.id)">Открыть</button>
                    </template>
                </div>

                <!-- Блок с причиной отклонения (только для отклонённых конспектов) -->
                <div v-if="activeTab === 'myNotes' && getStatusClass(note.status) === 'rejected' && note.rejectionReason" class="rejection-reason">
                    <span class="rejection-label">Причина отклонения:</span>
                    <span class="rejection-text">{{ note.rejectionReason }}</span>
                </div>
            </div>
            <div v-if="currentNotes.length === 0" class="empty-state">
                <p>Нет конспектов</p>
            </div>
        </div>

        <!-- Модальное окно редактирования -->
        <div v-if="showEditModal" class="modal-overlay" @click.self="closeEditModal">
            <div class="modal-content edit-modal">
                <h2>Редактировать профиль</h2>
                <div class="form-group">
                    <label>Имя</label>
                    <input type="text" v-model="editForm.fullName" placeholder="Иван Иванов" />
                </div>
                <div class="form-group">
                    <label>Email</label>
                    <input type="email" v-model="editForm.email" placeholder="ivan@example.com" />
                </div>
                <div class="form-group">
                    <label>ВУЗ</label>
                    <select v-model="editForm.universityId">
                        <option v-for="uni in universities" :key="uni.id" :value="uni.id">{{ uni.name }}</option>
                    </select>
                </div>
                <div class="form-group">
                    <label>Новый пароль</label>
                    <div class="password-wrapper">
                        <input :type="showNewPassword ? 'text' : 'password'" v-model="editForm.newPassword" placeholder="Введите новый пароль" />
                        <button class="eye-icon-small" :class="{ off: !showNewPassword }" @click="showNewPassword = !showNewPassword"></button>
                    </div>
                </div>
                <div class="form-group">
                    <label>Повторите пароль</label>
                    <div class="password-wrapper">
                        <input :type="showConfirmPassword ? 'text' : 'password'" v-model="editForm.confirmPassword" placeholder="Повторите пароль" />
                        <button class="eye-icon-small" :class="{ off: !showConfirmPassword }" @click="showConfirmPassword = !showConfirmPassword"></button>
                    </div>
                </div>
                <div v-if="editError" class="error-message">{{ editError }}</div>
                <div class="modal-buttons">
                    <button class="cancel-btn" @click="closeEditModal">Отмена</button>
                    <button class="save-btn" @click="saveProfile">Сохранить</button>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
    import { ref, reactive, computed, onMounted } from 'vue'
    import { useRouter } from 'vue-router'
    import { useAuthStore } from '@/stores/auth'
    import { useFavoritesStore } from '@/stores/favorites'
    import api from '@/services/api'

    const router = useRouter()
    const authStore = useAuthStore()
    const favoritesStore = useFavoritesStore()

    const profile = reactive({
        fullName: '',
        email: '',
        university: ''
    })

    // Инициалы для аватара
    const userInitials = computed(() => {
        if (!profile.fullName) return ''
        const nameParts = profile.fullName.trim().split(' ')
        if (nameParts.length >= 2) {
            return (nameParts[0][0] + nameParts[1][0]).toUpperCase()
        }
        return profile.fullName[0]?.toUpperCase() || ''
    })

    const universities = ref([])
    const activeTab = ref('myNotes')
    const myNotes = ref([])
    const favorites = ref([])
    const downloadHistory = ref([])

    const currentNotes = computed(() => {
        switch (activeTab.value) {
            case 'myNotes': return myNotes.value
            case 'favorites': return favorites.value
            case 'history': return downloadHistory.value
            default: return []
        }
    })

    const showEditModal = ref(false)
    const editForm = reactive({
        fullName: '',
        email: '',
        universityId: null,
        newPassword: '',
        confirmPassword: ''
    })
    const showNewPassword = ref(false)
    const showConfirmPassword = ref(false)
    const editError = ref('')

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
        if (confirm('Вы уверены, что хотите удалить этот конспект?')) {
            try {
                await api.delete(`/notes/${id}`)
                myNotes.value = myNotes.value.filter(n => n.id !== id)
            } catch (error) {
                console.error('Ошибка удаления:', error)
                alert('Не удалось удалить конспект')
            }
        }
    }

    const getStatusText = (status) => {
        if (!status) return ''
        let statusStr = typeof status === 'string' ? status : (status?.name || status?.code || '')
        statusStr = String(statusStr).toLowerCase()
        switch (statusStr) {
            case 'одобрено':
            case 'approved':
                return 'Опубликован'
            case 'отклонено':
            case 'rejected':
                return 'Отклонён'
            case 'pending':
            case 'на проверке':
                return 'На проверке'
            default:
                return statusStr
        }
    }

    const getStatusClass = (status) => {
        if (!status) return ''
        let statusStr = typeof status === 'string' ? status : (status?.name || status?.code || '')
        statusStr = String(statusStr).toLowerCase()
        switch (statusStr) {
            case 'одобрено':
            case 'approved':
                return 'approved'
            case 'отклонено':
            case 'rejected':
                return 'rejected'
            case 'pending':
            case 'на проверке':
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
            myNotes.value = response.data
        } catch (error) {
            console.error('Ошибка загрузки конспектов:', error)
        }
    }

    const loadFavorites = async () => {
        await favoritesStore.loadFavorites()
        favorites.value = favoritesStore.favorites
    }

    const loadDownloadHistory = () => {
        const history = JSON.parse(localStorage.getItem('downloadHistory') || '[]')
        downloadHistory.value = history
    }

    const openEditModal = () => {
        editForm.fullName = profile.fullName
        editForm.email = profile.email
        editForm.universityId = null
        editForm.newPassword = ''
        editForm.confirmPassword = ''
        editError.value = ''
        showEditModal.value = true
    }

    const closeEditModal = () => {
        showEditModal.value = false
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
            if (editForm.universityId) {
                const selectedUni = universities.value.find(u => u.id === editForm.universityId)
                if (selectedUni) profile.university = selectedUni.name
            }

            closeEditModal()
        } catch (error) {
            editError.value = error.response?.data?.message || 'Ошибка сохранения'
        }
    }

    onMounted(() => {
        loadProfile()
        loadUniversities()
        loadMyNotes()
        loadFavorites()
        loadDownloadHistory()
    })
</script>

<style scoped>
    .profile-page {
        min-height: 100vh;
        background: #0F0F14;
        font-family: 'Inter', system-ui, -apple-system, sans-serif;
        display: flex;
        flex-direction: column;
        align-items: center;
    }

    /* Шапка - отступ 56px сверху */
    .header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        width: 100%;
        max-width: 1440px;
        margin: 0 auto;
        padding: 56px 80px 0 80px;
        margin-bottom: 0;
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

    /* Карточка профиля */
    .profile-card {
        width: 100%;
        max-width: 1080px;
        margin: 24px auto 0;
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 20px;
        padding: 24px;
        position: relative;
    }

    .profile-header {
        display: flex;
        align-items: center;
        gap: 24px;
        position: relative;
    }

    .avatar-large {
        width: 96px;
        height: 96px;
        border-radius: 50%;
        overflow: hidden;
        flex-shrink: 0;
        background: linear-gradient(160.69deg, #6C63FF 19.22%, #413B99 78.54%);
        display: flex;
        align-items: center;
        justify-content: center;
    }

    .avatar-initials {
        font-size: 36px;
        font-weight: 700;
        color: #FFFFFF;
        text-transform: uppercase;
    }

    .profile-info {
        flex: 1;
    }

    .profile-name {
        font-size: 32px;
        font-weight: 700;
        color: #FFFFFF;
        margin: 0 0 16px 0;
    }

    .profile-details {
        display: flex;
        gap: 32px;
        flex-wrap: wrap;
    }

    .detail-item {
        display: flex;
        align-items: center;
        gap: 8px;
        font-size: 20px;
        font-weight: 700;
        color: #A0A0B0;
    }

    .logout-icon {
        background: none;
        border: none;
        cursor: pointer;
        position: absolute;
        top: 0;
        right: 0;
        padding: 0;
    }

    .edit-profile-btn {
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 12px;
        background: #6C63FF;
        border: none;
        border-radius: 14px;
        padding: 12px 32px;
        margin-top: 20px;
        font-size: 24px;
        font-weight: 600;
        color: #FFFFFF;
        cursor: pointer;
        transition: all 0.2s;
        width: fit-content;
        min-width: 320px;
    }

        .edit-profile-btn:hover {
            background: #594FFE;
            transform: translateY(-2px);
            box-shadow: 0px 8px 16px rgba(108, 99, 255, 0.45);
        }

    /* Табы */
    .tabs {
        width: 100%;
        max-width: 1080px;
        margin: 24px auto 0;
        display: flex;
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 20px;
        position: relative;
    }

    .tab {
        flex: 1;
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 12px;
        padding: 18px;
        cursor: pointer;
        position: relative;
        transition: all 0.2s;
    }

        .tab span {
            font-size: 24px;
            font-weight: 500;
            color: #A0A0B0;
            transition: color 0.2s;
        }

        .tab.active span {
            color: #FFFFFF;
        }

    .tab-indicator {
        position: absolute;
        bottom: 0;
        left: 0;
        right: 0;
        height: 2px;
        background: #6C63FF;
    }

    .tab-icon {
        width: 32px;
        height: 32px;
    }

    .file-icon {
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%237F8499" stroke-width="2"%3E%3Cpath d="M13 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9z"/%3E%3Cpolyline points="13 2 13 9 20 9"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .heart-icon {
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23A0A0B0" stroke-width="2"%3E%3Cpath d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

        .heart-icon.active {
            background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="%236C63FF" stroke="%236C63FF" stroke-width="1"%3E%3Cpath d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/%3E%3C/svg%3E') no-repeat center;
            background-size: contain;
        }

    .time-icon {
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23A0A0B0" stroke-width="2"%3E%3Ccircle cx="12" cy="12" r="10"/%3E%3Cpolyline points="12 6 12 12 16 14"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

        .time-icon.active {
            background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%236C63FF" stroke-width="2"%3E%3Ccircle cx="12" cy="12" r="10"/%3E%3Cpolyline points="12 6 12 12 16 14"/%3E%3C/svg%3E') no-repeat center;
            background-size: contain;
        }

    /* Кнопка добавить - по левому краю */
    .add-note-btn-wrapper {
        width: 100%;
        max-width: 1080px;
        margin: 24px auto 0;
    }

    .add-note-btn {
        display: inline-block;
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 14px;
        padding: 12px 24px;
        cursor: pointer;
        transition: all 0.2s;
    }

        .add-note-btn span {
            font-size: 24px;
            font-weight: 600;
            color: #FFFFFF;
        }

        .add-note-btn:hover {
            background: #2A2A35;
        }

    /* Список конспектов */
    .notes-list {
        width: 100%;
        max-width: 1080px;
        margin: 24px auto 0;
        display: flex;
        flex-direction: column;
        gap: 24px;
        padding-bottom: 40px;
    }

    .note-card {
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 20px;
        padding: 24px;
        display: flex;
        gap: 24px;
        position: relative;
        flex-wrap: wrap;
    }

    .note-icon {
        width: 62px;
        height: 66px;
        background: #2A2348;
        border-radius: 10px;
        display: flex;
        align-items: center;
        justify-content: center;
        flex-shrink: 0;
    }

    .file-icon-large {
        width: 45px;
        height: 50px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%236C63FF" stroke-width="2"%3E%3Cpath d="M13 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9z"/%3E%3Cpolyline points="13 2 13 9 20 9"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .note-info {
        flex: 2;
        min-width: 200px;
    }

        .note-info h3 {
            font-size: 26px;
            font-weight: 700;
            color: #FFFFFF;
            margin: 0 0 8px 0;
        }

    .note-subject {
        font-size: 18px;
        font-weight: 700;
        color: #A0A0B0;
        margin-bottom: 12px;
    }

    .note-meta {
        font-size: 18px;
        font-weight: 700;
        color: #9A9A9F;
        line-height: 30px;
        margin-bottom: 12px;
    }

    .note-stats {
        display: flex;
        align-items: center;
        gap: 24px;
        margin-top: 8px;
    }

    .rating {
        display: flex;
        align-items: center;
        gap: 8px;
    }

    .star-small {
        width: 40px;
        height: 40px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="%23FFE100"%3E%3Cpolygon points="12 2 15 9 22 9 16 14 18 21 12 17 6 21 8 14 2 9 9 9 12 2"/%3E%3C/svg%3E') no-repeat center;
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

    .download-date {
        font-size: 16px;
        font-weight: 500;
        color: #DFDFE4;
        margin-top: 8px;
    }

    .note-actions {
        display: flex;
        flex-direction: column;
        align-items: flex-end;
        gap: 12px;
        margin-left: auto;
    }

    /* Стили статусов - с !important для гарантии */
    .status-badge {
        padding: 4px 12px !important;
        border-radius: 10px !important;
        font-size: 20px !important;
        font-weight: 500 !important;
        text-align: center !important;
        min-width: 132px !important;
        height: 32px !important;
        display: flex !important;
        align-items: center !important;
        justify-content: center !important;
    }

        .status-badge.approved {
            background: rgba(34, 197, 94, 0.18) !important;
            color: #22C55E !important;
        }

        .status-badge.rejected {
            background: rgba(239, 68, 68, 0.2) !important;
            color: #EF4444 !important;
        }

        .status-badge.pending {
            background: rgba(255, 193, 7, 0.2) !important;
            color: #FFC107 !important;
        }

    .open-btn, .delete-btn, .fix-btn {
        padding: 14px 24px;
        border-radius: 14px;
        font-size: 20px;
        font-weight: 600;
        cursor: pointer;
        transition: all 0.2s;
        width: 157px;
        text-align: center;
    }

    .open-btn {
        background: #6C63FF;
        border: none;
        color: #FFFFFF;
    }

        .open-btn:hover {
            background: #594FFE;
            transform: translateY(-2px);
        }

    .delete-btn {
        background: #0F0F14;
        border: 1px solid #151516;
        color: #FFFFFF;
    }

        .delete-btn:hover {
            border-color: #EF4444;
            color: #EF4444;
        }

    .fix-btn {
        background: #6C63FF;
        border: none;
        color: #FFFFFF;
    }

        .fix-btn:hover {
            background: #594FFE;
            transform: translateY(-2px);
        }

    .rejection-reason {
        width: 100%;
        margin-top: 16px;
        padding: 16px 20px;
        background: rgba(42, 15, 18, 0.7);
        border: 1px solid #EF4444;
        border-radius: 12px;
        display: flex;
        flex-direction: column;
        gap: 4px;
    }

    .rejection-label {
        font-size: 20px;
        font-weight: 700;
        color: #EF4444;
    }

    .rejection-text {
        font-size: 20px;
        font-weight: 500;
        color: #F87171;
        line-height: 24px;
    }

        .rejection-reason span {
            font-size: 20px;
            font-weight: 500;
            color: #F87171;
        }

    .empty-state {
        text-align: center;
        padding: 60px 20px;
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 20px;
    }

        .empty-state p {
            font-size: 24px;
            color: #A0A0B0;
        }

    /* Модальное окно */
    .modal-overlay {
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(0, 0, 0, 0.5);
        backdrop-filter: blur(6px);
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 200;
    }

    .edit-modal {
        width: 600px;
        max-width: 90%;
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 20px;
        padding: 32px;
    }

        .edit-modal h2 {
            font-size: 32px;
            font-weight: 600;
            color: #FFFFFF;
            margin-bottom: 24px;
            text-align: center;
        }

        .edit-modal .form-group {
            margin-bottom: 20px;
        }

            .edit-modal .form-group label {
                display: block;
                font-size: 20px;
                font-weight: 500;
                color: #A0A0B0;
                margin-bottom: 8px;
            }

            .edit-modal .form-group input,
            .edit-modal .form-group select {
                width: 100%;
                padding: 14px 16px;
                background: #0F0F14;
                border: 1px solid #2A2A35;
                border-radius: 14px;
                font-size: 20px;
                color: #FFFFFF;
                font-family: 'Inter', sans-serif;
            }

                .edit-modal .form-group input:focus,
                .edit-modal .form-group select:focus {
                    outline: none;
                    border-color: #6C63FF;
                }

    .password-wrapper {
        position: relative;
    }

        .password-wrapper input {
            width: 100%;
            padding-right: 48px;
        }

    .eye-icon-small {
        position: absolute;
        right: 16px;
        top: 50%;
        transform: translateY(-50%);
        width: 24px;
        height: 24px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%237F8499" stroke-width="2"%3E%3Cpath d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/%3E%3Ccircle cx="12" cy="12" r="3"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
        border: none;
        cursor: pointer;
        background-color: transparent;
    }

        .eye-icon-small.off {
            background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%237F8499" stroke-width="2"%3E%3Cpath d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"/%3E%3Cline x1="1" y1="1" x2="23" y2="23"/%3E%3C/svg%3E') no-repeat center;
            background-size: contain;
        }

    .error-message {
        background: rgba(239, 68, 68, 0.1);
        color: #EF4444;
        padding: 12px;
        border-radius: 14px;
        margin: 16px 0;
        text-align: center;
        font-size: 16px;
    }

    .modal-buttons {
        display: flex;
        gap: 16px;
        margin-top: 24px;
    }

    .cancel-btn, .save-btn {
        flex: 1;
        padding: 14px;
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

    .save-btn {
        background: #6C63FF;
        border: none;
        color: #FFFFFF;
    }

        .save-btn:hover {
            background: #594FFE;
            transform: translateY(-2px);
            box-shadow: 0px 8px 16px rgba(108, 99, 255, 0.45);
        }

    /* Адаптив */
    @media (max-width: 1200px) {
        .header {
            padding: 56px 40px 0 40px;
        }

        .profile-card,
        .tabs,
        .notes-list,
        .add-note-btn-wrapper {
            width: calc(100% - 80px);
        }
    }

    @media (max-width: 900px) {
        .header {
            padding: 40px 16px 0 16px;
        }

        .profile-card,
        .tabs,
        .notes-list,
        .add-note-btn-wrapper {
            width: calc(100% - 32px);
        }

        .profile-header {
            flex-direction: column;
            text-align: center;
        }

        .profile-details {
            flex-direction: column;
            align-items: center;
            gap: 12px;
        }

        .edit-profile-btn {
            width: 100%;
            justify-content: center;
            min-width: auto;
        }

        .logout-icon {
            position: static;
            margin-top: 16px;
        }

        .tabs {
            flex-direction: column;
        }

        .tab {
            justify-content: flex-start;
            padding: 12px 20px;
        }

        .tab-indicator {
            display: none;
        }

        .tab.active {
            border-bottom: 2px solid #6C63FF;
        }

        .note-card {
            flex-direction: column;
        }

        .note-actions {
            align-items: flex-start;
            margin-left: 0;
        }

        .open-btn, .delete-btn, .fix-btn {
            width: 100%;
        }

        .edit-modal {
            width: 95%;
            padding: 20px;
        }

            .edit-modal h2 {
                font-size: 24px;
            }

        .modal-buttons {
            flex-direction: column;
        }
    }
</style>