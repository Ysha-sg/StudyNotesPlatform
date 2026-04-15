<template>
    <div class="login-page">
        <!-- Логотип и заголовки -->
        <div class="logo">
            <div class="book-icon">
                <svg width="69" height="69" viewBox="0 0 69 69" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M34.5 20.125C34.5 17.075 33.2884 14.1499 31.1317 11.9933C28.9751 9.8366 26.05 8.625 23 8.625H5.75V51.75H25.875C28.1625 51.75 30.3563 52.6587 31.9738 54.2762C33.5913 55.8937 34.5 58.0875 34.5 60.375M34.5 20.125V60.375M34.5 20.125C34.5 17.075 35.7116 14.1499 37.8683 11.9933C40.0249 9.8366 42.95 8.625 46 8.625H63.25V51.75H43.125C40.8375 51.75 38.6437 52.6587 37.0262 54.2762C35.4087 55.8937 34.5 58.0875 34.5 60.375" stroke="#8B7FFF" stroke-width="4" stroke-linecap="round" stroke-linejoin="round" />
                </svg>
            </div>
        </div>
        <h1 class="title">Каталог конспектов</h1>
        <p class="subtitle">Делитесь конспектами и находите нужные материалы</p>

        <!-- Карточка входа -->
        <div class="login-card">
            <h2 class="form-title">Вход в аккаунт</h2>

            <form @submit.prevent="handleSubmit">
                <div class="form-group">
                    <label>Email</label>
                    <div class="input-wrapper" :class="{ error: errors.email }">
                        <span class="input-icon mail-icon"></span>
                        <input type="email"
                               v-model="form.email"
                               placeholder="student@example.com"
                               @input="clearError('email')" />
                    </div>
                    <div class="error-message" v-if="errors.email">{{ errors.email }}</div>
                </div>

                <div class="form-group">
                    <label>Пароль</label>
                    <div class="input-wrapper" :class="{ error: errors.password }">
                        <span class="input-icon lock-icon"></span>
                        <input :type="showPassword ? 'text' : 'password'"
                               v-model="form.password"
                               placeholder="Введите пароль"
                               @input="clearError('password')" />
                        <button type="button"
                                class="eye-icon"
                                :class="{ off: !showPassword }"
                                @click="showPassword = !showPassword"></button>
                    </div>
                    <div class="error-message" v-if="errors.password">{{ errors.password }}</div>
                </div>

                <div v-if="generalError" class="general-error">{{ generalError }}</div>

                <button type="submit" class="login-btn">Войти</button>
            </form>

            <p class="register-link">
                Нет аккаунта? <router-link to="/register">Зарегистрироваться</router-link>
            </p>
        </div>
    </div>
</template>

<script setup>
    import { ref, reactive } from 'vue'
    import { useRouter } from 'vue-router'
    import { useAuthStore } from '@/stores/auth'

    const router = useRouter()
    const authStore = useAuthStore()

    const form = reactive({
        email: '',
        password: ''
    })

    const errors = reactive({
        email: '',
        password: ''
    })

    const generalError = ref('')
    const showPassword = ref(false)

    const clearError = (field) => {
        errors[field] = ''
        generalError.value = ''
    }

    const validateForm = () => {
        let isValid = true
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

        if (!form.email.trim()) {
            errors.email = 'Заполните поле'
            isValid = false
        } else if (!emailRegex.test(form.email)) {
            errors.email = 'Адрес эл. почты должен содержать символ "@"'
            isValid = false
        } else {
            errors.email = ''
        }

        if (!form.password) {
            errors.password = 'Заполните поле'
            isValid = false
        } else {
            errors.password = ''
        }

        return isValid
    }

    const handleSubmit = async () => {
        if (!validateForm()) return

        generalError.value = ''

        const result = await authStore.login(form.email, form.password)

        if (result.success) {
            router.push('/')
        } else {
            generalError.value = result.message || 'Неверный email или пароль'
        }
    }
</script>

