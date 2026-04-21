<template>
    <div class="register-page">
        <button type="button" class="back-button" @click="goBack" aria-label="Назад">
            <svg width="49" height="49" viewBox="0 0 49 49" fill="none" xmlns="http://www.w3.org/2000/svg">
                <circle cx="24.5" cy="24.5" r="23" stroke="#A0A0B0" stroke-width="2.5" />
                <polyline points="27 17 18 24.5 27 32" stroke="#A0A0B0" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" />
            </svg>
        </button>

        <div class="register-content">
            <!-- Логотип / иконка книги -->
            <div class="logo" @click="goHome" role="button" tabindex="0" @keydown.enter="goHome" @keydown.space.prevent="goHome">
                <div class="book-icon">
                    <svg width="69" height="69" viewBox="0 0 69 69" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M34.5 20.125C34.5 17.075 33.2884 14.1499 31.1317 11.9933C28.9751 9.8366 26.05 8.625 23 8.625H5.75V51.75H25.875C28.1625 51.75 30.3563 52.6587 31.9738 54.2762C33.5913 55.8937 34.5 58.0875 34.5 60.375M34.5 20.125V60.375M34.5 20.125C34.5 17.075 35.7116 14.1499 37.8683 11.9933C40.0249 9.8366 42.95 8.625 46 8.625H63.25V51.75H43.125C40.8375 51.75 38.6437 52.6587 37.0262 54.2762C35.4087 55.8937 34.5 58.0875 34.5 60.375" stroke="#6C63FF" stroke-width="4" stroke-linecap="round" stroke-linejoin="round" />
                    </svg>
                </div>
            </div>

            <!-- Заголовки -->
            <h1 class="title">Каталог конспектов</h1>
            <p class="subtitle">Присоединяйтесь к сообществу студентов</p>

            <div class="register-card">
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
                        <div class="custom-select" :class="{ open: isUniversityOpen, error: errors.universityName }">
                            <div class="select-trigger" @click="toggleUniversityDropdown">
                                <span class="input-icon university-icon"></span>
                                <span class="select-value" :class="{ placeholder: !form.universityName }">
                                    {{ form.universityName || 'Выберите ваш ВУЗ' }}
                                </span>
                                <span class="chevron-icon"></span>
                            </div>
                            <div v-if="isUniversityOpen" class="select-dropdown">
                                <div v-for="university in universities"
                                     :key="university"
                                     class="select-option"
                                     :class="{ selected: form.universityName === university }"
                                     @click="selectUniversity(university)">
                                    {{ university }}
                                </div>
                            </div>
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
    </div>
</template>

