<template>
    <div class="moderation-page">
        <header class="topbar">
            <div class="topbar-inner">
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

                <button class="exit-btn" @click="logout" aria-label="Выйти">
                    <svg width="32" height="32" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" stroke="#7F8499" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
                        <polyline points="16 17 21 12 16 7" stroke="#7F8499" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
                        <line x1="21" y1="12" x2="9" y2="12" stroke="#7F8499" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
                    </svg>
                </button>
            </div>
        </header>

        <main class="content">
            <nav class="tabs">
                <button class="tab-btn" :class="{ active: activeTab === 'pending' }" @click="activeTab = 'pending'">
                    <span class="tab-icon"></span>
                    <span class="tab-label">На проверку</span>
                    <span class="tab-badge" :class="activeTab === 'pending' ? 'tab-badge-purple' : 'tab-badge-gray'">{{ pendingNotes.length }}</span>
                </button>

                <button class="tab-btn" :class="{ active: activeTab === 'history' }" @click="activeTab = 'history'">
                    <span class="tab-icon"></span>
                    <span class="tab-label">История проверок</span>
                </button>

                <button class="tab-btn complaints" :class="{ active: activeTab === 'complaints' }" @click="activeTab = 'complaints'">
                    <span class="tab-icon"></span>
                    <span class="tab-label">Жалобы</span>
                    <span class="tab-badge" :class="activeTab === 'complaints' ? 'tab-badge-purple' : 'tab-badge-gray'">{{ openComplaints.length }}</span>
                </button>
            </nav>

            <section v-if="activeTab === 'pending'" class="cards">
                <article v-for="note in pendingNotes" :key="note.id" class="card pending-card">
                    <div class="card-main pending-meta">
                        <p class="card-line">Тема: {{ note.title }}</p>
                        <p class="card-line">Предмет: {{ note.subject }}</p>
                        <p class="card-line">ВУЗ: {{ note.university }}</p>
                    </div>
                    <p class="card-line pending-author">Автор: {{ note.author }}</p>
                    <p class="date pending-date">Дата загрузки: {{ formatDate(note.uploadedAt) }}</p>
                    <button class="open-btn pending-open-btn" @click="openNoteForModeration(note.id)">Открыть для проверки</button>
                </article>

                <p v-if="!pendingNotes.length" class="empty">Нет конспектов на проверке</p>
            </section>

            <section v-if="activeTab === 'history'" class="cards">
                <article
                    v-for="item in moderationHistory"
                    :key="item.id"
                    class="card history-card"
                    :class="{ 'history-card-rejected': item.resultCode === 'rejected' && item.comment }"
                >
                    <div class="card-main history-meta">
                        <p class="card-line">Тема: {{ item.noteTitle }}</p>
                        <p class="card-line">Предмет: {{ item.subject || "—" }}</p>
                        <p class="card-line">ВУЗ: {{ item.university || "—" }}</p>
                    </div>

                    <div class="history-top-right">
                        <div class="history-review-block">
                            <div class="history-review-head">
                                <p class="date history-review-date">Дата проверки: {{ formatDate(item.createdAt) }}</p>
                                <span class="history-status" :class="item.resultCode === 'approved' ? 'approved' : 'rejected'">
                                    {{ item.resultCode === 'approved' ? "Одобрен" : "Отклонен" }}
                                </span>
                            </div>
                            <p class="history-reviewed-by">Проверил: {{ getReviewerLabel(item) }}</p>
                        </div>
                    </div>

                    <p class="card-line history-author">Автор: {{ item.author || "—" }}</p>
                    <div class="history-center-info">
                        <p class="date history-upload-date">Дата загрузки: {{ formatDate(item.uploadedAt) }}</p>
                    </div>
                    <button class="open-btn history-open-btn" @click="openNoteForModeration(item.noteId)">Открыть для проверки</button>

                    <div v-if="item.resultCode === 'rejected' && item.comment" class="history-reject-reason">
                        <span class="history-reject-label">Причина отклонения:</span>
                        <span>{{ item.comment }}</span>
                    </div>
                </article>

                <p v-if="!moderationHistory.length" class="empty">История проверок пока пустая</p>
            </section>

            <section v-if="activeTab === 'complaints'" class="cards">
                <article v-for="complaint in openComplaints" :key="complaint.id" class="card complaint-card">
                    <div class="card-main">
                        <p class="card-line">Тема: {{ complaint.noteTitle }}</p>
                        <p class="card-line">Предмет: {{ complaint.subject || "—" }}</p>
                        <p class="card-line">ВУЗ: {{ complaint.university || "—" }}</p>
                        <p class="card-line">Автор: {{ complaint.author || "—" }}</p>
                        <p class="card-line">Причина жалобы: {{ complaint.reason }}</p>
                        <p class="card-line">Комментарий: {{ complaint.comment || "—" }}</p>
                    </div>

                    <div class="card-right">
                        <p class="date">Дата жалобы: {{ formatDate(complaint.createdAt) }}</p>
                        <button class="open-btn" @click="openComplaint(complaint.id)">Открыть для проверки</button>
                    </div>
                </article>

                <p v-if="!openComplaints.length" class="empty">Открытых жалоб нет</p>
            </section>
        </main>
    </div>