<style scoped>
    .login-page {
        min-height: 100vh;
        background: #0A0A0F;
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;
        padding: 40px 20px;
        font-family: 'Inter', system-ui, -apple-system, sans-serif;
    }

    /* Логотип */
    .logo {
        display: flex;
        justify-content: center;
        margin-bottom: 24px;
    }

    .book-icon {
        width: 76px;
        height: 76px;
        background: #2A2348;
        border-radius: 10px;
        display: flex;
        align-items: center;
        justify-content: center;
    }

        .book-icon svg {
            width: 60px;
            height: 60px;
        }

    .title {
        font-size: 40px;
        font-weight: 700;
        text-align: center;
        color: #FFFFFF;
        margin-bottom: 8px;
    }

    .subtitle {
        font-size: 20px;
        font-weight: 400;
        text-align: center;
        color: #A0A0B0;
        margin-bottom: 40px;
    }

    /* Карточка входа */
    .login-card {
        width: 500px;
        max-width: 100%;
        background: #1A1A22;
        border: 1px solid #2A2A35;
        border-radius: 20px;
        padding: 40px 32px;
        box-shadow: 0 10px 30px rgba(0,0,0,0.3);
    }

    .form-title {
        font-size: 28px;
        font-weight: 600;
        text-align: center;
        color: #FFFFFF;
        margin-bottom: 32px;
    }

    /* Поля формы */
    .form-group {
        margin-bottom: 20px;
    }

        .form-group label {
            display: block;
            font-size: 20px;
            font-weight: 500;
            color: #FFFFFF;
            margin-bottom: 8px;
        }

    .input-wrapper {
        position: relative;
        width: 100%;
    }

    .input-icon {
        position: absolute;
        left: 16px;
        top: 50%;
        transform: translateY(-50%);
        width: 24px;
        height: 24px;
        z-index: 2;
    }

    .mail-icon {
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23A0A0B0" stroke-width="2"%3E%3Crect x="2" y="4" width="20" height="16" rx="2"/%3E%3Cpath d="m22 7-10 7L2 7"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .lock-icon {
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23A0A0B0" stroke-width="2"%3E%3Crect x="3" y="11" width="18" height="11" rx="2" ry="2"/%3E%3Cpath d="M7 11V7a5 5 0 0 1 10 0v4"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .input-wrapper input {
        width: 100%;
        padding: 14px 16px 14px 52px;
        background: #0F0F14;
        border: 1px solid #2A2A35;
        border-radius: 14px;
        font-size: 20px;
        color: #FFFFFF;
        transition: all 0.2s;
    }

        .input-wrapper input::placeholder {
            color: #7F8499;
        }

        .input-wrapper input:focus {
            outline: none;
            border-color: #8B7FFF;
            box-shadow: 0 0 0 3px rgba(139, 127, 255, 0.2);
        }

    .input-wrapper.error input {
        border-color: #EF4444;
        box-shadow: 0px 0px 4px rgba(239, 68, 68, 0.25);
    }

    .eye-icon {
        position: absolute;
        right: 16px;
        top: 50%;
        transform: translateY(-50%);
        width: 31px;
        height: 31px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%237F8499" stroke-width="2"%3E%3Cpath d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/%3E%3Ccircle cx="12" cy="12" r="3"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
        border: none;
        cursor: pointer;
        background-color: transparent;
        z-index: 2;
    }

        .eye-icon.off {
            background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%237F8499" stroke-width="2"%3E%3Cpath d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"/%3E%3Cline x1="1" y1="1" x2="23" y2="23"/%3E%3C/svg%3E') no-repeat center;
            background-size: contain;
        }

    .error-message {
        font-size: 14px;
        color: #EF4444;
        margin-top: 6px;
        margin-left: 12px;
    }

    .general-error {
        background: rgba(239, 68, 68, 0.1);
        color: #EF4444;
        padding: 12px;
        border-radius: 14px;
        margin: 16px 0;
        text-align: center;
        font-size: 14px;
    }

    .login-btn {
        width: 100%;
        max-width: 360px;
        display: block;
        margin: 24px auto 0;
        padding: 14px;
        background: #6C63FF;
        color: white;
        border: none;
        border-radius: 14px;
        font-size: 24px;
        font-weight: 600;
        cursor: pointer;
        transition: all 0.2s;
    }

        .login-btn:hover {
            background: #594FFE;
            transform: translateY(-2px);
            box-shadow: 0px 8px 16px rgba(108, 99, 255, 0.45);
        }

        .login-btn:active {
            transform: translateY(0);
        }

    .register-link {
        text-align: center;
        margin-top: 32px;
        padding-top: 24px;
        border-top: 1px solid #2A2A35;
        font-size: 24px;
        font-weight: 600;
        color: #A0A0B0;
    }

        .register-link a {
            color: #A0A0B0;
            text-decoration: none;
            transition: color 0.2s;
        }

            .register-link a:hover {
                color: #6C63FF;
                text-decoration: underline;
            }

    /* Адаптивность */
    @media (max-width: 560px) {
        .login-card {
            width: 90%;
            padding: 30px 20px;
        }

        .title {
            font-size: 28px;
        }

        .subtitle,
        .form-title {
            font-size: 18px;
        }

        .form-group label,
        .input-wrapper input {
            font-size: 16px;
        }

        .login-btn {
            font-size: 18px;
            padding: 12px;
        }

        .register-link {
            font-size: 16px;
        }
    }
</style>