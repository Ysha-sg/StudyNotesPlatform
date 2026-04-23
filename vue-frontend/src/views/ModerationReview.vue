<template>
    <div class="review-page">
        <header class="topbar">
            <div class="topbar-inner">
                <div class="topbar-start">
                    <button type="button" class="back-button" @click="goBack" aria-label="Назад">
                        <svg width="49" height="49" viewBox="0 0 49 49" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <circle cx="24.5" cy="24.5" r="23" stroke="#A0A0B0" stroke-width="2.5" />
                            <polyline points="27 17 18 24.5 27 32" stroke="#A0A0B0" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" />
                        </svg>
                    </button>

                    <div class="brand">
                        <div class="brand-icon">
                            <svg width="31" height="28" viewBox="0 0 69 69" fill="none" xmlns="http://www.w3.org/2000/svg">
                                <path
                                    d="M34.5 20.125C34.5 17.075 33.2884 14.1499 31.1317 11.9933C28.9751 9.8366 26.05 8.625 23 8.625H5.75V51.75H25.875C28.1625 51.75 30.3563 52.6587 31.9738 54.2762C33.5913 55.8937 34.5 58.0875 34.5 60.375M34.5 20.125V60.375M34.5 20.125C34.5 17.075 35.7116 14.1499 37.8683 11.9933C40.0249 9.8366 42.95 8.625 46 8.625H63.25V51.75H43.125C40.8375 51.75 38.6437 52.6587 37.0262 54.2762C35.4087 55.8937 34.5 58.0875 34.5 60.375"
                                    stroke="#6C63FF"
                                    stroke-width="4"
                                    stroke-linecap="round"
                                    stroke-linejoin="round"
                                />
                            </svg>
                        </div>
                        <span>Модерация конспекта</span>
                    </div>
                </div>

                <button class="exit-btn" @click="logout" aria-label="Выйти">
                    <svg width="32" height="32" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" stroke="#7F8499" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
                        <polyline points="16 17 21 12 16 7" stroke="#7F8499" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
                        <line x1="21" y1="12" x2="9" y2="12" stroke="#7F8499" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
                    </svg>
                </button>
            </div>
        </header>

        <main v-if="!isLoading && note.id" class="content">
            <h1 class="title" :class="{ 'title-complaint': mode === 'complaint' }">{{ note.title }}</h1>

            <section class="main-column">

                <article v-if="mode === 'complaint' && complaint.id" class="panel complaint-panel">
                    <div class="complaint-header">
                        <h3>Жалоба</h3>
                        <p class="complaint-date">Дата жалобы: {{ formatDate(complaint.createdAt) }}</p>
                    </div>
                    <p class="complaint-line"><strong>Причина жалобы:</strong> {{ complaint.reason || "—" }}</p>
                    <p class="complaint-line"><strong>Комментарий:</strong> {{ complaint.comment || "—" }}</p>
                    <p class="complaint-line"><strong>Заявитель:</strong> {{ complaint.reporter || "—" }}</p>
                </article>

                <article class="panel">
                    <h3>Описание</h3>
                    <p class="description">{{ note.description || "Описание не указано" }}</p>
                </article>

                <article class="panel">
                    <h3>Информация об авторе</h3>
                    <div class="author-card">
                        <div class="author-avatar">{{ authorInitials }}</div>
                        <div class="author-meta">
                            <p class="author-name">{{ note.author || "Неизвестный автор" }}</p>
                            <p class="author-university">{{ note.university || "—" }}</p>
                        </div>
                    </div>
                </article>

                <article class="panel file-panel">
                    <h3>Файл конспекта</h3>

                    <div class="file-box">
                        <div class="file-head">
                            <div class="file-icon"></div>
                            <div>
                                <p class="file-name">{{ getFileName(note.filePath) }}</p>
                                <p class="file-type">PDF файл</p>
                            </div>
                        </div>

                        <div class="file-actions">
                            <button class="ghost-btn" @click="viewFile">Посмотреть</button>
                            <button class="ghost-btn" @click="downloadFile">Скачать</button>
                        </div>
                    </div>
                </article>
            </section>

            <aside class="decision-column">
                <article class="decision-panel">
                    <h3>{{ mode === 'complaint' ? "Решение по жалобе" : "Принятие решения" }}</h3>

                    <template v-if="mode === 'note'">
                        <button class="decision-btn approve-btn" :class="{ active: decisionMode === 'approve' }" @click="selectDecision('approve')">
                            <span class="btn-icon check"></span>
                            Одобрить конспект
                        </button>

                        <button class="decision-btn reject-btn" :class="{ active: decisionMode === 'reject' }" @click="selectDecision('reject')">
                            <span class="btn-icon close"></span>
                            Отклонить конспект
                        </button>

                        <div v-if="decisionMode" class="decision-divider"></div>

                        <div v-if="decisionMode === 'approve'" class="state-box state-green">
                            Конспект будет опубликован в каталоге
                            <br />
                            и станет доступен всем пользователям.
                        </div>

                        <div v-if="decisionMode === 'reject'" class="reject-block">
                            <p class="warning-row">
                                <span class="warning-icon"></span>
                                Укажите причину отклонения
                            </p>

                            <label class="field-label">Причина отклонения</label>
                            <div class="custom-select reject-reason-select" :class="{ open: isRejectReasonOpen }">
                                <div class="select-trigger" @click="toggleRejectReasonDropdown">
                                    <span class="select-value" :class="{ placeholder: !rejectReason }">{{ rejectReason || "Выберите причину" }}</span>
                                    <span class="chevron-icon"></span>
                                </div>

                                <div v-if="isRejectReasonOpen" class="select-dropdown">
                                    <div
                                        v-for="reason in rejectReasons"
                                        :key="reason"
                                        class="select-option"
                                        :class="{ selected: rejectReason === reason }"
                                        @click="selectRejectReason(reason)"
                                    >
                                        {{ reason }}
                                    </div>
                                </div>
                            </div>

                            <label class="field-label" for="reject-comment">Комментарий модератора</label>
                            <textarea
                                id="reject-comment"
                                v-model="decisionComment"
                                class="field-textarea"
                                rows="4"
                                placeholder="Дополнительные пояснения для автора..."
                            ></textarea>
                        </div>

                        <button v-if="decisionMode === 'approve'" class="confirm-btn confirm-green" @click="submitNoteDecision">
                            <span class="btn-icon check"></span>
                            Подтвердить одобрение
                        </button>

                        <button v-if="decisionMode === 'reject'" class="confirm-btn confirm-red" @click="submitNoteDecision">
                            <span class="btn-icon close"></span>
                            Подтвердить отклонение
                        </button>
                    </template>

                    <template v-else>
                        <button
                            class="decision-btn reject-btn"
                            :class="{ active: decisionMode === 'confirm-complaint' }"
                            @click="selectDecision('confirm-complaint')"
                        >
                            <span class="btn-icon close"></span>
                            Подтвердить жалобу
                        </button>

                        <button
                            class="decision-btn neutral-btn"
                            :class="{ active: decisionMode === 'dismiss-complaint' }"
                            @click="selectDecision('dismiss-complaint')"
                        >
                            <span class="btn-icon close"></span>
                            Отклонить жалобу
                        </button>

                        <div v-if="decisionMode" class="decision-divider"></div>

                        <div v-if="decisionMode === 'confirm-complaint'" class="state-box state-red">
                            Конспект будет снят с публикации.
                        </div>

                        <div v-if="decisionMode === 'dismiss-complaint'" class="reject-block">
                            <textarea
                                v-model="decisionComment"
                                class="field-textarea"
                                rows="4"
                                placeholder="Поясните, почему жалоба отклонена..."
                            ></textarea>
                        </div>

                        <button v-if="decisionMode === 'confirm-complaint'" class="confirm-btn confirm-red" @click="submitComplaintDecision">
                            <span class="btn-icon close"></span>
                            Снять с публикации
                        </button>

                        <button v-if="decisionMode === 'dismiss-complaint'" class="confirm-btn confirm-neutral" @click="submitComplaintDecision">
                            Применить решение
                        </button>
                    </template>

                    <p v-if="feedbackMessage" class="feedback" :class="{ error: isErrorFeedback }">{{ feedbackMessage }}</p>
                </article>
            </aside>
        </main>

        <div v-else class="loading">Загрузка...</div>
    </div>