<script setup>
    import { ref, reactive, onMounted, onUnmounted } from 'vue'
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
    const isUniversityOpen = ref(false)
    const universities = ['ПГГПУ', 'СПбГУ', 'ПГНИУ']

    // Очистка ошибки при вводе
    const clearError = (field) => {
        errors[field] = ''
        generalError.value = ''
    }

    const goHome = () => {
        router.push('/')
    }

    const toggleUniversityDropdown = () => {
        isUniversityOpen.value = !isUniversityOpen.value
    }

    const selectUniversity = (university) => {
        form.universityName = university
        clearError('universityName')
        isUniversityOpen.value = false
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

    const goBack = () => {
        router.back()
    }

    const handleClickOutside = (event) => {
        if (!event.target.closest('.custom-select')) {
            isUniversityOpen.value = false
        }
    }

    onMounted(() => {
        document.addEventListener('click', handleClickOutside)
    })

    onUnmounted(() => {
        document.removeEventListener('click', handleClickOutside)
    })

</script>

<style scoped>
    .register-page {
        min-height: 100vh;
        background: #0F0F14;
        display: flex;
        justify-content: center;
        align-items: flex-start;
        padding: 28px 20px;
        font-family: 'Inter', system-ui, -apple-system, sans-serif;
        position: relative;
    }

    .back-button {
        position: absolute;
        left: 80px;
        top: 59px;
        width: 49px;
        height: 49px;
        border: none;
        background: transparent;
        padding: 0;
        cursor: pointer;
    }

        .back-button svg circle,
        .back-button svg polyline {
            transition: stroke 0.2s ease;
        }

        .back-button:hover svg circle,
        .back-button:hover svg polyline {
            stroke: #6C63FF;
        }

    .register-content {
        width: 100%;
        max-width: 500px;
    }

    .register-card {
        background: #1A1A22;
        border-radius: 20px;
        padding: 36px 32px 28px;
        width: 100%;
        border: 1px solid #2A2A35;
        box-shadow: 0 10px 30px rgba(0,0,0,0.3);
    }

    /* Логотип */
    .logo {
        display: flex;
        justify-content: center;
        margin-bottom: 24px;
        cursor: pointer;
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
        font-size: 40px;
        line-height: 48px;
        font-weight: 700;
        margin-bottom: 10px;
    }

    .subtitle {
        color: #A0A0B0;
        text-align: center;
        font-size: 20px;
        line-height: 24px;
        margin-bottom: 26px;
    }

    .form-title {
        color: #FFFFFF;
        text-align: left;
        font-size: 28px;
        line-height: 34px;
        font-weight: 600;
        margin-bottom: 18px;
    }

    /* Поля формы */
    .form-group {
        margin-bottom: 14px;
    }

        .form-group label {
            display: block;
            color: #FFFFFF;
            margin-bottom: 8px;
            font-size: 20px;
            line-height: 24px;
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
        width: 28px;
        height: 28px;
        z-index: 2;
    }

    /* Иконки */
    .user-icon {
        background: url('data:image/svg+xml,%3Csvg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"%237F8499\" stroke-width=\"2.2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"%3E%3Cpath d=\"M20 21a8 8 0 0 0-16 0\"/%3E%3Ccircle cx=\"12\" cy=\"8\" r=\"4\"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .mail-icon {
        background: url('data:image/svg+xml,%3Csvg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"%237F8499\" stroke-width=\"2.2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"%3E%3Crect x=\"3\" y=\"5\" width=\"18\" height=\"14\" rx=\"2\"/%3E%3Cpath d=\"m3 8 9 6 9-6\"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .lock-icon {
        background: url('data:image/svg+xml,%3Csvg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"%237F8499\" stroke-width=\"2.2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"%3E%3Crect x=\"3\" y=\"11\" width=\"18\" height=\"10\" rx=\"2\"/%3E%3Cpath d=\"M7 11V8a5 5 0 0 1 10 0v3\"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    .university-icon {
        background: url('data:image/svg+xml,%3Csvg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"%237F8499\" stroke-width=\"2.2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"%3E%3Cpath d=\"m22 10-10-5L2 10l10 5 10-5Z\"/%3E%3Cpath d=\"m6 12v5c0 1 2.5 3 6 3s6-2 6-3v-5\"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
    }

    /* Поля ввода */
    .input-wrapper input {
        width: 100%;
        height: 56px;
        padding: 0 16px 0 58px;
        background: #0F0F14;
        border: 1px solid #2A2A35;
        border-radius: 14px;
        color: #FFFFFF;
        font-size: 20px;
        line-height: 24px;
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

    /* Ошибка — красная рамка */
    .input-wrapper.error input {
        border-color: #FF6B6B;
        box-shadow: 0 0 0 3px rgba(255, 107, 107, 0.1);
    }

    /* Кастомный выпадающий список ВУЗа */
    .custom-select {
        position: relative;
    }

    .select-trigger {
        position: relative;
        width: 100%;
        height: 56px;
        padding: 0 56px 0 58px;
        border: 1px solid #2A2A35;
        border-radius: 14px;
        background: #0F0F14;
        display: flex;
        align-items: center;
        cursor: pointer;
        transition: border-color 0.2s, box-shadow 0.2s;
    }

        .select-trigger .input-icon {
            left: 16px;
        }

    .select-value {
        font-size: 20px;
        line-height: 24px;
        color: #FFFFFF;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
    }

        .select-value.placeholder {
            color: #7F8499;
        }

    .chevron-icon {
        position: absolute;
        right: 14px;
        top: 50%;
        width: 36px;
        height: 38px;
        transform: translateY(-50%);
        background: url('data:image/svg+xml,%3Csvg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"%23A0A0B0\" stroke-width=\"2.8\" stroke-linecap=\"round\" stroke-linejoin=\"round\"%3E%3Cpolyline points=\"6 9 12 15 18 9\"/%3E%3C/svg%3E') no-repeat center;
        background-size: 22px;
        transition: transform 0.2s;
    }

    .custom-select.open .chevron-icon {
        transform: translateY(-50%) rotate(180deg);
    }

    .custom-select.open .select-trigger,
    .select-trigger:hover {
        border-color: #6C63FF;
        box-shadow: 0 0 0 1px rgba(108, 99, 255, 0.28);
    }

    .custom-select.error .select-trigger {
        border-color: #FF6B6B;
        box-shadow: 0 0 0 1px rgba(255, 107, 107, 0.28);
    }

    .select-dropdown {
        position: absolute;
        top: calc(100% + 4px);
        left: 0;
        right: 0;
        z-index: 30;
        max-height: 130px;
        overflow-y: auto;
        background: #171722;
        border: 1px solid #2A2A35;
        border-radius: 0 0 10px 10px;
        box-shadow: 0 10px 24px rgba(0, 0, 0, 0.3);
    }

        .select-dropdown::-webkit-scrollbar {
            width: 8px;
        }

        .select-dropdown::-webkit-scrollbar-thumb {
            background: #7F8499;
            border-radius: 999px;
        }

    .select-option {
        height: 32px;
        padding: 0 16px;
        display: flex;
        align-items: center;
        font-size: 16px;
        line-height: 19px;
        color: #FFFFFF;
        cursor: pointer;
        transition: background 0.18s;
    }

        .select-option:hover {
            background: rgba(160, 160, 176, 0.24);
        }

        .select-option.selected {
            background: rgba(108, 99, 255, 0.16);
            color: #FFFFFF;
        }

    /* Глазик */
    .eye-icon {
        position: absolute;
        right: 16px;
        top: 50%;
        transform: translateY(-50%);
        width: 31px;
        height: 31px;
        background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%237F8499" stroke-width="2.2"%3E%3Cpath d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/%3E%3Ccircle cx="12" cy="12" r="3"/%3E%3C/svg%3E') no-repeat center;
        background-size: contain;
        border: none;
        cursor: pointer;
        background-color: transparent;
        z-index: 2;
    }

        .eye-icon.off {
            background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="%237F8499" stroke-width="2.2"%3E%3Cpath d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"/%3E%3Cline x1="1" y1="1" x2="23" y2="23"/%3E%3C/svg%3E') no-repeat center;
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
        max-width: 360px;
        height: 56px;
        display: block;
        margin: 14px auto 0;
        background: #6C63FF;
        color: white;
        border: none;
        border-radius: 14px;
        font-size: 24px;
        line-height: 29px;
        font-weight: 600;
        cursor: pointer;
        transition: all 0.2s;
    }

        .register-btn:hover {
            background: #594FFE;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(139, 127, 255, 0.3);
        }

        .register-btn:active {
            transform: translateY(0);
        }

    /* Ссылка на вход */
    .login-link {
        text-align: center;
        margin-top: 20px;
        padding-top: 16px;
        border-top: 1px solid #2A2A35;
        color: #A0A0B0;
        font-size: 24px;
        line-height: 29px;
        font-weight: 600;
    }

        .login-link a {
            color: #6C63FF;
            text-decoration: none;
            font-weight: 600;
        }

            .login-link a:hover {
                text-decoration: underline;
            }

    /* Адаптивность */
    @media (max-width: 560px) {
        .back-button {
            left: 20px;
            top: 20px;
        }

        .register-card {
            padding: 30px 20px;
        }

        .title {
            font-size: 32px;
            line-height: 38px;
        }

        .subtitle {
            font-size: 18px;
            line-height: 24px;
        }

        .form-title,
        .form-group label,
        .input-wrapper input,
        .select-value {
            font-size: 18px;
        }

        .login-link {
            font-size: 20px;
            line-height: 24px;
        }

        .register-btn {
            font-size: 20px;
        }
    }
</style>
