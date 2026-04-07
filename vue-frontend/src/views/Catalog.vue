<template>
    <div class="catalog-container">
        <div class="header">
            <h1>Каталог конспектов</h1>
            <p>Делитесь конспектами и находите нужные материалы</p>
            <div class="user-info">
                {{ authStore.user?.fullName }} ({{ authStore.user?.university }})
                <button @click="logout">Выйти</button>
            </div>
        </div>

        <div class="filters">
            <input type="text" v-model="searchTerm" placeholder="Поиск конспектов..." />
            <select v-model="universityFilter">
                <option value="">Все вузы</option>
                <option value="ПГНИУ">ПГНИУ</option>
                <option value="СПбГУ">СПбГУ</option>
                <option value="ПГГПУ">ПГГПУ</option>
            </select>
            <input type="text" v-model="teacherFilter" placeholder="Преподаватель" />
        </div>

        <div class="results-count">Найдено: {{ filteredNotes.length }}</div>

        <div class="notes-grid">
            <div v-for="note in filteredNotes" :key="note.id" class="note-card">
                <h3>{{ note.title }}</h3>
                <div class="subject">{{ note.subject }}</div>
                <div><strong>Преподаватель:</strong> {{ note.teacher }}</div>
                <div><strong>Вуз:</strong> {{ note.university }}</div>
                <div class="footer">
                    <span>⭐ {{ note.rating }}</span>
                    <span>📥 {{ note.downloads }}</span>
                    <button>Открыть</button>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const searchTerm = ref('')
const universityFilter = ref('')
const teacherFilter = ref('')

const logout = () => {
  authStore.logout()
}

// Временные тестовые данные (потом заменим на API)
const notes = ref([
  { id: 1, title: "Введение в базы данных", subject: "Базы данных", teacher: "Проф. Иванов А.С.", university: "ПГНИУ", rating: 9.8, downloads: 1250 },
  { id: 2, title: "Алгоритмы и структуры данных", subject: "Алгоритмы", teacher: "Доц. Смирнов И.В.", university: "СПбГУ", rating: 9.6, downloads: 980 },
  { id: 3, title: "Педагогика: основы обучения", subject: "Педагогика", teacher: "Проф. Кузнецова Е.В.", university: "ПГГПУ", rating: 9.9, downloads: 760 },
])

const filteredNotes = computed(() => {
  return notes.value.filter(note => {
    const matchSearch = note.title.toLowerCase().includes(searchTerm.value.toLowerCase())
    const matchUniversity = !universityFilter.value || note.university === universityFilter.value
    const matchTeacher = !teacherFilter.value || note.teacher.toLowerCase().includes(teacherFilter.value.toLowerCase())
    return matchSearch && matchUniversity && matchTeacher
  })
})
</script>

<style scoped>
    .catalog-container {
        max-width: 1200px;
        margin: 0 auto;
        padding: 20px;
    }

    .header {
        margin-bottom: 30px;
    }

    .user-info {
        margin-top: 10px;
        padding: 10px;
        background: #eef2ff;
        border-radius: 8px;
        display: flex;
        justify-content: space-between;
        align-items: center;
    }

    .filters {
        display: flex;
        gap: 12px;
        margin-bottom: 20px;
        flex-wrap: wrap;
    }

        .filters input, .filters select {
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 8px;
            flex: 1;
            min-width: 150px;
        }

    .results-count {
        margin-bottom: 20px;
        color: #666;
    }

    .notes-grid {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
        gap: 20px;
    }

    .note-card {
        background: white;
        border-radius: 12px;
        padding: 16px;
        box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

        .note-card h3 {
            margin-bottom: 8px;
        }

    .subject {
        display: inline-block;
        background: #eef2ff;
        padding: 4px 10px;
        border-radius: 20px;
        font-size: 12px;
        margin: 8px 0;
    }

    .footer {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-top: 12px;
        padding-top: 12px;
        border-top: 1px solid #eee;
    }

        .footer button {
            background: #4f46e5;
            color: white;
            border: none;
            padding: 6px 16px;
            border-radius: 6px;
            cursor: pointer;
        }
</style>