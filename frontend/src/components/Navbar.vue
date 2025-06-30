<template>
  <nav class="fixed top-0 left-0 right-0 z-50 bg-white shadow-lg">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex justify-between items-center h-16">
        <!-- Logo -->
        <div class="flex items-center space-x-2">
          <router-link to="/" class="flex items-center space-x-2">
            <div class="w-8 h-8 bg-gradient-to-br from-primary-teal to-primary-orange rounded-lg flex items-center justify-center">
              <CpuChipIcon class="w-5 h-5 text-white" />
            </div>
            <span class="text-xl font-bold text-primary-teal">AI Agent</span>
          </router-link>
        </div>

        <!-- Navigation Links -->
        <div class="hidden md:flex items-center space-x-8">
          <router-link to="/" class="text-primary-teal hover:text-primary-orange transition-colors">
            {{ $t('nav.home') }}
          </router-link>
          <router-link to="/purchase" class="text-primary-teal hover:text-primary-orange transition-colors">
            {{ $t('nav.purchase') }}
          </router-link>
          <div v-if="!isLoggedIn" class="flex items-center space-x-4">
            <router-link to="/login" class="text-primary-teal hover:text-primary-orange transition-colors">
              {{ $t('nav.login') }}
            </router-link>
            <router-link to="/register" class="btn-primary text-sm">
              {{ $t('nav.register') }}
            </router-link>
          </div>
          <div v-else class="flex items-center space-x-4">
            <router-link to="/dashboard" class="text-primary-teal hover:text-primary-orange transition-colors">
              {{ $t('nav.dashboard') }}
            </router-link>
          </div>
        </div>

        <!-- Right side: Language + Profile/Auth -->
        <div class="flex items-center space-x-4">
          <!-- Language Selector -->
          <select @change="changeLanguage" v-model="currentLocale" class="text-sm border border-primary-border rounded-lg px-2 py-1">
            <option value="en">🇺🇸 EN</option>
            <option value="es">🇪🇸 ES</option>
          </select>

          <!-- Profile Dropdown (when logged in) -->
          <div v-if="isLoggedIn" class="relative" ref="profileDropdown">
            <button 
              @click="toggleProfileDropdown"
              class="flex items-center space-x-2 bg-primary-gray hover:bg-gray-200 rounded-full px-3 py-2 transition-colors"
            >
              <div class="w-8 h-8 bg-gradient-to-br from-primary-teal to-primary-orange rounded-full flex items-center justify-center">
                <UserIcon class="w-5 h-5 text-white" />
              </div>
              <span class="hidden sm:block text-sm font-medium text-primary-teal">{{ userDisplayName }}</span>
              <ChevronDownIcon class="w-4 h-4 text-primary-teal" />
            </button>

            <!-- Dropdown Menu -->
            <div 
              v-if="profileDropdownOpen" 
              class="absolute right-0 mt-2 w-56 bg-white rounded-xl shadow-lg border border-gray-200 py-2 z-50"
            >
              <div class="px-4 py-3 border-b border-gray-100">
                <p class="text-sm font-medium text-primary-teal">{{ currentUser?.name }}</p>
                <p class="text-xs text-gray-500">{{ currentUser?.email }}</p>
              </div>
              
              <router-link 
                to="/profile" 
                @click="closeProfileDropdown"
                class="flex items-center px-4 py-2 text-sm text-gray-700 hover:bg-primary-gray transition-colors"
              >
                <UserIcon class="w-4 h-4 mr-3" />
                {{ $t('nav.profile') }}
              </router-link>
              
              <router-link 
                to="/dashboard" 
                @click="closeProfileDropdown"
                class="flex items-center px-4 py-2 text-sm text-gray-700 hover:bg-primary-gray transition-colors"
              >
                <ChartBarIcon class="w-4 h-4 mr-3" />
                {{ $t('nav.dashboard') }}
              </router-link>
              
              <hr class="my-2 border-gray-100">
              
              <button 
                @click="logout"
                class="flex items-center w-full px-4 py-2 text-sm text-red-600 hover:bg-red-50 transition-colors"
              >
                <ArrowRightOnRectangleIcon class="w-4 h-4 mr-3" />
                {{ $t('nav.logout') }}
              </button>
            </div>
          </div>

          <!-- Mobile menu button -->
          <button @click="mobileMenuOpen = !mobileMenuOpen" class="md:hidden">
            <Bars3Icon v-if="!mobileMenuOpen" class="w-6 h-6 text-primary-teal" />
            <XMarkIcon v-else class="w-6 h-6 text-primary-teal" />
          </button>
        </div>
      </div>

      <!-- Mobile menu -->
      <div v-if="mobileMenuOpen" class="md:hidden pb-4 border-t border-primary-border mt-4">
        <div class="flex flex-col space-y-3 pt-4">
          <router-link to="/" @click="mobileMenuOpen = false" class="text-primary-teal hover:text-primary-orange transition-colors">
            {{ $t('nav.home') }}
          </router-link>
          <router-link to="/purchase" @click="mobileMenuOpen = false" class="text-primary-teal hover:text-primary-orange transition-colors">
            {{ $t('nav.purchase') }}
          </router-link>
          <div v-if="!isLoggedIn" class="flex flex-col space-y-3">
            <router-link to="/login" @click="mobileMenuOpen = false" class="text-primary-teal hover:text-primary-orange transition-colors">
              {{ $t('nav.login') }}
            </router-link>
            <router-link to="/register" @click="mobileMenuOpen = false" class="btn-primary text-sm w-fit">
              {{ $t('nav.register') }}
            </router-link>
          </div>
          <div v-else class="flex flex-col space-y-3">
            <router-link to="/dashboard" @click="mobileMenuOpen = false" class="text-primary-teal hover:text-primary-orange transition-colors">
              {{ $t('nav.dashboard') }}
            </router-link>
            <router-link to="/profile" @click="mobileMenuOpen = false" class="text-primary-teal hover:text-primary-orange transition-colors">
              {{ $t('nav.profile') }}
            </router-link>
            <button @click="logout" class="text-red-600 hover:text-red-800 transition-colors text-left">
              {{ $t('nav.logout') }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { 
  CpuChipIcon, 
  Bars3Icon, 
  XMarkIcon, 
  UserIcon, 
  ChevronDownIcon,
  ArrowRightOnRectangleIcon,
  ChartBarIcon
} from '@heroicons/vue/24/outline'
import { useToast } from 'vue-toastification'

const router = useRouter()
const { locale, t } = useI18n()
const toast = useToast()

const mobileMenuOpen = ref(false)
const profileDropdownOpen = ref(false)
const currentLocale = ref(locale.value)
const profileDropdown = ref(null)

const isLoggedIn = computed(() => {
  return !!localStorage.getItem('token')
})

const currentUser = computed(() => {
  const userData = localStorage.getItem('user')
  return userData ? JSON.parse(userData) : null
})

const userDisplayName = computed(() => {
  if (currentUser.value?.name) {
    const names = currentUser.value.name.split(' ')
    return names.length > 1 ? `${names[0]} ${names[names.length - 1]}` : names[0]
  }
  return 'Usuario'
})

const changeLanguage = (event) => {
  locale.value = event.target.value
  localStorage.setItem('locale', event.target.value)
}

const toggleProfileDropdown = () => {
  profileDropdownOpen.value = !profileDropdownOpen.value
}

const closeProfileDropdown = () => {
  profileDropdownOpen.value = false
  mobileMenuOpen.value = false
}

const logout = () => {
  localStorage.removeItem('token')
  localStorage.removeItem('user')
  closeProfileDropdown()
  toast.success(t('messages.logoutSuccess'))
  router.push('/')
}

// Cerrar dropdown al hacer clic fuera
const handleClickOutside = (event) => {
  if (profileDropdown.value && !profileDropdown.value.contains(event.target)) {
    profileDropdownOpen.value = false
  }
}

onMounted(() => {
  const savedLocale = localStorage.getItem('locale')
  if (savedLocale) {
    locale.value = savedLocale
    currentLocale.value = savedLocale
  }
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>