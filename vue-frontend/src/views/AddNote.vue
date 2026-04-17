<template>
    <div class="add-note-page">
        <!-- Шапка -->
        <div class="header">
            <div class="logo-area" @click="goToCatalog">
                <div class="logo-icon">
                    <svg width="55" height="50" viewBox="0 0 69 69" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M34.5 20.125C34.5 17.075 33.2884 14.1499 31.1317 11.9933C28.9751 9.8366 26.05 8.625 23 8.625H5.75V51.75H25.875C28.1625 51.75 30.3563 52.6587 31.9738 54.2762C33.5913 55.8937 34.5 58.0875 34.5 60.375M34.5 20.125V60.375M34.5 20.125C34.5 17.075 35.7116 14.1499 37.8683 11.9933C40.0249 9.8366 42.95 8.625 46 8.625H63.25V51.75H43.125C40.8375 51.75 38.6437 52.6587 37.0262 54.2762C35.4087 55.8937 34.5 58.0875 34.5 60.375" stroke="#8B7FFF" stroke-width="4" stroke-linecap="round" stroke-linejoin="round" />
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

        <!-- Заголовок -->
        <h1 class="page-title">{{ isEditMode ? 'Редактирование конспекта' : 'Загрузка конспекта' }}</h1>
        <p class="page-subtitle">{{ isEditMode ? 'Измените данные конспекта' : 'Добавьте новый конспект для публикации на платформе' }}</p>

        <!-- Основная форма -->
        <div class="form-container">
            <div class="main-form">
                <!-- Название -->
                <div class="form-group">
                    <label>Название конспекта *</label>
                    <div class="input-wrapper" :class="{ error: errors.title }">
                        <input type="text" v-model="form.title" placeholder="Введение в базы данных" @input="clearError('title')" />
                    </div>
                    <div class="error-message" v-if="errors.title">{{ errors.title }}</div>
                </div>

                <!-- Описание -->
                <div class="form-group">
                    <label>Описание *</label>
                    <div class="input-wrapper" :class="{ error: errors.description }">
                        <textarea v-model="form.description" placeholder="Опишите содержание конспекта, основные темы и разделы..." rows="8" @input="clearError('description')"></textarea>
                    </div>
                    <div class="error-message" v-if="errors.description">{{ errors.description }}</div>
                </div>

                <!-- Предмет -->
                <div class="form-group">
                    <label>Предмет *</label>
                    <div class="custom-select" :class="{ open: isSubjectOpen, error: errors.subjectId }">
                        <div class="select-trigger" @click="toggleSubjectDropdown">
                            <span class="select-value">{{ selectedSubjectName || 'Выберите предмет' }}</span>
                            <span class="chevron-icon"></span>
                        </div>
                        <div v-if="isSubjectOpen" class="select-dropdown">
                            <div v-for="subject in subjects" :key="subject.id" class="select-option" :class="{ selected: form.subjectId === subject.id }" @click="selectSubject(subject)">
                                {{ subject.name }}
                            </div>
                        </div>
                    </div>
                    <div class="error-message" v-if="errors.subjectId">{{ errors.subjectId }}</div>
                </div>

                <!-- Преподаватель -->
                <div class="form-group">
                    <label>Преподаватель *</label>
                    <div class="custom-select" :class="{ open: isTeacherOpen, error: errors.teacherId }">
                        <div class="select-trigger" @click="toggleTeacherDropdown">
                            <span class="select-value">{{ selectedTeacherName || 'Выберите преподавателя' }}</span>
                            <span class="chevron-icon"></span>
                        </div>
                        <div v-if="isTeacherOpen" class="select-dropdown">
                            <div v-for="teacher in teachers" :key="teacher.id" class="select-option" :class="{ selected: form.teacherId === teacher.id }" @click="selectTeacher(teacher)">
                                {{ teacher.fullName }}
                            </div>
                        </div>
                    </div>
                    <div class="error-message" v-if="errors.teacherId">{{ errors.teacherId }}</div>
                </div>

                <!-- ВУЗ -->
                <div class="form-group">
                    <label>ВУЗ *</label>
                    <div class="custom-select" :class="{ open: isUniversityOpen, error: errors.universityId }">
                        <div class="select-trigger" @click="toggleUniversityDropdown">
                            <span class="select-value">{{ selectedUniversityName || 'Выберите ВУЗ' }}</span>
                            <span class="chevron-icon"></span>
                        </div>
                        <div v-if="isUniversityOpen" class="select-dropdown">
                            <div v-for="university in universities" :key="university.id" class="select-option" :class="{ selected: form.universityId === university.id }" @click="selectUniversity(university)">
                                {{ university.name }}
                            </div>
                        </div>
                    </div>
                    <div class="error-message" v-if="errors.universityId">{{ errors.universityId }}</div>
                </div>

                <!-- Загрузка файла (только в режиме добавления) -->
                <div class="form-group" v-if="!isEditMode">
                    <label>Загрузка файла *</label>
                    <div class="drop-zone" :class="{ dragOver: isDragOver, error: errors.file }" @dragover.prevent="isDragOver = true" @dragleave.prevent="isDragOver = false" @drop.prevent="handleDrop" @click="triggerFileInput">
                        <div class="drop-zone-icon"></div>
                        <p>Перетащите файл сюда или выберите с устройства</p>
                        <span>Поддерживаются только PDF файлы</span>
                        <input type="file" ref="fileInput" accept=".pdf" @change="handleFileSelect" style="display: none" />
                    </div>
                    <div class="error-message" v-if="errors.file">{{ errors.file }}</div>

                    <div v-if="selectedFile" class="selected-file">
                        <div class="file-info">
                            <div class="file-icon-small"></div>
                            <div class="file-details">
                                <div class="file-name">{{ selectedFile.name }}</div>
                                <div class="file-size">{{ formatFileSize(selectedFile.size) }}</div>
                            </div>
                        </div>
                        <button class="remove-file" @click="removeFile">
                            <div class="close-icon"></div>
                        </button>
                    </div>
                </div>

                <!-- Текущий файл (в режиме редактирования) -->
                <div class="form-group" v-if="isEditMode && note.filePath">
                    <label>Текущий файл</label>
                    <div class="current-file">
                        <div class="file-icon-small"></div>
                        <span>{{ getFileName(note.filePath) }}</span>
                        <a :href="note.filePath" target="_blank" class="view-file-link">Посмотреть</a>
                    </div>
                </div>

                <!-- Кнопки -->
                <div class="form-buttons">
                    <button class="cancel-btn" @click="goBack">Отменить</button>
                    <button class="submit-btn" @click="submitNote" :disabled="isSubmitting">
                        {{ isSubmitting ? 'Сохранение...' : (isEditMode ? 'Сохранить изменения' : 'Отправить на модерацию') }}
                    </button>
                </div>
            </div>

            <!-- Правая панель -->
            <div class="info-panel">
                <div class="info-card">
                    <div class="info-header">
                        <div class="info-icon requirements-icon"></div>
                        <h3>Требования</h3>
                    </div>
                    <ul class="requirements-list">
                        <li><span class="check-icon"></span>Только PDF формат</li>
                        <li><span class="check-icon"></span>Корректное и понятное название</li>
                        <li><span class="check-icon"></span>Подробное описание содержания</li>
                        <li><span class="check-icon"></span>Качественный и читаемый материал</li>
                    </ul>
                </div>

                <div class="info-card">
                    <div class="info-header">
                        <div class="info-icon moderation-icon"></div>
                        <h3>Модерация</h3>
                    </div>
                    <p>Все загруженные конспекты проходят модерацию перед публикацией. Процесс проверки обычно занимает до 24 часов. Вы получите уведомление о результатах проверки в личном кабинете.</p>
                </div>
            </div>
        </div>

        <!-- Уведомление -->
        <div v-if="showNotification" class="notification">
            <div class="notification-content">
                <div class="check-icon-large"></div>
                <span>{{ isEditMode ? 'Конспект обновлён' : 'Конспект отправлен на модерацию' }}</span>
                <div class="close-icon" @click="showNotification = false"></div>
            </div>
        </div>
    </div>
