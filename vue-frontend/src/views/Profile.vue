<template>
    <div class="profile-page">
        <div class="header">
            <div class="logo-area" @click="goToCatalog">
                <div class="logo-icon">
                    <svg width="55" height="50" viewBox="0 0 69 69" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M34.5 20.125C34.5 17.075 33.2884 14.1499 31.1317 11.9933C28.9751 9.8366 26.05 8.625 23 8.625H5.75V51.75H25.875C28.1625 51.75 30.3563 52.6587 31.9738 54.2762C33.5913 55.8937 34.5 58.0875 34.5 60.375M34.5 20.125V60.375M34.5 20.125C34.5 17.075 35.7116 14.1499 37.8683 11.9933C40.0249 9.8366 42.95 8.625 46 8.625H63.25V51.75H43.125C40.8375 51.75 38.6437 52.6587 37.0262 54.2762C35.4087 55.8937 34.5 58.0875 34.5 60.375" stroke="#8B7FFF" stroke-width="4" stroke-linecap="round" stroke-linejoin="round" />
                    </svg>
                </div>
                <span class="logo-text">Каталог конспектов</span>
            </div>

        </div>

        <div class="back-button" @click="goBack">
            <svg width="49" height="49" viewBox="0 0 49 49" fill="none" xmlns="http://www.w3.org/2000/svg">
                <circle cx="24.5" cy="24.5" r="23" stroke="#A0A0B0" stroke-width="2.5" />
                <polyline points="27 17 18 24.5 27 32" stroke="#A0A0B0" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" />
            </svg>
        </div>

        <div class="profile-card">
            <div class="profile-header">
                <div class="avatar-large">
                    <div class="avatar-gradient"></div>
                </div>
                <div class="profile-info">
                    <h2 class="profile-name">{{ profile.fullName }}</h2>
                    <div class="profile-details">
                        <div class="detail-item">
                            <div class="uni-icon"></div>
                            <span>{{ profile.university }}</span>
                        </div>
                        <div class="detail-item">
                            <div class="email-icon"></div>
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

        <div class="tabs">
            <div class="tab" :class="{ active: activeTab === 'myNotes' }" @click="activeTab = 'myNotes'">
                <div class="tab-icon file-icon"></div>
                <span>Мои конспекты</span>
            </div>
            <div class="tab" :class="{ active: activeTab === 'favorites' }" @click="activeTab = 'favorites'">
                <div class="tab-icon heart-icon" :class="{ active: activeTab === 'favorites' }"></div>
                <span>Избранное</span>
            </div>
            <div class="tab" :class="{ active: activeTab === 'history' }" @click="activeTab = 'history'">
                <div class="tab-icon time-icon" :class="{ active: activeTab === 'history' }"></div>
                <span>История скачиваний</span>
            </div>
        </div>

        <div v-if="activeTab === 'myNotes'" class="add-note-btn" @click="goToAddNote">
            <span>+ Добавить</span>
        </div>

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
                        <button class="edit-btn" @click="editNote(note.id)">Редактировать</button>
                        <button class="delete-btn" @click="deleteNote(note.id)">Удалить</button>
                    </template>
                    <template v-else>
                        <button class="open-btn" @click="openNote(note.id)">Открыть</button>
                    </template>
                </div>
                <div v-if="activeTab === 'myNotes' && note.status === 'rejected' && note.rejectionReason" class="rejection-reason">
                    <span>Причина отклонения: {{ note.rejectionReason }}</span>
                </div>
            </div>
            <div v-if="currentNotes.length === 0" class="empty-state">
                <p>Нет конспектов</p>
            </div>
        </div>

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
            case 'approved': return 'Опубликован'
            case 'rejected': return 'Отклонён'
            case 'pending': return 'На проверке'
            default: return statusStr
        }
    }

    const getStatusClass = (status) => {
        if (!status) return ''
        let statusStr = typeof status === 'string' ? status : (status?.name || status?.code || '')
        statusStr = String(statusStr).toLowerCase()
        switch (statusStr) {
            case 'approved': return 'approved'
            case 'rejected': return 'rejected'
            case 'pending': return 'pending'
            default: return ''
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
        background: #0A0A0F;
        padding: 32px 80px;
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
    }

    .profile-card {
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 20px;
        padding: 24px;
        margin-bottom: 32px;
        position: relative;
    }

    .profile-header {
        display: flex;
        align-items: center;
        gap: 24px;
    }

    .avatar-large {
        width: 96px;
        height: 96px;
        border-radius: 50%;
        overflow: hidden;
    }

    .avatar-gradient {
        width: 100%;
        height: 100%;
        background: linear-gradient(160.69deg, #6C63FF 19.22%, #413B99 78.54%);
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
    }

    .detail-item {
        display: flex;
        align-items: center;
        gap: 8px;
        font-size: 20px;
        font-weight: 700;
        color: #A0A0B0;
    }

    .uni-icon {
        width: 24px;
        height: 24px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%236C63FF" stroke-width="2"%3E%3Cpath d="M12 3L2 9l10 6 10-6-10-6zM2 9v6l10 6 10-6V9"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .email-icon {
        width: 24px;
        height: 24px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%236C63FF" stroke-width="2"%3E%3Crect x="2" y="4" width="20" height="16" rx="2"/%3E%3Cpath d="m22 7-10 7L2 7"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .logout-icon {
        background: none;
        border: none;
        cursor: pointer;
        position: absolute;
        top: 24px;
        right: 24px;
    }

    .edit-profile-btn {
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 8px;
        background: #6C63FF;
        border: none;
        border-radius: 14px;
        padding: 12px 24px;
        font-size: 24px;
        font-weight: 600;
        color: #FFFFFF;
        cursor: pointer;
        transition: all 0.2s;
        margin-top: 20px;
        width: fit-content;
    }

        .edit-profile-btn:hover {
            background: #594FFE;
            transform: translateY(-2px);
            box-shadow: 0px 8px 16px rgba(108, 99, 255, 0.45);
        }

    .tabs {
        display: flex;
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 20px;
        margin-bottom: 24px;
    }

    .tab {
        flex: 1;
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 12px;
        padding: 18px;
        cursor: pointer;
        transition: all 0.2s;
    }

        .tab.active {
            border-bottom: 2px solid #6C63FF;
        }

        .tab span {
            font-size: 24px;
            font-weight: 500;
            color: #A0A0B0;
        }

        .tab.active span {
            color: #FFFFFF;
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
            stroke: #6C63FF;
        }

    .add-note-btn {
        display: inline-block;
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 14px;
        padding: 12px 24px;
        margin-bottom: 24px;
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

    .notes-list {
        display: flex;
        flex-direction: column;
        gap: 24px;
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
    }

    .file-icon-large {
        width: 45px;
        height: 50px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%236C63FF" stroke-width="2"%3E%3Cpath d="M13 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9z"/%3E%3Cpolyline points="13 2 13 9 20 9"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .note-info {
        flex: 2;
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
        line-height: 1.5;
        margin-bottom: 12px;
    }

    .note-stats {
        display: flex;
        align-items: center;
        gap: 24px;
    }

    .rating {
        display: flex;
        align-items: center;
        gap: 6px;
    }

    .star-small {
        width: 20px;
        height: 20px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="%23FFE100"%3E%3Cpolygon points="12 2 15 9 22 9 16 14 18 21 12 17 6 21 8 14 2 9 9 9 12 2"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .rating span, .downloads {
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
    }

    .status-badge {
        padding: 6px 12px;
        border-radius: 10px;
        font-size: 20px;
        font-weight: 500;
    }

        .status-badge.approved {
            background: rgba(34, 197, 94, 0.2);
            color: #00FF5E;
        }

        .status-badge.rejected {
            background: rgba(239, 68, 68, 0.2);
            color: #FF0000;
        }

        .status-badge.pending {
            background: rgba(255, 193, 7, 0.2);
            color: #FFC107;
        }

    .open-btn, .edit-btn, .delete-btn {
        padding: 10px 24px;
        border-radius: 14px;
        font-size: 20px;
        font-weight: 600;
        cursor: pointer;
        transition: all 0.2s;
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

    .edit-btn {
        background: #1A1A22;
        border: 1px solid #6C63FF;
        color: #6C63FF;
    }

        .edit-btn:hover {
            background: #2A2A35;
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

    .rejection-reason {
        width: 100%;
        margin-top: 16px;
        padding-top: 16px;
        border-top: 1px solid #2A2A35;
    }

        .rejection-reason span {
            font-size: 18px;
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

    .edit-modal .error-message {
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

    @media (max-width: 1200px) {
        .profile-page {
            padding: 20px 40px;
        }
    }

    @media (max-width: 900px) {
        .profile-page {
            padding: 16px;
        }

        .profile-header {
            flex-direction: column;
            text-align: center;
        }

        .profile-details {
            flex-direction: column;
            gap: 12px;
            align-items: center;
        }

        .edit-profile-btn {
            width: 100%;
            justify-content: center;
        }

        .tabs {
            flex-direction: column;
        }

        .tab {
            justify-content: flex-start;
            padding: 12px 20px;
        }

        .note-card {
            flex-direction: column;
        }

        .note-actions {
            align-items: flex-start;
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
