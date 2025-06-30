<template>
  <div class="min-h-screen py-20 animate-fade-in">
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-primary-teal mb-2">
          {{ $t('profile.title') }}
        </h1>
        <p class="text-gray-600">
          {{ $t('profile.subtitle') }}
        </p>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Profile Information -->
        <div class="lg:col-span-2">
          <div class="card animate-slide-up">
            <div class="flex items-center justify-between mb-6">
              <h2 class="text-2xl font-bold text-primary-teal">{{ $t('profile.personalInfo.title') }}</h2>
              <button 
                v-if="!editMode"
                @click="enableEditMode"
                class="btn-secondary text-sm"
              >
                {{ $t('profile.personalInfo.edit') }}
              </button>
            </div>

            <form @submit.prevent="updateProfile" class="space-y-6">
              <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    {{ $t('profile.personalInfo.name') }}
                  </label>
                  <input
                    v-model="profileData.name"
                    type="text"
                    :disabled="!editMode"
                    class="input-field"
                    :class="{ 'bg-gray-50': !editMode }"
                  />
                </div>

                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    {{ $t('profile.personalInfo.email') }}
                  </label>
                  <input
                    v-model="profileData.email"
                    type="email"
                    :disabled="!editMode"
                    class="input-field"
                    :class="{ 'bg-gray-50': !editMode }"
                  />
                </div>

                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    {{ $t('profile.personalInfo.whatsapp') }}
                  </label>
                  <input
                    v-model="profileData.whatsapp"
                    type="tel"
                    :disabled="!editMode"
                    class="input-field"
                    :class="{ 'bg-gray-50': !editMode }"
                  />
                </div>

                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    {{ $t('profile.personalInfo.memberSince') }}
                  </label>
                  <input
                    :value="formatDate(profileData.createdAt)"
                    type="text"
                    disabled
                    class="input-field bg-gray-50"
                  />
                </div>
              </div>

              <div v-if="editMode" class="flex flex-col sm:flex-row gap-4">
                <button 
                  type="submit"
                  :disabled="loading"
                  class="btn-primary"
                >
                  <span v-if="loading" class="flex items-center justify-center">
                    <div class="spinner w-5 h-5 mr-2"></div>
                    {{ $t('profile.personalInfo.saving') }}
                  </span>
                  <span v-else>
                    {{ $t('profile.personalInfo.save') }}
                  </span>
                </button>
                <button 
                  type="button"
                  @click="cancelEdit"
                  class="btn-secondary bg-gray-600 hover:bg-gray-700"
                >
                  {{ $t('profile.personalInfo.cancel') }}
                </button>
              </div>
            </form>
          </div>

          <!-- Change Password -->
          <div class="card mt-8 animate-slide-up" style="animation-delay: 0.2s">
            <h2 class="text-2xl font-bold text-primary-teal mb-6">{{ $t('profile.password.title') }}</h2>
            
            <form @submit.prevent="changePassword" class="space-y-6">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  {{ $t('profile.password.current') }}
                </label>
                <input
                  v-model="passwordData.currentPassword"
                  type="password"
                  class="input-field"
                  :placeholder="$t('profile.password.currentPlaceholder')"
                />
              </div>

              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  {{ $t('profile.password.new') }}
                </label>
                <input
                  v-model="passwordData.newPassword"
                  type="password"
                  class="input-field"
                  :placeholder="$t('profile.password.newPlaceholder')"
                />
              </div>

              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  {{ $t('profile.password.confirm') }}
                </label>
                <input
                  v-model="passwordData.confirmPassword"
                  type="password"
                  class="input-field"
                  :placeholder="$t('profile.password.confirmPlaceholder')"
                />
              </div>

              <button 
                type="submit"
                :disabled="passwordLoading"
                class="btn-primary"
              >
                <span v-if="passwordLoading" class="flex items-center justify-center">
                  <div class="spinner w-5 h-5 mr-2"></div>
                  {{ $t('profile.password.changing') }}
                </span>
                <span v-else>
                  {{ $t('profile.password.change') }}
                </span>
              </button>
            </form>
          </div>
        </div>

        <!-- Account Actions -->
        <div class="space-y-6">
          <!-- Account Status -->
          <div class="card animate-slide-up" style="animation-delay: 0.4s">
            <h3 class="text-lg font-semibold text-primary-teal mb-4">{{ $t('profile.account.status') }}</h3>
            <div class="space-y-3">
              <div class="flex justify-between items-center">
                <span class="text-gray-600">{{ $t('profile.account.accountType') }}</span>
                <span class="font-bold text-primary-teal">{{ $t('profile.account.premium') }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-gray-600">{{ $t('profile.account.status') }}</span>
                <span class="font-bold text-green-600">{{ $t('profile.account.active') }}</span>
              </div>
            </div>
          </div>

          <!-- Quick Actions -->
          <div class="card animate-slide-up" style="animation-delay: 0.6s">
            <h3 class="text-lg font-semibold text-primary-teal mb-4">{{ $t('profile.actions.title') }}</h3>
            <div class="space-y-3">
              <router-link to="/dashboard" class="btn-secondary w-full text-center block">
                {{ $t('profile.actions.viewDashboard') }}
              </router-link>
              <button @click="downloadData" class="btn-secondary w-full bg-blue-600 hover:bg-blue-700">
                {{ $t('profile.actions.downloadData') }}
              </button>
            </div>
          </div>

          <!-- Danger Zone -->
          <div class="card border-2 border-red-200 animate-slide-up" style="animation-delay: 0.8s">
            <h3 class="text-lg font-semibold text-red-600 mb-4">{{ $t('profile.danger.title') }}</h3>
            <p class="text-sm text-gray-600 mb-4">
              {{ $t('profile.danger.description') }}
            </p>
            <button 
              @click="showDeleteConfirmation = true"
              class="btn-secondary w-full bg-red-600 hover:bg-red-700"
            >
              {{ $t('profile.danger.deleteAccount') }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Delete Account Confirmation Modal -->
    <div v-if="showDeleteConfirmation" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-2xl p-6 max-w-md w-full mx-4">
        <div class="text-center">
          <div class="w-16 h-16 bg-red-100 rounded-full flex items-center justify-center mx-auto mb-4">
            <ExclamationTriangleIcon class="w-8 h-8 text-red-600" />
          </div>
          <h3 class="text-xl font-bold text-gray-900 mb-2">
            {{ $t('profile.deleteModal.title') }}
          </h3>
          <p class="text-gray-600 mb-6">
            {{ $t('profile.deleteModal.description') }}
          </p>
          
          <div class="space-y-4">
            <input
              v-model="deleteConfirmationText"
              type="text"
              class="input-field"
              :placeholder="$t('profile.deleteModal.placeholder')"
            />
            
            <div class="flex space-x-3">
              <button 
                @click="showDeleteConfirmation = false"
                class="btn-secondary flex-1 bg-gray-600 hover:bg-gray-700"
              >
                {{ $t('profile.deleteModal.cancel') }}
              </button>
              <button 
                @click="deleteAccount"
                :disabled="deleteConfirmationText !== 'ELIMINAR' && deleteConfirmationText !== 'DELETE'"
                class="btn-primary flex-1 bg-red-600 hover:bg-red-700 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {{ $t('profile.deleteModal.confirm') }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useToast } from 'vue-toastification'
import { ExclamationTriangleIcon } from '@heroicons/vue/24/outline'
import api from '../utils/api.js'

const router = useRouter()
const { t } = useI18n()
const toast = useToast()

const loading = ref(false)
const passwordLoading = ref(false)
const editMode = ref(false)
const showDeleteConfirmation = ref(false)
const deleteConfirmationText = ref('')

const profileData = ref({
  name: '',
  email: '',
  whatsapp: '',
  createdAt: ''
})

const originalProfileData = ref({})

const passwordData = ref({
  currentPassword: '',
  newPassword: '',
  confirmPassword: ''
})

const loadUserProfile = async () => {
  try {
    const userData = localStorage.getItem('user')
    if (userData) {
      const user = JSON.parse(userData)
      profileData.value = {
        name: user.name || '',
        email: user.email || '',
        whatsapp: user.whatsApp || '',
        createdAt: user.createdAt || ''
      }
      originalProfileData.value = { ...profileData.value }
    }
  } catch (error) {
    console.error('Error loading user profile:', error)
    toast.error(t('profile.errors.loadError'))
  }
}

const enableEditMode = () => {
  editMode.value = true
  originalProfileData.value = { ...profileData.value }
}

const cancelEdit = () => {
  editMode.value = false
  profileData.value = { ...originalProfileData.value }
}

const updateProfile = async () => {
  try {
    loading.value = true
    
    const response = await api.put('/api/user/profile', {
      name: profileData.value.name,
      email: profileData.value.email,
      whatsapp: profileData.value.whatsapp
    })

    if (response.data.success) {
      // Actualizar localStorage
      const userData = JSON.parse(localStorage.getItem('user'))
      userData.name = profileData.value.name
      userData.email = profileData.value.email
      userData.whatsApp = profileData.value.whatsapp
      localStorage.setItem('user', JSON.stringify(userData))
      
      editMode.value = false
      toast.success(t('profile.personalInfo.updateSuccess'))
    } else {
      toast.error(response.data.message || t('profile.personalInfo.updateError'))
    }
  } catch (error) {
    console.error('Update profile error:', error)
    toast.error(t('profile.personalInfo.updateError'))
  } finally {
    loading.value = false
  }
}

const changePassword = async () => {
  try {
    if (passwordData.value.newPassword !== passwordData.value.confirmPassword) {
      toast.error(t('profile.password.mismatch'))
      return
    }

    if (passwordData.value.newPassword.length < 6) {
      toast.error(t('profile.password.tooShort'))
      return
    }

    passwordLoading.value = true
    
    const response = await api.put('/api/user/change-password', {
      currentPassword: passwordData.value.currentPassword,
      newPassword: passwordData.value.newPassword
    })

    if (response.data.success) {
      passwordData.value = {
        currentPassword: '',
        newPassword: '',
        confirmPassword: ''
      }
      toast.success(t('profile.password.changeSuccess'))
    } else {
      toast.error(response.data.message || t('profile.password.changeError'))
    }
  } catch (error) {
    console.error('Change password error:', error)
    toast.error(t('profile.password.changeError'))
  } finally {
    passwordLoading.value = false
  }
}

const downloadData = async () => {
  try {
    const response = await api.get('/api/user/export-data', {
      responseType: 'blob'
    })
    
    const url = window.URL.createObjectURL(new Blob([response.data]))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', 'my-data.json')
    document.body.appendChild(link)
    link.click()
    link.remove()
    
    toast.success(t('profile.actions.downloadSuccess'))
  } catch (error) {
    console.error('Download data error:', error)
    toast.error(t('profile.actions.downloadError'))
  }
}

const deleteAccount = async () => {
  try {
    const response = await api.delete('/api/user/account')

    if (response.data.success) {
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      toast.success(t('profile.deleteModal.success'))
      router.push('/')
    } else {
      toast.error(response.data.message || t('profile.deleteModal.error'))
    }
  } catch (error) {
    console.error('Delete account error:', error)
    toast.error(t('profile.deleteModal.error'))
  }
}

const formatDate = (dateString) => {
  if (!dateString) return '-'
  return new Date(dateString).toLocaleDateString()
}

onMounted(() => {
  loadUserProfile()
})
</script>