<template>
    <div class="register-page">
        <div class="register-card">
            <!-- Логотип / иконка книги -->
            <div class="logo">
                <div class="book-icon">
                    <svg width="69" height="69" viewBox="0 0 69 69" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M34.5 20.125C34.5 17.075 33.2884 14.1499 31.1317 11.9933C28.9751 9.8366 26.05 8.625 23 8.625H5.75V51.75H25.875C28.1625 51.75 30.3563 52.6587 31.9738 54.2762C33.5913 55.8937 34.5 58.0875 34.5 60.375M34.5 20.125V60.375M34.5 20.125C34.5 17.075 35.7116 14.1499 37.8683 11.9933C40.0249 9.8366 42.95 8.625 46 8.625H63.25V51.75H43.125C40.8375 51.75 38.6437 52.6587 37.0262 54.2762C35.4087 55.8937 34.5 58.0875 34.5 60.375" stroke="#6C63FF" stroke-width="4" stroke-linecap="round" stroke-linejoin="round" />
                    </svg>
                </div>
            </div>

            <!-- Заголовки -->
            <h1 class="title">Каталог конспектов</h1>
            <p class="subtitle">Присоединяйтесь к сообществу студентов</p>
            <h2 class="form-title">Создать аккаунт</h2>

            <!-- Форма регистрации -->
            <form @submit.prevent="handleSubmit">
                <!-- Имя -->
                <div class="form-group">
                    <label>Имя</label>
                    <div class="input-wrapper" :class="{ error: errors.fullName }">
                        <span class="input-icon user-icon"></span>
                        <input type="text"
                               v-model="form.fullName"
                               placeholder="Иван Иванов"
                               @input="clearError('fullName')" />
                    </div>
                    <div class="error-message" v-if="errors.fullName">{{ errors.fullName }}</div>
                </div>

                <!-- Email -->
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

                <!-- Пароль -->
                <div class="form-group">
                    <label>Пароль</label>
                    <div class="input-wrapper" :class="{ error: errors.password }">
                        <span class="input-icon lock-icon"></span>
                        <input :type="showPassword ? 'text' : 'password'"
                               v-model="form.password"
                               placeholder="Создайте пароль"
                               @input="clearError('password')" />
                        <button type="button"
                                class="eye-icon"
                                :class="{ off: !showPassword }"
                                @click="showPassword = !showPassword"></button>
                    </div>
                    <div class="error-message" v-if="errors.password">{{ errors.password }}</div>
                </div>

                <!-- Повторите пароль -->
                <div class="form-group">
                    <label>Повторите пароль</label>
                    <div class="input-wrapper" :class="{ error: errors.confirmPassword }">
                        <span class="input-icon lock-icon"></span>
                        <input :type="showConfirmPassword ? 'text' : 'password'"
                               v-model="form.confirmPassword"
                               placeholder="Повторите пароль"
                               @input="clearError('confirmPassword')" />
                        <button type="button"
                                class="eye-icon"
                                :class="{ off: !showConfirmPassword }"
                                @click="showConfirmPassword = !showConfirmPassword"></button>
                    </div>
                    <div class="error-message" v-if="errors.confirmPassword">{{ errors.confirmPassword }}</div>
                </div>

                <!-- ВУЗ -->
                <div class="form-group">
                    <label>ВУЗ</label>
                    <div class="input-wrapper" :class="{ error: errors.universityName }">
                        <span class="input-icon university-icon"></span>
                        <select v-model="form.universityName" @change="clearError('universityName')">
                            <option value="">Выберите ваш ВУЗ</option>
                            <option value="МГУ">МГУ</option>
                            <option value="СПбГУ">СПбГУ</option>
                            <option value="МФТИ">МФТИ</option>
                            <option value="НИУ ВШЭ">НИУ ВШЭ</option>
                            <option value="ПГНИУ">ПГНИУ</option>
                            <option value="ПГГПУ">ПГГПУ</option>
                        </select>
                    </div>
                    <div class="error-message" v-if="errors.universityName">{{ errors.universityName }}</div>
                </div>

                <!-- Общая ошибка -->
                <div v-if="generalError" class="general-error">{{ generalError }}</div>

                <!-- Кнопка регистрации -->
                <button type="submit" class="register-btn">Зарегистрироваться</button>
            </form>

            <!-- Ссылка на вход -->
            <p class="login-link">
                Уже есть аккаунт? <router-link to="/login">Войти</router-link>
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

    // Данные формы
    const form = reactive({
        fullName: '',
        email: '',
        password: '',
        confirmPassword: '',
        universityName: ''
    })

    // Ошибки
    const errors = reactive({
        fullName: '',
        email: '',
        password: '',
        confirmPassword: '',
        universityName: ''
    })

    const generalError = ref('')
    const showPassword = ref(false)
    const showConfirmPassword = ref(false)

    // Очистка ошибки при вводе
    const clearError = (field) => {
        errors[field] = ''
        generalError.value = ''
    }

    // Валидация формы
    const validateForm = () => {
        let isValid = true
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

        if (!form.fullName.trim()) {
            errors.fullName = 'Заполните поле'
            isValid = false
        } else {
            errors.fullName = ''
        }

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
        } else if (form.password.length < 8) {
            errors.password = 'Минимум 8 символов'
            isValid = false
        } else {
            errors.password = ''
        }

        if (!form.confirmPassword) {
            errors.confirmPassword = 'Заполните поле'
            isValid = false
        } else if (form.password !== form.confirmPassword) {
            errors.confirmPassword = 'Пароли не совпадают'
            isValid = false
        } else {
            errors.confirmPassword = ''
        }

        if (!form.universityName) {
            errors.universityName = 'Выберите ВУЗ'
            isValid = false
        } else {
            errors.universityName = ''
        }

        return isValid
    }

    // Отправка формы
    const handleSubmit = async () => {
        if (!validateForm()) return

        generalError.value = ''

        const result = await authStore.register(
            form.fullName,
            form.email,
            form.password,
            form.universityName
        )

        if (result.success) {
            router.push('/')
        } else {
            generalError.value = result.message || 'Ошибка регистрации'
        }
    }
