<template>
  <div class="min-h-screen flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8">
      <div class="text-center">
        <div class="w-16 h-16 bg-gradient-to-br from-primary-teal to-primary-orange rounded-2xl flex items-center justify-center mx-auto mb-6">
          <CpuChipIcon class="w-8 h-8 text-white" />
        </div>
        <h2 class="text-3xl font-bold text-primary-teal">
          {{ $t('login.title') }}
        </h2>
        <p class="mt-2 text-gray-600">
          {{ $t('login.subtitle') }}
        </p>
      </div>

      <div class="card">
        <h1>Esto es una prueba 3</h1>
        <form @submit.prevent="login" class="space-y-6">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              {{ $t('login.email') }}
            </label>
            <input
              v-model="formData.email"
              type="email"
              required
              class="input-field"
              :placeholder="$t('login.emailPlaceholder')"
            />
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              {{ $t('login.password') }}
            </label>
            <input
              v-model="formData.password"
              type="password"
              required
              class="input-field"
              :placeholder="$t('login.passwordPlaceholder')"
            />
          </div>

          <button
            type="submit"
            :disabled="loading"
            class="btn-primary w-full"
          >
            <span v-if="loading" class="flex items-center justify-center">
              <div class="spinner w-5 h-5 mr-2"></div>
              {{ $t('login.loggingIn') }}
            </span>
            <span v-else>
              {{ $t('login.loginButton') }}
            </span>
          </button>
        </form>

        <div class="text-center mt-6">
          <p class="text-gray-600">
            {{ $t('login.noAccount') }}
            <router-link to="/register" class="text-primary-orange hover:underline font-medium">
              {{ $t('login.signUp') }}
            </router-link>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useToast } from 'vue-toastification'
import { CpuChipIcon } from '@heroicons/vue/24/outline'
import api from '../utils/api.js'

const router = useRouter()
const { t } = useI18n()
const toast = useToast()

const loading = ref(false)
const formData = ref({
  email: '',
  password: ''
})

const login = async () => {
  try {
    loading.value = true
    
    const response = await api.post('/api/auth/login', {
      email: formData.value.email,
      password: formData.value.password
    })

    if (response.data.success) {
      localStorage.setItem('token', response.data.token)
      localStorage.setItem('user', JSON.stringify(response.data.user))
      
      toast.success(t('login.success'))
      router.push('/dashboard')
    } else {
      toast.error(response.data.message || t('login.error'))
    }
  } catch (error) {
    console.error('Login error:', error)
    toast.error(t('login.error'))
  } finally {
    loading.value = false
  }
}
</script>