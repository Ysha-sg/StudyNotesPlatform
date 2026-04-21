import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'

// https://vite.dev/config/
export default defineConfig({
    plugins: [
        vue(),
        vueDevTools(),
    ],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        },
    },
    // 👇 ДОБАВЬТЕ ЭТОТ БЛОК НИЖЕ
    server: {
        port: 5173, // Порт, на котором работает ваш Vue-фронтенд
        proxy: {
            '/api': {
                target: 'https://localhost:7111', // АДРЕС ВАШЕГО C# БЭКЕНДА
                changeOrigin: true,
                secure: false,     // Отключаем проверку SSL для локальной разработки
            }
        }
    }
})
