<template>
    <div id="app">
        <router-view />
    </div>
</template>

<script setup>
    import { onMounted } from 'vue'
    import { useAuthStore } from './stores/auth'
    import { useFavoritesStore } from './stores/favorites'

    const authStore = useAuthStore()
    const favoritesStore = useFavoritesStore()

    onMounted(async () => {
        if (authStore.isAuthenticated) {
            await favoritesStore.loadFavorites()
        }
    })
</script>

<style>
    * {
        margin: 0;
        padding: 0;
        box-sizing: border-box;
    }

    html, body {
        width: 100%;
        min-height: 100vh;
    }

    body {
        font-family: 'Inter', system-ui, -apple-system, sans-serif;
        background: #0A0A0F;
    }

    #app {
        width: 100%;
        min-height: 100vh;
    }
</style>