</template>

<script setup>
    import { onMounted, ref } from 'vue'
    import { useRouter } from 'vue-router'
    import { useAuthStore } from '@/stores/auth'
    import api from '@/services/api'

    const router = useRouter()
    const authStore = useAuthStore()

    const activeTab = ref('pending')
    const pendingNotes = ref([])
    const moderationHistory = ref([])
    const openComplaints = ref([])

    const formatDate = (value) => {
        if (!value) return '—'
        const date = new Date(value)
        return date.toLocaleDateString('ru-RU', { day: 'numeric', month: 'long', year: 'numeric' }).replace(' г.', '')
    }

    const getReviewerLabel = (item) => {
        const action = (item?.action || '').toLowerCase()
        if (action.startsWith('auto_')) {
            return 'автопроверка'
        }

        const moderatorName = (item?.moderator || '').trim()
        if (moderatorName) {
            return `модератор ${moderatorName}`
        }

        return 'модератор'
    }

    const loadPendingNotes = async () => {
        try {
            const response = await api.get('/notes/moderation/pending')
            pendingNotes.value = response.data || []
        } catch (error) {
            console.error('Ошибка загрузки списка на проверку:', error)
        }
    }

    const loadModerationHistory = async () => {
        try {
            const response = await api.get('/notes/moderation/history')
            const source = response.data || []
            moderationHistory.value = source.map((item) => ({
                ...item,
                resultCode: item.resultCode || (item.action === 'approved' || item.action === 'auto_approved' ? 'approved' : 'rejected')
            }))
        } catch (error) {
            console.error('Ошибка загрузки истории модерации:', error)
        }
    }

    const loadComplaints = async () => {
        try {
            const response = await api.get('/notes/complaints', { params: { status: 'open' } })
            const source = response.data || []

            const enriched = await Promise.all(
                source.map(async (item) => {
                    try {
                        const noteResponse = await api.get(`/notes/${item.noteId}`)
                        const note = noteResponse.data || {}
                        return {
                            ...item,
                            noteTitle: item.noteTitle || note.title || '',
                            subject: note.subject || '',
                            university: note.university || '',
                            author: note.author || item.author || ''
                        }
                    } catch {
                        return {
                            ...item,
                            subject: '',
                            university: '',
                            author: item.author || ''
                        }
                    }
                })
            )

            openComplaints.value = enriched
        } catch (error) {
            console.error('Ошибка загрузки жалоб:', error)
        }
    }

    const loadData = async () => {
        await Promise.all([loadPendingNotes(), loadModerationHistory(), loadComplaints()])
    }

    const openNoteForModeration = (noteId) => {
        router.push(`/moderation/note/${noteId}`)
    }

    const openComplaint = (complaintId) => {
        router.push(`/moderation/complaint/${complaintId}`)
    }

    const logout = () => {
        authStore.logout()
        router.push('/login')
    }

    onMounted(() => {
        loadData()
    })
</script>