</template>

<script setup>
    import { computed, onMounted, onUnmounted, ref } from 'vue'
    import { useRoute, useRouter } from 'vue-router'
    import { useAuthStore } from '@/stores/auth'
    import api from '@/services/api'

    const props = defineProps({
        mode: {
            type: String,
            default: 'note'
        }
    })

    const route = useRoute()
    const router = useRouter()
    const authStore = useAuthStore()

    const isLoading = ref(true)
    const feedbackMessage = ref('')
    const isErrorFeedback = ref(false)
    const decisionMode = ref(null)
    const decisionComment = ref('')
    const rejectReason = ref('')
    const isRejectReasonOpen = ref(false)

    const rejectReasons = ['Плохое качество', 'Не соответствует описанию', 'Содержит нарушения правил', 'Недостаточно учебного содержания']

    const statusIds = ref({
        approved: null,
        rejected: null
    })

    const note = ref({
        id: null,
        title: '',
        description: '',
        subject: '',
        university: '',
        teacher: '',
        author: '',
        uploadedAt: '',
        filePath: ''
    })

    const complaint = ref({
        id: null,
        reason: '',
        comment: '',
        reporter: '',
        createdAt: '',
        noteId: null
    })

    const authorInitials = computed(() => {
        const fullName = (note.value.author || '').trim()
        if (!fullName) return 'ИИ'

        const parts = fullName.split(/\s+/)
        if (parts.length === 1) return parts[0][0]?.toUpperCase() || 'И'
        return `${parts[0][0] || ''}${parts[1][0] || ''}`.toUpperCase()
    })

    const currentNoteId = computed(() => {
        if (props.mode === 'complaint') return Number(complaint.value.noteId)
        return Number(route.params.id)
    })

    const formatDate = (value) => {
        if (!value) return '—'
        const date = new Date(value)
        return date.toLocaleDateString('ru-RU', { day: 'numeric', month: 'long', year: 'numeric' }).replace(' г.', '')
    }

    const getFileName = (filePath) => {
        if (!filePath) return ''
        return filePath.split('/').pop()
    }

    const getDownloadFileName = (fallbackName, contentDisposition) => {
        if (!contentDisposition) return fallbackName

        const utfMatch = contentDisposition.match(/filename\*=UTF-8''([^;]+)/i)
        if (utfMatch && utfMatch[1]) {
            try {
                return decodeURIComponent(utfMatch[1])
            } catch {
                return utfMatch[1]
            }
        }

        const asciiMatch = contentDisposition.match(/filename=\"?([^\";]+)\"?/i)
        if (asciiMatch && asciiMatch[1]) {
            return asciiMatch[1]
        }

        return fallbackName
    }

    const redirectToLogin = () => {
        router.push({
            path: '/login',
            query: {
                redirect: route.fullPath
            }
        })
    }

    const loadStatuses = async () => {
        const response = await api.get('/notes/statuses')
        const statuses = response.data || []
        statusIds.value.approved = statuses.find((item) => item.code === 'approved')?.id || null
        statusIds.value.rejected = statuses.find((item) => item.code === 'rejected')?.id || null
    }

    const loadNote = async (noteId) => {
        const response = await api.get(`/notes/${noteId}`)
        const data = response.data || {}

        note.value = {
            id: data.id ?? null,
            title: data.title || '',
            description: data.description || '',
            subject: data.subject || '',
            university: data.university || '',
            teacher: data.teacher || '',
            author: data.author || '',
            uploadedAt: data.uploadedAt || '',
            filePath: data.filePath || ''
        }
    }

    const loadComplaint = async (complaintId) => {
        const response = await api.get(`/notes/complaints/${complaintId}`)
        const data = response.data || {}

        complaint.value = {
            id: data.id ?? null,
            reason: data.reason || '',
            comment: data.comment || '',
            reporter: data.reporter || data.reporterName || '',
            createdAt: data.createdAt || '',
            noteId: data.noteId ?? null
        }
    }

    const loadPage = async () => {
        try {
            if (props.mode === 'note') {
                await loadStatuses()
            }

            if (props.mode === 'complaint') {
                await loadComplaint(Number(route.params.id))
                if (complaint.value.noteId) {
                    await loadNote(Number(complaint.value.noteId))
                }
            } else {
                await loadNote(Number(route.params.id))
            }
        } catch (error) {
            console.error('Ошибка загрузки страницы модерации:', error)
            isErrorFeedback.value = true
            feedbackMessage.value = 'Не удалось загрузить данные'
        } finally {
            isLoading.value = false
        }
    }

    const viewFile = async () => {
        if (!currentNoteId.value) return
        if (!authStore.isAuthenticated) {
            redirectToLogin()
            return
        }

        const previewWindow = window.open('', '_blank')
        if (!previewWindow) {
            isErrorFeedback.value = true
            feedbackMessage.value = 'Разрешите всплывающие окна в браузере, чтобы открыть файл'
            return
        }

        try {
            const response = await api.get(`/notes/${currentNoteId.value}/file`, {
                responseType: 'blob'
            })
            const blobUrl = URL.createObjectURL(response.data)
            previewWindow.location.href = blobUrl
            setTimeout(() => URL.revokeObjectURL(blobUrl), 60000)
        } catch (error) {
            previewWindow.close()
            if (error.response?.status === 401 || error.response?.status === 403) {
                redirectToLogin()
                return
            }
            console.error('Ошибка просмотра файла:', error)
            isErrorFeedback.value = true
            feedbackMessage.value = 'Не удалось открыть файл'
        }
    }

    const downloadFile = async () => {
        if (!currentNoteId.value) return
        if (!authStore.isAuthenticated) {
            redirectToLogin()
            return
        }

        try {
            const response = await api.get(`/notes/${currentNoteId.value}/download`, {
                responseType: 'blob'
            })

            const fallbackName = getFileName(note.value.filePath) || `note-${currentNoteId.value}.pdf`
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
        } catch (error) {
            if (error.response?.status === 401 || error.response?.status === 403) {
                redirectToLogin()
                return
            }
            console.error('Ошибка скачивания файла:', error)
            isErrorFeedback.value = true
            feedbackMessage.value = 'Не удалось скачать файл'
        }
    }

    const toggleRejectReasonDropdown = () => {
        isRejectReasonOpen.value = !isRejectReasonOpen.value
    }

    const selectRejectReason = (reason) => {
        rejectReason.value = reason
        isRejectReasonOpen.value = false
    }

    const selectDecision = (mode) => {
        decisionMode.value = mode
        if (mode !== 'reject') {
            isRejectReasonOpen.value = false
        }
    }

    const submitNoteDecision = async () => {
        isErrorFeedback.value = false
        feedbackMessage.value = ''

        if (decisionMode.value !== 'approve' && decisionMode.value !== 'reject') {
            isErrorFeedback.value = true
            feedbackMessage.value = 'Выберите решение'
            return
        }

        if (decisionMode.value === 'reject' && !rejectReason.value.trim()) {
            isErrorFeedback.value = true
            feedbackMessage.value = 'Укажите причину отклонения'
            return
        }

        try {
            const statusId = decisionMode.value === 'approve' ? statusIds.value.approved : statusIds.value.rejected
            if (!statusId) {
                throw new Error('Статусы модерации не загружены')
            }

            const comment =
                decisionMode.value === 'reject'
                    ? `${rejectReason.value}${decisionComment.value ? `. ${decisionComment.value}` : ''}`
                    : decisionComment.value || null

            await api.post(`/notes/moderate/${currentNoteId.value}`, { statusId, comment })

            feedbackMessage.value = decisionMode.value === 'approve' ? 'Конспект одобрен' : 'Конспект отклонен'
            setTimeout(() => router.push('/moderation'), 700)
        } catch (error) {
            console.error('Ошибка модерации конспекта:', error)
            isErrorFeedback.value = true
            feedbackMessage.value = error.response?.data?.title || 'Не удалось сохранить решение'
        }
    }

    const submitComplaintDecision = async () => {
        isErrorFeedback.value = false
        feedbackMessage.value = ''

        if (decisionMode.value !== 'confirm-complaint' && decisionMode.value !== 'dismiss-complaint') {
            isErrorFeedback.value = true
            feedbackMessage.value = 'Выберите решение по жалобе'
            return
        }

        try {
            await api.post(`/notes/complaints/${complaint.value.id}/resolve`, {
                confirmComplaint: decisionMode.value === 'confirm-complaint',
                comment: decisionComment.value || null
            })

            feedbackMessage.value = decisionMode.value === 'confirm-complaint' ? 'Жалоба подтверждена' : 'Жалоба отклонена'
            setTimeout(() => router.push('/moderation'), 700)
        } catch (error) {
            console.error('Ошибка обработки жалобы:', error)
            isErrorFeedback.value = true
            feedbackMessage.value = error.response?.data?.title || 'Не удалось обработать жалобу'
        }
    }

    const goBack = () => {
        router.push('/moderation')
    }

    const logout = () => {
        authStore.logout()
        router.push('/login')
    }

    const handleClickOutside = (event) => {
        if (!event.target.closest('.reject-reason-select')) {
            isRejectReasonOpen.value = false
        }
    }

    onMounted(() => {
        document.addEventListener('click', handleClickOutside)
        loadPage()
    })

    onUnmounted(() => {
        document.removeEventListener('click', handleClickOutside)
    })
</script>

<style scoped>
    .review-page {
        min-height: 100vh;
        background: #0f0f14;
        color: #ffffff;
        font-family: 'Inter', sans-serif;
    }

    .topbar {
        height: 126px;
        background: #151722;
    }

    .topbar-inner {
        width: min(1189px, calc(100vw - 40px));
        margin: 0 auto;
        height: 100%;
        display: flex;
        align-items: center;
        justify-content: space-between;
    }

    .topbar-start {
        display: flex;
        align-items: center;
        gap: 58px;
    }

    .back-button {
        width: 49px;
        height: 49px;
        padding: 0;
        border: none;
        background: transparent;
        cursor: pointer;
        flex-shrink: 0;
    }

    .back-button svg circle,
    .back-button svg polyline {
        transition: stroke 0.2s ease;
    }

    .back-button:hover svg circle,
    .back-button:hover svg polyline {
        stroke: #6c63ff;
    }

    .brand {
        display: flex;
        align-items: center;
        gap: 7px;
        font-size: 24px;
        line-height: 29px;
        font-weight: 700;
    }

    .brand-icon {
        width: 55px;
        height: 50px;
        border-radius: 10px;
        background: #2a2348;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        flex-shrink: 0;
    }

    .exit-btn {
        width: 52px;
        height: 52px;
        border: 0;
        border-radius: 12px;
        background: transparent;
        cursor: pointer;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        transition: box-shadow 0.2s, background-color 0.2s;
    }

    .exit-btn:hover {
        background: rgba(42, 42, 53, 0.5);
        box-shadow: 0 0 0 1px rgba(108, 99, 255, 0.26);
    }

    .content {
        width: min(1189px, calc(100vw - 40px));
        margin: 24px auto 0;
        display: grid;
        grid-template-columns: minmax(0, 800px) 370px;
        grid-template-areas:
            'title .'
            'main decision';
        align-items: start;
        column-gap: 20px;
        row-gap: 16px;
    }

    .main-column {
        grid-area: main;
        display: flex;
        flex-direction: column;
        gap: 16px;
    }

    .title {
        grid-area: title;
        margin: 0;
        font-size: 52px;
        line-height: 63px;
        font-weight: 700;
    }

    .title-complaint {
        font-size: 48px;
        line-height: 58px;
    }

    .panel {
        border: 1px solid #2a2a35;
        border-radius: 16px;
        background: #1a1a22;
        padding: 25px 30px;
    }

    .panel h3 {
        margin: 0 0 10px;
        font-size: 20px;
        line-height: 24px;
        font-weight: 700;
    }

    .description {
        margin: 0;
        color: #dfdfea;
        font-size: 16px;
        line-height: 25px;
    }

    .complaint-panel {
        padding-top: 14px;
        padding-bottom: 14px;
    }

    .complaint-header {
        display: flex;
        align-items: baseline;
        justify-content: space-between;
        gap: 16px;
        margin-bottom: 10px;
    }

    .complaint-header h3 {
        margin: 0;
    }

    .complaint-date {
        margin: 0;
        font-size: 20px;
        line-height: 24px;
        color: #ffffff;
    }

    .complaint-line {
        margin: 0 0 6px;
        font-size: 16px;
        line-height: 19px;
        color: #ffffff;
    }

    .author-card {
        min-height: 115px;
        display: flex;
        align-items: center;
        gap: 22px;
    }

    .author-avatar {
        width: 80px;
        height: 80px;
        border-radius: 50%;
        background: linear-gradient(160.69deg, #6c63ff 19.22%, #413b99 78.54%);
        display: inline-flex;
        align-items: center;
        justify-content: center;
        font-size: 34px;
        line-height: 1;
        font-weight: 700;
        color: #ffffff;
        flex-shrink: 0;
    }

    .author-name {
        margin: 0 0 9px;
        font-size: 24px;
        line-height: 29px;
        font-weight: 700;
    }

    .author-university {
        margin: 0;
        color: #a0a0b0;
        font-size: 20px;
        line-height: 24px;
        font-weight: 700;
    }

    .file-panel {
        min-height: 339px;
    }

    .file-box {
        margin-top: 8px;
        background: #0f0f14;
        border: 1px solid transparent;
        border-radius: 12px;
        padding: 26px 26px 22px;
        transition: none;
    }

    .file-head {
        display: flex;
        align-items: center;
        gap: 14px;
    }

    .file-icon {
        width: 62px;
        height: 66px;
        border-radius: 10px;
        background: #2a2348;
        position: relative;
        flex-shrink: 0;
    }

    .file-icon::before {
        content: '';
        position: absolute;
        inset: 11px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%236C63FF" stroke-width="2.8"%3E%3Cpath d="M13 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9z"/%3E%3Cpolyline points="13 2 13 9 20 9"/%3E%3C/svg%3E')
            center / contain no-repeat;
    }

    .file-name {
        margin: 0 0 5px;
        font-size: 20px;
        line-height: 24px;
        font-weight: 500;
        word-break: break-word;
    }

    .file-type {
        margin: 0;
        font-size: 16px;
        line-height: 19px;
        font-weight: 500;
        color: #a0a0b0;
    }

    .file-actions {
        margin-top: 30px;
        display: grid;
        grid-template-columns: repeat(2, minmax(0, 1fr));
        gap: 15px;
    }

    .ghost-btn {
        height: 52px;
        border-radius: 14px;
        border: 1px solid #2a2a35;
        background: #0f0f14;
        color: #ffffff;
        font-size: 20px;
        line-height: 24px;
        font-weight: 700;
        cursor: pointer;
        transition: border-color 0.2s, box-shadow 0.2s;
    }

    .ghost-btn:hover {
        border-color: #6c63ff;
        box-shadow: 0 0 0 1px rgba(108, 99, 255, 0.28);
    }

    .decision-column {
        grid-area: decision;
        width: 100%;
        margin-top: 0;
    }

    .decision-panel {
        border: 1px solid #2a2a35;
        border-radius: 20px;
        background: #1a1a22;
        padding: 20px;
        display: flex;
        flex-direction: column;
        gap: 8px;
        min-height: 210px;
    }

    .decision-panel h3 {
        margin: 0 0 8px;
        font-size: 20px;
        line-height: 24px;
        font-weight: 600;
    }

    .decision-btn,
    .confirm-btn {
        width: 100%;
        min-height: 44px;
        border-radius: 14px;
        border: 1px solid #2a2a35;
        background: #0f0f14;
        color: #ffffff;
        font-size: 20px;
        line-height: 24px;
        font-weight: 500;
        cursor: pointer;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        gap: 8px;
        outline: none;
        transition: border-color 0.2s, box-shadow 0.2s, color 0.2s, background-color 0.2s;
    }

    .approve-btn:hover,
    .approve-btn.active {
        background: rgba(34, 197, 94, 0.15);
        border-color: #22c55e;
        color: #22c55e;
    }

    .reject-btn:hover,
    .reject-btn.active {
        background: rgba(239, 68, 68, 0.12);
        border-color: #ef4444;
        color: #ef4444;
    }

    .neutral-btn:hover,
    .neutral-btn.active {
        background: rgba(160, 160, 176, 0.1);
        border-color: #7f8499;
        color: #ffffff;
        box-shadow: 0 8px 16px rgba(160, 160, 176, 0.2);
    }

    .decision-divider {
        width: 100%;
        border-top: 1px solid #2a2a35;
        margin: 4px 0 2px;
    }

    .btn-icon {
        width: 24px;
        height: 24px;
        border-radius: 50%;
        border: 2px solid currentColor;
        position: relative;
        flex-shrink: 0;
        color: inherit;
    }

    .btn-icon.check::before {
        content: '';
        position: absolute;
        left: 7px;
        top: 4px;
        width: 6px;
        height: 11px;
        border-right: 2px solid currentColor;
        border-bottom: 2px solid currentColor;
        transform: rotate(40deg);
    }

    .btn-icon.close::before,
    .btn-icon.close::after {
        content: '';
        position: absolute;
        left: 10px;
        top: 5px;
        width: 2px;
        height: 11px;
        background: currentColor;
    }

    .btn-icon.close::before {
        transform: rotate(45deg);
    }

    .btn-icon.close::after {
        transform: rotate(-45deg);
    }

    .state-box {
        border-radius: 14px;
        padding: 14px 10px;
        font-size: 15px;
        line-height: 18px;
        text-align: center;
    }

    .state-green {
        border: 1px solid #22c55e;
        background: rgba(34, 197, 94, 0.05);
        color: #22c55e;
    }

    .state-red {
        border: 1px solid #ef4444;
        background: rgba(239, 68, 68, 0.05);
        color: #ef4444;
    }

    .reject-block {
        display: flex;
        flex-direction: column;
        gap: 6px;
    }

    .warning-row {
        margin: 0 0 2px;
        color: #ffe100;
        font-size: 16px;
        line-height: 19px;
        font-weight: 500;
        display: inline-flex;
        align-items: center;
        gap: 6px;
    }

    .warning-icon {
        width: 20px;
        height: 20px;
        border: 2px solid #ffe100;
        border-radius: 50%;
        display: inline-block;
        position: relative;
        flex-shrink: 0;
    }

    .warning-icon::before {
        content: '';
        position: absolute;
        left: 8px;
        top: 3px;
        width: 2px;
        height: 8px;
        background: #ffe100;
    }

    .warning-icon::after {
        content: '';
        position: absolute;
        left: 8px;
        top: 13px;
        width: 2px;
        height: 2px;
        background: #ffe100;
    }

    .field-label {
        margin: 2px 0 0;
        font-size: 15px;
        line-height: 18px;
        font-weight: 400;
        color: #ffffff;
    }

    .custom-select {
        position: relative;
    }

    .select-trigger {
        height: 34px;
        padding: 0 12px;
        border: 1px solid #2a2a35;
        border-radius: 14px;
        background: #0f0f14;
        color: #ffffff;
        font-size: 15px;
        line-height: 18px;
        display: flex;
        align-items: center;
        justify-content: space-between;
        cursor: pointer;
        transition: border-color 0.2s ease, box-shadow 0.2s ease;
    }

    .custom-select.open .select-trigger,
    .select-trigger:hover {
        border-color: #6c63ff;
        box-shadow: 0 0 0 1px rgba(108, 99, 255, 0.28);
    }

    .select-value {
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
    }

    .select-value.placeholder {
        color: #a0a0b0;
    }

    .chevron-icon {
        width: 24px;
        height: 24px;
        flex-shrink: 0;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23A0A0B0" stroke-width="2.8" stroke-linecap="round" stroke-linejoin="round"%3E%3Cpolyline points="6 9 12 15 18 9"/%3E%3C/svg%3E')
            no-repeat center;
        background-size: 16px;
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
        z-index: 40;
        max-height: 130px;
        overflow-y: auto;
        background: #171722;
        border: 1px solid #2a2a35;
        border-radius: 0 0 10px 10px;
        box-shadow: 0 10px 24px rgba(0, 0, 0, 0.3);
    }

    .select-dropdown::-webkit-scrollbar {
        width: 8px;
    }

    .select-dropdown::-webkit-scrollbar-thumb {
        background: #7f8499;
        border-radius: 999px;
    }

    .select-option {
        height: 32px;
        padding: 0 12px;
        display: flex;
        align-items: center;
        color: #ffffff;
        font-size: 15px;
        line-height: 18px;
        cursor: pointer;
        transition: background 0.18s ease;
    }

    .select-option:hover {
        background: rgba(160, 160, 176, 0.24);
    }

    .select-option.selected {
        background: rgba(108, 99, 255, 0.16);
    }

    .field-textarea {
        width: 100%;
        min-height: 92px;
        border-radius: 14px;
        border: 1px solid #2a2a35;
        background: #0f0f14;
        color: #ffffff;
        font-family: inherit;
        font-size: 15px;
        line-height: 18px;
        padding: 12px;
        resize: vertical;
        transition: border-color 0.2s, box-shadow 0.2s;
    }

    .field-textarea:hover {
        border-color: #6c63ff;
        box-shadow: 0 0 0 1px rgba(108, 99, 255, 0.28);
    }

    .field-textarea::placeholder {
        color: #a0a0b0;
    }

    .confirm-green:hover {
        border-color: #22c55e;
        box-shadow: 0 0 0 1px rgba(34, 197, 94, 0.28);
    }

    .confirm-green {
        border-color: #22c55e;
        background: rgba(34, 197, 94, 0.15);
        color: #22c55e;
        box-shadow: 0 8px 16px rgba(34, 197, 94, 0.2);
    }

    .confirm-red:hover {
        border-color: #ef4444;
        box-shadow: 0 0 0 1px rgba(239, 68, 68, 0.28);
    }

    .confirm-red {
        border-color: #ef4444;
        background: rgba(239, 68, 68, 0.2);
        color: #ef4444;
        box-shadow: 0 8px 16px rgba(239, 68, 68, 0.2);
    }

    .confirm-neutral:hover {
        border-color: #7f8499;
        box-shadow: 0 0 0 1px rgba(127, 132, 153, 0.28);
    }

    .confirm-neutral {
        border-color: #7f8499;
        background: rgba(160, 160, 176, 0.2);
        color: #ffffff;
        box-shadow: 0 8px 16px rgba(160, 160, 176, 0.2);
    }

    .feedback {
        margin: 6px 0 0;
        color: #22c55e;
        font-size: 14px;
        line-height: 17px;
    }

    .feedback.error {
        color: #ef4444;
    }

    .loading {
        width: min(1189px, calc(100vw - 40px));
        margin: 80px auto;
        color: #a0a0b0;
        font-size: 24px;
        text-align: center;
    }

    @media (max-width: 1200px) {
        .topbar-inner,
        .content,
        .loading {
            width: calc(100vw - 24px);
        }

        .content {
            grid-template-columns: 1fr;
            grid-template-areas:
                'title'
                'main'
                'decision';
        }

        .decision-column {
            margin-top: 0;
        }

        .title {
            font-size: 36px;
            line-height: 1.1;
        }

        .title-complaint {
            font-size: 34px;
            line-height: 1.12;
        }
    }

    @media (max-width: 860px) {
        .brand {
            font-size: 20px;
            line-height: 24px;
        }

        .topbar-start {
            gap: 18px;
        }

        .complaint-header {
            flex-direction: column;
            align-items: flex-start;
            gap: 6px;
        }

        .file-actions {
            grid-template-columns: 1fr;
        }

        .ghost-btn,
        .decision-btn,
        .confirm-btn {
            font-size: 18px;
        }
    }
</style>
