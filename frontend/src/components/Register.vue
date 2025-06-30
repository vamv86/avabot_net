<template>
  <div class="min-h-screen flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8">
      <div class="text-center">
        <div class="w-16 h-16 bg-gradient-to-br from-primary-teal to-primary-orange rounded-2xl flex items-center justify-center mx-auto mb-6">
          <CpuChipIcon class="w-8 h-8 text-white" />
        </div>
        <h2 class="text-3xl font-bold text-primary-teal">
          {{ $t('register.title') }}
        </h2>
        <p class="mt-2 text-gray-600">
          {{ $t('register.subtitle') }}
        </p>
      </div>

      <div class="card">
        <form @submit.prevent="register" class="space-y-6">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              {{ $t('register.name') }}
            </label>
            <input
              v-model="formData.name"
              type="text"
              required
              class="input-field"
              :placeholder="$t('register.namePlaceholder')"
            />
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              {{ $t('register.email') }}
            </label>
            <input
              v-model="formData.email"
              type="email"
              required
              class="input-field"
              :placeholder="$t('register.emailPlaceholder')"
            />
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              {{ $t('register.whatsapp') }}
            </label>
            <input
              v-model="formData.whatsapp"
              type="tel"
              required
              class="input-field"
              :placeholder="$t('register.whatsappPlaceholder')"
            />
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              {{ $t('register.password') }}
            </label>
            <input
              v-model="formData.password"
              type="password"
              required
              class="input-field"
              :placeholder="$t('register.passwordPlaceholder')"
            />
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              {{ $t('register.confirmPassword') }}
            </label>
            <input
              v-model="formData.confirmPassword"
              type="password"
              required
              class="input-field"
              :placeholder="$t('register.confirmPasswordPlaceholder')"
            />
          </div>

          <button
            type="submit"
            :disabled="loading"
            class="btn-primary w-full"
          >
            <span v-if="loading" class="flex items-center justify-center">
              <div class="spinner w-5 h-5 mr-2"></div>
              {{ $t('register.registering') }}
            </span>
            <span v-else>
              {{ $t('register.registerButton') }}
            </span>
          </button>
        </form>

        <div class="text-center mt-6">
          <p class="text-gray-600">
            {{ $t('register.hasAccount') }}
            <router-link to="/login" class="text-primary-orange hover:underline font-medium">
              {{ $t('register.signIn') }}
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
  name: '',
  email: '',
  whatsapp: '',
  password: '',
  confirmPassword: ''
})

const register = async () => {
  try {
    if (formData.value.password !== formData.value.confirmPassword) {
      toast.error(t('register.passwordMismatch'))
      return
    }

    loading.value = true
    
    const response = await api.post('/api/auth/register', {
      name: formData.value.name,
      email: formData.value.email,
      whatsapp: formData.value.whatsapp,
      password: formData.value.password
    })

    if (response.data.success) {
      toast.success(t('register.success'))
      router.push('/login')
    } else {
      toast.error(response.data.message || t('register.error'))
    }
  } catch (error) {
    console.error('Register error:', error)
    toast.error(t('register.error'))
  } finally {
    loading.value = false
  }
}
</script>