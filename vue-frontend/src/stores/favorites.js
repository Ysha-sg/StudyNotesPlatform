import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '@/services/api'

export const useFavoritesStore = defineStore('favorites', () => {
    const favorites = ref([])

    // Загрузка избранного с сервера
    const loadFavorites = async () => {
        try {
            const response = await api.get('/notes/favorites')
            favorites.value = response.data
        } catch (error) {
            console.error('Ошибка загрузки избранного:', error)
        }
    }

    // Проверка, в избранном ли конспект
    const isFavorite = (noteId) => {
        return favorites.value.some(fav => fav.id === noteId)
    }

    // Добавление/удаление из избранного
    const toggleFavorite = async (note) => {
        try {
            const response = await api.post(`/notes/${note.id}/favorite`)
            if (response.data.isFavorite) {
                favorites.value.push({
                    id: note.id,
                    title: note.title,
                    subject: note.subject,
                    teacher: note.teacher,
                    university: note.university,
                    rating: note.rating,
                    downloadsCount: note.downloadsCount
                })
            } else {
                favorites.value = favorites.value.filter(f => f.id !== note.id)
            }
            return response.data.isFavorite
        } catch (error) {
            console.error('Ошибка при изменении избранного:', error)
            return false
        }
    }

    return {
        favorites,
        loadFavorites,
        isFavorite,
        toggleFavorite
    }
})