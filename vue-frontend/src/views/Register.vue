<template>
    <div class="auth-container">
        <h2>Регистрация</h2>
        <div v-if="error" class="error">{{ error }}</div>

        <form @submit.prevent="handleSubmit">
            <input type="text" v-model="fullName" placeholder="Полное имя" required />
            <input type="email" v-model="email" placeholder="Email" required />
            <input type="password" v-model="password" placeholder="Пароль (мин. 6 символов)" required />
            <select v-model="university" required>
                <option value="">Выберите вуз</option>
                <option value="МГУ">МГУ</option>
                <option value="СПбГУ">СПбГУ</option>
                <option value="МФТИ">МФТИ</option>
                <option value="НИУ ВШЭ">НИУ ВШЭ</option>
            </select>
            <button type="submit">Зарегистрироваться</button>
        </form>

        <p>Уже есть аккаунт? <router-link to="/login">Войти</router-link></p>
    </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const fullName = ref('')
const email = ref('')
const password = ref('')
const university = ref('')
const error = ref('')
const router = useRouter()
const authStore = useAuthStore()

const handleSubmit = async () => {
  error.value = ''
  const result = await authStore.register(fullName.value, email.value, password.value, university.value)

  if (result.success) {
    router.push('/')
  } else {
    error.value = result.message
  }
}
</script>

<style scoped>
    /* Те же стили, что в Login.vue */
    .auth-container {
        max-width: 400px;
        margin: 100px auto;
        padding: 30px;
        background: white;
        border-radius: 12px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.1);
    }

        .auth-container h2 {
            margin-bottom: 20px;
            text-align: center;
        }

        .auth-container input, .auth-container select {
            width: 100%;
            padding: 12px;
            margin-bottom: 16px;
            border: 1px solid #ddd;
            border-radius: 8px;
        }

        .auth-container button {
            width: 100%;
            padding: 12px;
            background: #4f46e5;
            color: white;
            border: none;
            border-radius: 8px;
            cursor: pointer;
        }

    .error {
        background: #fee2e2;
        color: #dc2626;
        padding: 10px;
        border-radius: 8px;
        margin-bottom: 16px;
        text-align: center;
    }

    p {
        text-align: center;
        margin-top: 16px;
    }

    a {
        color: #4f46e5;
        text-decoration: none;
    }
</style>