</template>

<script setup>
    import { ref, reactive, onMounted, onUnmounted, computed } from 'vue'
    import { useRouter, useRoute } from 'vue-router'
    import { useAuthStore } from '@/stores/auth'
    import api from '@/services/api'

    const router = useRouter()
    const route = useRoute()
    const authStore = useAuthStore()

    // Режим редактирования
    const isEditMode = computed(() => !!route.params.id)
    const noteId = computed(() => route.params.id)

    // Состояние меню
    const isMenuOpen = ref(false)

    // Данные конспекта (для редактирования)
    const note = ref({
        title: '',
        description: '',
        subjectId: null,
        teacherId: null,
        universityId: null,
        filePath: ''
    })

    // Данные формы
    const form = reactive({
        title: '',
        description: '',
        subjectId: null,
        teacherId: null,
        universityId: null
    })

    // Ошибки
    const errors = reactive({
        title: '',
        description: '',
        subjectId: '',
        teacherId: '',
        universityId: '',
        file: ''
    })

    // Состояние выпадающих списков
    const isSubjectOpen = ref(false)
    const isTeacherOpen = ref(false)
    const isUniversityOpen = ref(false)

    // Данные для комбобоксов
    const subjects = ref([])
    const teachers = ref([])
    const universities = ref([])

    // Выбранные значения
    const selectedSubjectName = ref('')
    const selectedTeacherName = ref('')
    const selectedUniversityName = ref('')

    // Файл (только для добавления)
    const fileInput = ref(null)
    const selectedFile = ref(null)
    const isDragOver = ref(false)
    const isSubmitting = ref(false)
    const showNotification = ref(false)

    // Загрузка конспекта для редактирования
    const loadNoteForEdit = async () => {
        if (!isEditMode.value) return

        try {
            const response = await api.get(`/notes/${noteId.value}`)
            note.value = response.data

            form.title = note.value.title
            form.description = note.value.description || ''
            form.subjectId = note.value.subjectId
            form.teacherId = note.value.teacherId
            form.universityId = note.value.universityId

            // Устанавливаем отображаемые значения
            const subject = subjects.value.find(s => s.id === note.value.subjectId)
            if (subject) selectedSubjectName.value = subject.name

            const teacher = teachers.value.find(t => t.id === note.value.teacherId)
            if (teacher) selectedTeacherName.value = teacher.fullName

            const university = universities.value.find(u => u.id === note.value.universityId)
            if (university) selectedUniversityName.value = university.name
        } catch (error) {
            console.error('Ошибка загрузки конспекта:', error)
        }
    }

    // Методы меню
    const toggleMenu = () => {
        isMenuOpen.value = !isMenuOpen.value
    }

    const closeMenu = () => {
        isMenuOpen.value = false
    }

    const goToCatalog = () => {
        router.push('/')
    }

    const goToProfile = () => {
        router.push('/profile')
        closeMenu()
    }

    const goToAddNote = () => {
        router.push('/add-note')
        closeMenu()
    }

    const goToLogin = () => {
        router.push('/login')
        closeMenu()
    }

    const goToRegister = () => {
        router.push('/register')
        closeMenu()
    }

    const handleLogout = () => {
        authStore.logout()
        closeMenu()
        router.push('/login')
    }

    const goBack = () => {
        router.back()
    }

    // Методы для выпадающих списков
    const toggleSubjectDropdown = () => {
        isSubjectOpen.value = !isSubjectOpen.value
        isTeacherOpen.value = false
        isUniversityOpen.value = false
    }

    const toggleTeacherDropdown = () => {
        isTeacherOpen.value = !isTeacherOpen.value
        isSubjectOpen.value = false
        isUniversityOpen.value = false
    }

    const toggleUniversityDropdown = () => {
        isUniversityOpen.value = !isUniversityOpen.value
        isSubjectOpen.value = false
        isTeacherOpen.value = false
    }

    const selectSubject = (subject) => {
        form.subjectId = subject.id
        selectedSubjectName.value = subject.name
        isSubjectOpen.value = false
        clearError('subjectId')
    }

    const selectTeacher = (teacher) => {
        form.teacherId = teacher.id
        selectedTeacherName.value = teacher.fullName
        isTeacherOpen.value = false
        clearError('teacherId')
    }

    const selectUniversity = (university) => {
        form.universityId = university.id
        selectedUniversityName.value = university.name
        isUniversityOpen.value = false
        clearError('universityId')
    }

    const clearError = (field) => {
        errors[field] = ''
    }

    const getFileName = (filePath) => {
        if (!filePath) return ''
        return filePath.split('/').pop()
    }

    const validateForm = () => {
        let isValid = true

        if (!form.title.trim()) {
            errors.title = 'Заполните поле'
            isValid = false
        } else {
            errors.title = ''
        }

        if (!form.description.trim()) {
            errors.description = 'Заполните поле'
            isValid = false
        } else {
            errors.description = ''
        }

        if (!form.subjectId) {
            errors.subjectId = 'Выберите предмет'
            isValid = false
        } else {
            errors.subjectId = ''
        }

        if (!form.teacherId) {
            errors.teacherId = 'Выберите преподавателя'
            isValid = false
        } else {
            errors.teacherId = ''
        }

        if (!form.universityId) {
            errors.universityId = 'Выберите ВУЗ'
            isValid = false
        } else {
            errors.universityId = ''
        }

        if (!isEditMode.value && !selectedFile.value) {
            errors.file = 'Загрузите файл'
            isValid = false
        } else {
            errors.file = ''
        }

        return isValid
    }

    // Работа с файлом
    const triggerFileInput = () => {
        if (!isEditMode.value) {
            fileInput.value.click()
        }
    }

    const handleFileSelect = (event) => {
        const file = event.target.files[0]
        if (file && file.type === 'application/pdf') {
            selectedFile.value = file
            clearError('file')
        } else if (file) {
            errors.file = 'Поддерживаются только PDF файлы'
        }
    }

    const handleDrop = (event) => {
        if (isEditMode.value) return
        isDragOver.value = false
        const file = event.dataTransfer.files[0]
        if (file && file.type === 'application/pdf') {
            selectedFile.value = file
            clearError('file')
        } else if (file) {
            errors.file = 'Поддерживаются только PDF файлы'
        }
    }

    const removeFile = () => {
        selectedFile.value = null
        if (fileInput.value) {
            fileInput.value.value = ''
        }
    }

    const formatFileSize = (bytes) => {
        if (bytes < 1024) return bytes + ' B'
        if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(2) + ' KB'
        return (bytes / (1024 * 1024)).toFixed(2) + ' MB'
    }

    // Загрузка данных для комбобоксов
    const loadSubjects = async () => {
        try {
            const response = await api.get('/lookup/all-subjects')
            subjects.value = response.data
        } catch (error) {
            console.error('Ошибка загрузки предметов:', error)
        }
    }

    const loadTeachers = async () => {
        try {
            const response = await api.get('/lookup/all-teachers')
            teachers.value = response.data
        } catch (error) {
            console.error('Ошибка загрузки преподавателей:', error)
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

    // Отправка формы
    const submitNote = async () => {
        if (!validateForm()) return

        isSubmitting.value = true

        try {
            if (isEditMode.value) {
                // Режим редактирования
                await api.put(`/notes/${noteId.value}`, {
                    title: form.title,
                    description: form.description,
                    subjectId: form.subjectId,
                    teacherId: form.teacherId,
                    universityId: form.universityId
                })
            } else {
                // Режим добавления
                const formData = new FormData()
                formData.append('Title', form.title)
                formData.append('Description', form.description)
                formData.append('SubjectId', form.subjectId)
                formData.append('TeacherId', form.teacherId)
                formData.append('UniversityId', form.universityId)
                formData.append('File', selectedFile.value)

                await api.post('/notes/upload', formData, {
                    headers: { 'Content-Type': 'multipart/form-data' }
                })
            }

            showNotification.value = true
            setTimeout(() => {
                showNotification.value = false
                router.push('/profile')
            }, 2000)
        } catch (error) {
            console.error('Ошибка:', error)
            alert(error.response?.data?.message || 'Ошибка при сохранении')
        } finally {
            isSubmitting.value = false
        }
    }

    // Закрытие выпадающих списков при клике вне
    const handleClickOutside = (event) => {
        if (!event.target.closest('.custom-select')) {
            isSubjectOpen.value = false
            isTeacherOpen.value = false
            isUniversityOpen.value = false
        }
        if (!event.target.closest('.avatar-menu')) {
            isMenuOpen.value = false
        }
    }

    onMounted(async () => {
        document.addEventListener('click', handleClickOutside)
        await loadSubjects()
        await loadTeachers()
        await loadUniversities()
        if (isEditMode.value) {
            await loadNoteForEdit()
        }
    })

    onUnmounted(() => {
        document.removeEventListener('click', handleClickOutside)
    })
</script>

<style scoped>
    .add-note-page {
        min-height: 100vh;
        background: #0F0F14;
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

    .page-title {
        font-size: 32px;
        font-weight: 700;
        color: #FFFFFF;
        margin-bottom: 8px;
    }

    .page-subtitle {
        font-size: 16px;
        font-weight: 400;
        color: #A0A0B0;
        margin-bottom: 32px;
    }

    .form-container {
        display: flex;
        gap: 32px;
    }

    .main-form {
        flex: 2;
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 20px;
        padding: 32px;
    }

    .form-group {
        margin-bottom: 24px;
    }

        .form-group label {
            display: block;
            font-size: 18px;
            font-weight: 500;
            color: #FFFFFF;
            margin-bottom: 8px;
        }

    .input-wrapper input,
    .input-wrapper textarea {
        width: 100%;
        padding: 16px;
        background: #0F0F14;
        border: 1px solid #2A2A35;
        border-radius: 14px;
        font-size: 16px;
        color: #FFFFFF;
        font-family: 'Inter', sans-serif;
    }

    .input-wrapper textarea {
        resize: vertical;
        min-height: 120px;
    }

        .input-wrapper input:focus,
        .input-wrapper textarea:focus {
            outline: none;
            border-color: #6C63FF;
        }

    .input-wrapper.error input,
    .input-wrapper.error textarea {
        border-color: #EF4444;
        box-shadow: 0px 0px 8px rgba(239, 68, 68, 0.25);
    }

    .error-message {
        font-size: 14px;
        color: #EF4444;
        margin-top: 6px;
        margin-left: 12px;
    }

    .custom-select {
        position: relative;
    }

    .select-trigger {
        width: 100%;
        padding: 16px;
        background: #0F0F14;
        border: 1px solid #2A2A35;
        border-radius: 14px;
        display: flex;
        justify-content: space-between;
        align-items: center;
        cursor: pointer;
        color: #FFFFFF;
        font-size: 16px;
    }

    .custom-select.error .select-trigger {
        border-color: #EF4444;
        box-shadow: 0px 0px 8px rgba(239, 68, 68, 0.25);
    }

    .chevron-icon {
        width: 24px;
        height: 24px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%237F8499" stroke-width="3"%3E%3Cpolyline points="6 9 12 15 18 9"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
        transition: transform 0.2s;
    }

    .custom-select.open .chevron-icon {
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
        padding: 12px 16px;
        cursor: pointer;
        color: #FFFFFF;
        font-size: 16px;
        transition: background 0.2s;
    }

        .select-option:hover {
            background: #2A2A35;
        }

        .select-option.selected {
            background: #2A2A35;
            color: #6C63FF;
        }

    .drop-zone {
        border: 2px dashed #2A2A35;
        border-radius: 16px;
        padding: 40px 20px;
        text-align: center;
        cursor: pointer;
        transition: all 0.2s;
        background: #1A1A22;
    }

        .drop-zone.dragOver {
            border-color: #6C63FF;
            background: #2A2A35;
        }

        .drop-zone.error {
            border-color: #EF4444;
        }

    .drop-zone-icon {
        width: 51px;
        height: 52px;
        margin: 0 auto 16px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%237F8499" stroke-width="2"%3E%3Cpath d="M12 3v12m0 0-3-3m3 3 3-3M5 17v2a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2v-2"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .drop-zone p {
        font-size: 16px;
        color: #FFFFFF;
        margin-bottom: 8px;
    }

    .drop-zone span {
        font-size: 14px;
        color: #7F8499;
    }

    .selected-file {
        display: flex;
        align-items: center;
        justify-content: space-between;
        background: #0F0F14;
        border-radius: 12px;
        padding: 12px 16px;
        margin-top: 12px;
    }

    .file-info {
        display: flex;
        align-items: center;
        gap: 12px;
    }

    .file-icon-small {
        width: 40px;
        height: 40px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%236C63FF" stroke-width="2"%3E%3Cpath d="M13 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9z"/%3E%3Cpolyline points="13 2 13 9 20 9"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .file-name {
        font-size: 16px;
        font-weight: 500;
        color: #FFFFFF;
    }

    .file-size {
        font-size: 14px;
        color: #A0A0B0;
    }

    .remove-file {
        background: none;
        border: none;
        cursor: pointer;
    }

    .close-icon {
        width: 24px;
        height: 24px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23EF4444" stroke-width="2"%3E%3Cline x1="18" y1="6" x2="6" y2="18"/%3E%3Cline x1="6" y1="6" x2="18" y2="18"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .current-file {
        display: flex;
        align-items: center;
        gap: 12px;
        background: #0F0F14;
        border-radius: 12px;
        padding: 12px 16px;
        margin-top: 8px;
    }

    .view-file-link {
        color: #6C63FF;
        text-decoration: none;
        margin-left: auto;
    }

        .view-file-link:hover {
            text-decoration: underline;
        }

    .form-buttons {
        display: flex;
        gap: 16px;
        margin-top: 24px;
    }

    .cancel-btn, .submit-btn {
        padding: 14px 24px;
        border-radius: 14px;
        font-size: 18px;
        font-weight: 600;
        cursor: pointer;
        transition: all 0.2s;
    }

    .cancel-btn {
        background: #0F0F14;
        border: 1px solid #2A2A35;
        color: #FFFFFF;
    }

    .submit-btn {
        background: #6C63FF;
        border: none;
        color: #FFFFFF;
        flex: 1;
    }

        .submit-btn:hover:not(:disabled) {
            background: #594FFE;
            transform: translateY(-2px);
            box-shadow: 0px 8px 16px rgba(108, 99, 255, 0.45);
        }

        .submit-btn:disabled {
            opacity: 0.6;
            cursor: not-allowed;
        }

    .info-panel {
        flex: 1;
        display: flex;
        flex-direction: column;
        gap: 24px;
    }

    .info-card {
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 20px;
        padding: 24px;
    }

    .info-header {
        display: flex;
        align-items: center;
        gap: 12px;
        margin-bottom: 16px;
    }

    .info-icon {
        width: 28px;
        height: 28px;
    }

    .requirements-icon {
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%236C63FF" stroke-width="2"%3E%3Ccircle cx="12" cy="12" r="10"/%3E%3Cpath d="M12 16v-4M12 8h.01"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .moderation-icon {
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23FFE100" stroke-width="2"%3E%3Cpath d="M12 2L2 7l10 5 10-5-10-5zM2 17l10 5 10-5M2 12l10 5 10-5"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .info-card h3 {
        font-size: 20px;
        font-weight: 600;
        color: #FFFFFF;
    }

    .requirements-list {
        list-style: none;
        padding: 0;
    }

        .requirements-list li {
            display: flex;
            align-items: center;
            gap: 8px;
            font-size: 16px;
            color: #FFFFFF;
            margin-bottom: 12px;
        }

    .check-icon {
        width: 20px;
        height: 20px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="%2322C55E"%3E%3Cpath d="M9 16.17L4.83 12l-1.42 1.41L9 19 21 7l-1.41-1.41L9 16.17z"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .info-card p {
        font-size: 16px;
        line-height: 1.5;
        color: #FFFFFF;
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

    .check-icon-large {
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

    @media (max-width: 1200px) {
        .add-note-page {
            padding: 20px 40px;
        }
    }

    @media (max-width: 900px) {
        .add-note-page {
            padding: 16px;
        }

        .form-container {
            flex-direction: column;
        }

        .info-panel {
            flex-direction: row;
        }
    }

    @media (max-width: 560px) {
        .form-container {
            flex-direction: column;
        }

        .info-panel {
            flex-direction: column;
        }

        .form-buttons {
            flex-direction: column;
        }

        .main-form {
            padding: 20px;
        }
    }
</style>