</script>

<style scoped>
    /* Яркие, контрастные цвета — всё видно */
    .register-page {
        min-height: 100vh;
        background: #0A0A0F;
        display: flex;
        justify-content: center;
        align-items: center;
        padding: 20px;
        font-family: 'Inter', system-ui, -apple-system, sans-serif;
    }

    .register-card {
        background: #1E1E2A;
        border-radius: 20px;
        padding: 40px;
        width: 100%;
        max-width: 500px;
        border: 1px solid #3A3A4A;
        box-shadow: 0 10px 30px rgba(0,0,0,0.3);
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

    /* Заголовки */
    .title {
        color: #FFFFFF;
        text-align: center;
        font-size: 32px;
        font-weight: 700;
        margin-bottom: 8px;
    }

    .subtitle {
        color: #A0A0B0;
        text-align: center;
        font-size: 18px;
        margin-bottom: 30px;
    }

    .form-title {
        color: #FFFFFF;
        text-align: center;
        font-size: 24px;
        font-weight: 600;
        margin-bottom: 30px;
    }

    /* Поля формы */
    .form-group {
        margin-bottom: 20px;
    }

        .form-group label {
            display: block;
            color: #D0D0D8;
            margin-bottom: 8px;
            font-size: 16px;
            font-weight: 500;
        }

    /* Обёртка для иконок */
    .input-wrapper {
        position: relative;
        width: 100%;
    }

    .input-icon {
        position: absolute;
        left: 16px;
        top: 50%;
        transform: translateY(-50%);
        width: 22px;
        height: 22px;
        z-index: 2;
    }

    /* Иконки */
    .user-icon {
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23A0A0B0" stroke-width="2"%3E%3Ccircle cx="12" cy="8" r="4"/%3E%3Cpath d="M5 20v-2a7 7 0 0 1 14 0v2"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .mail-icon {
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23A0A0B0" stroke-width="2"%3E%3Crect x="2" y="4" width="20" height="16" rx="2"/%3E%3Cpath d="m22 7-10 7L2 7"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .lock-icon {
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23A0A0B0" stroke-width="2"%3E%3Crect x="3" y="11" width="18" height="11" rx="2" ry="2"/%3E%3Cpath d="M7 11V7a5 5 0 0 1 10 0v4"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .university-icon {
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23A0A0B0" stroke-width="2"%3E%3Cpath d="M12 3 2 9l10 6 10-6-10-6zM2 9v6l10 6 10-6V9"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    /* Поля ввода и select */
    .input-wrapper input,
    .input-wrapper select {
        width: 100%;
        padding: 14px 16px 14px 48px;
        background: #2D2D3A;
        border: 1px solid #4A4A5A;
        border-radius: 12px;
        color: #FFFFFF;
        font-size: 16px;
        transition: all 0.2s;
    }

    .input-wrapper select {
        appearance: none;
        cursor: pointer;
    }

    .input-wrapper input::placeholder {
        color: #A0A0B0;
    }

    .input-wrapper input:focus,
    .input-wrapper select:focus {
        outline: none;
        border-color: #8B7FFF;
        box-shadow: 0 0 0 3px rgba(139, 127, 255, 0.2);
    }

    /* Ошибка — красная рамка */
    .input-wrapper.error input,
    .input-wrapper.error select {
        border-color: #FF6B6B;
        box-shadow: 0 0 0 3px rgba(255, 107, 107, 0.1);
    }

    /* Глазик */
    .eye-icon {
        position: absolute;
        right: 16px;
        top: 50%;
        transform: translateY(-50%);
        width: 24px;
        height: 24px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23A0A0B0" stroke-width="2"%3E%3Cpath d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/%3E%3Ccircle cx="12" cy="12" r="3"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
        border: none;
        cursor: pointer;
        background-color: transparent;
        z-index: 2;
    }

        .eye-icon.off {
            background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%23A0A0B0" stroke-width="2"%3E%3Cpath d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"/%3E%3Cline x1="1" y1="1" x2="23" y2="23"/%3E%3C/svg%3E') no-repeat center;
            background-size: contain;
        }

    /* Сообщения об ошибках */
    .error-message {
        font-size: 13px;
        color: #FF6B6B;
        margin-top: 6px;
        margin-left: 12px;
    }

    .general-error {
        background: rgba(255, 107, 107, 0.15);
        color: #FF6B6B;
        padding: 12px;
        border-radius: 12px;
        margin: 16px 0;
        text-align: center;
        font-size: 14px;
    }

    /* Кнопка регистрации */
    .register-btn {
        width: 100%;
        padding: 14px;
        background: #8B7FFF;
        color: white;
        border: none;
        border-radius: 12px;
        font-size: 18px;
        font-weight: 600;
        cursor: pointer;
        margin-top: 10px;
        transition: all 0.2s;
    }

        .register-btn:hover {
            background: #6C63FF;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(139, 127, 255, 0.3);
        }

        .register-btn:active {
            transform: translateY(0);
        }

    /* Ссылка на вход */
    .login-link {
        text-align: center;
        margin-top: 25px;
        padding-top: 20px;
        border-top: 1px solid #3A3A4A;
        color: #A0A0B0;
    }

        .login-link a {
            color: #8B7FFF;
            text-decoration: none;
            font-weight: 500;
        }

            .login-link a:hover {
                text-decoration: underline;
            }

    /* Адаптивность */
    @media (max-width: 560px) {
        .register-card {
            padding: 30px 20px;
        }

        .title {
            font-size: 28px;
        }

        .subtitle,
        .form-title {
            font-size: 18px;
        }

        .register-btn {
            font-size: 16px;
            padding: 12px;
        }
    }
</style>