<style scoped>
    .moderation-page {
        min-height: 100vh;
        background: #0f0f14;
        color: #ffffff;
        font-family: 'Inter', sans-serif;
    }

    :global(html),
    :global(body) {
        scrollbar-gutter: stable;
    }

    .topbar {
        height: 126px;
        background: #151722;
    }

    .topbar-inner {
        width: min(1263px, calc(100vw - 40px));
        margin: 0 auto;
        height: 100%;
        display: flex;
        align-items: center;
        justify-content: space-between;
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
        width: min(1263px, calc(100vw - 40px));
        margin: 16px auto 0;
    }

    .tabs {
        height: 72px;
        border: 1px solid #2a2a35;
        border-radius: 20px;
        background: #1a1a22;
        display: grid;
        grid-template-columns: repeat(3, minmax(0, 1fr));
        margin-bottom: 24px;
        overflow: hidden;
    }

    .tab-btn {
        border: 0;
        background: transparent;
        color: #a0a0b0;
        font-size: 20px;
        line-height: 24px;
        font-weight: 600;
        cursor: pointer;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        gap: 9px;
        position: relative;
        transition: color 0.2s;
    }

    .tab-btn.active {
        color: #ffffff;
    }

    .tab-btn.active::after {
        content: '';
        position: absolute;
        left: 13%;
        right: 13%;
        bottom: 0;
        border-bottom: 2px solid #6c63ff;
    }

    .tab-btn.complaints.active::after {
        border-bottom-color: #ef4444;
    }

    .tab-icon {
        width: 36px;
        height: 36px;
        flex-shrink: 0;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" width="36" height="36" viewBox="0 0 36 36" fill="none"%3E%3Cpath d="M8 10H28M8 18H28M8 26H28M4 10H5.2M4 18H5.2M4 26H5.2" stroke="%237F8499" stroke-width="3" stroke-linecap="round"/%3E%3C/svg%3E')
            center / 36px 36px no-repeat;
    }

    .tab-btn.active .tab-icon {
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" width="36" height="36" viewBox="0 0 36 36" fill="none"%3E%3Cpath d="M8 10H28M8 18H28M8 26H28M4 10H5.2M4 18H5.2M4 26H5.2" stroke="%236C63FF" stroke-width="3" stroke-linecap="round"/%3E%3C/svg%3E');
    }

    .tab-btn.complaints.active .tab-icon {
        background-image: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" width="36" height="36" viewBox="0 0 36 36" fill="none"%3E%3Cpath d="M8 10H28M8 18H28M8 26H28M4 10H5.2M4 18H5.2M4 26H5.2" stroke="%23EF4444" stroke-width="3" stroke-linecap="round"/%3E%3C/svg%3E');
    }

    .tab-badge {
        min-width: 30px;
        height: 28px;
        border-radius: 20px;
        padding: 0 8px;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        font-size: 16px;
        line-height: 19px;
        font-weight: 400;
    }

    .tab-badge-purple {
        background: #2a2348;
        color: #6c63ff;
    }

    .tab-badge-gray {
        background: #2a2a35;
        color: #a0a0b0;
    }

    .cards {
        display: flex;
        flex-direction: column;
        gap: 16px;
    }

    .card {
        min-height: 194px;
        border: 1px solid #2a2a35;
        border-radius: 22px;
        background: #1a1a22;
        padding: 30px 20px 24px 96px;
        display: grid;
        grid-template-columns: minmax(0, 1fr) auto;
        column-gap: 34px;
        align-items: end;
        transition: border-color 0.2s, box-shadow 0.2s;
    }

    .pending-card {
        grid-template-columns: minmax(0, 1fr) auto 408px;
        grid-template-areas:
            'meta . .'
            'author date action';
        row-gap: 10px;
        align-items: center;
    }

    .history-card {
        min-height: 206px;
        grid-template-columns: minmax(0, 1fr) auto 408px;
        grid-template-areas:
            'meta top top'
            'author center action';
        row-gap: 10px;
        align-items: center;
    }

    .history-card-rejected {
        min-height: 294px;
        grid-template-areas:
            'meta top top'
            'author center .'
            'reason reason reason'
            '. . action';
    }

    .history-meta {
        grid-area: meta;
        align-self: start;
    }

    .history-top-right {
        grid-area: top;
        justify-self: end;
        align-self: start;
    }

    .history-review-block {
        display: flex;
        flex-direction: column;
        gap: 8px;
        align-items: flex-start;
    }

    .history-review-head {
        display: flex;
        align-items: center;
        gap: 12px;
    }

    .history-status {
        min-width: 120px;
        height: 32px;
        border-radius: 10px;
        padding: 0 14px;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        font-size: 20px;
        line-height: 24px;
        font-weight: 600;
    }

    .history-status.approved {
        color: #22c55e;
        background: rgba(34, 197, 94, 0.18);
    }

    .history-status.rejected {
        color: #ef4444;
        background: rgba(239, 68, 68, 0.2);
    }

    .history-author {
        grid-area: author;
        align-self: center;
    }

    .history-upload-date {
        margin: 0;
    }

    .history-center-info {
        grid-area: center;
        align-self: end;
        display: flex;
        flex-direction: column;
        gap: 0;
    }

    .history-reviewed-by {
        margin: 0;
        color: #a0a0b0;
        font-size: 18px;
        line-height: 22px;
        font-weight: 500;
    }

    .history-open-btn {
        grid-area: action;
        width: 408px;
        min-width: 408px;
        align-self: end;
        justify-self: end;
    }

    .history-reject-reason {
        grid-area: reason;
        border: 1px solid #ef4444;
        border-radius: 12px;
        background: rgba(137, 10, 27, 0.18);
        padding: 12px 20px;
        display: flex;
        flex-direction: column;
        gap: 4px;
        font-size: 16px;
        line-height: 19px;
        color: #ffffff;
    }

    .history-reject-label {
        color: #ef4444;
        font-weight: 700;
    }

    .complaint-card {
        border-color: #2196f3;
    }

    .complaint-card:hover {
        border-color: #29a6ff;
        box-shadow: 0 0 0 1px rgba(33, 150, 243, 0.28);
    }

    .card:hover {
        border-color: #6c63ff;
        box-shadow: 0 0 0 1px rgba(108, 99, 255, 0.28);
    }

    .card-main {
        display: flex;
        flex-direction: column;
        gap: 10px;
        align-self: end;
    }

    .pending-meta {
        grid-area: meta;
        align-self: start;
    }

    .card-line {
        margin: 0;
        font-size: 20px;
        line-height: 24px;
        font-weight: 400;
        color: #ffffff;
    }

    .pending-author {
        grid-area: author;
        align-self: center;
    }

    .card-right {
        display: flex;
        flex-direction: row;
        align-items: center;
        align-self: end;
        gap: 34px;
    }

    .date {
        margin: 0;
        font-size: 20px;
        line-height: 24px;
        font-weight: 400;
        color: #ffffff;
        white-space: nowrap;
    }

    .pending-date {
        grid-area: date;
        align-self: center;
    }

    .open-btn {
        width: 408px;
        min-width: 408px;
        height: 52px;
        border-radius: 14px;
        border: 1px solid #2a2a35;
        background: #6c63ff;
        color: #ffffff;
        font-size: 20px;
        line-height: 24px;
        font-weight: 600;
        cursor: pointer;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        transition: filter 0.2s, border-color 0.2s, box-shadow 0.2s;
    }

    .pending-open-btn {
        grid-area: action;
        align-self: center;
    }

    .open-btn:hover {
        filter: brightness(1.06);
        border-color: #8a83ff;
        box-shadow: 0 8px 20px rgba(108, 99, 255, 0.3);
    }

    .empty {
        margin: 0;
        text-align: center;
        color: #a0a0b0;
        font-size: 20px;
        line-height: 24px;
        padding: 12px 0 18px;
    }

    @media (max-width: 1100px) {
        .topbar-inner,
        .content {
            width: calc(100vw - 24px);
        }

        .tabs {
            height: auto;
            grid-template-columns: 1fr;
        }

        .tab-btn {
            min-height: 56px;
            font-size: 18px;
        }

        .tab-btn.active::after {
            left: 8%;
            right: 8%;
        }

        .card {
            grid-template-columns: 1fr;
            align-items: stretch;
            padding: 22px;
        }

        .pending-card {
            grid-template-columns: 1fr;
            grid-template-areas:
                'meta'
                'author'
                'date'
                'action';
            row-gap: 12px;
        }

        .history-card {
            grid-template-columns: 1fr;
            grid-template-areas:
                'meta'
                'top'
                'author'
                'center'
                'action';
            row-gap: 12px;
        }

        .history-card-rejected {
            grid-template-areas:
                'meta'
                'top'
                'author'
                'center'
                'reason'
                'action';
        }

        .history-top-right {
            justify-self: start;
        }

        .history-review-head {
            flex-wrap: wrap;
        }

        .card-right {
            align-items: stretch;
            flex-direction: column;
            gap: 12px;
        }

        .date {
            white-space: normal;
        }

        .open-btn {
            width: 100%;
            min-width: 0;
        }
    }
</style>


