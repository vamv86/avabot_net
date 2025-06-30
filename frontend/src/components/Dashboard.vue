<template>
  <div class="min-h-screen py-20 animate-fade-in">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-primary-teal mb-2">
          {{ $t('dashboard.welcome') }}, {{ user?.name }}!
        </h1>
        <p class="text-gray-600">
          {{ $t('dashboard.subtitle') }}
        </p>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Subscription Status -->
        <div class="lg:col-span-2">
          <div class="card animate-slide-up">
            <div class="flex items-center justify-between mb-6">
              <h2 class="text-2xl font-bold text-primary-teal">{{ $t('dashboard.subscription.title') }}</h2>
              <div class="flex items-center space-x-2">
                <div :class="subscriptionStatusClass" class="w-3 h-3 rounded-full"></div>
                <span :class="subscriptionStatusTextClass" class="font-medium">
                  {{ getSubscriptionStatus() }}
                </span>
              </div>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
              <div class="bg-primary-gray p-4 rounded-xl">
                <h3 class="font-semibold text-primary-teal mb-2">{{ $t('dashboard.subscription.plan') }}</h3>
                <p class="text-2xl font-bold text-primary-orange">{{ subscription?.planName || 'AI Agent Pro' }}</p>
                <p class="text-gray-600">$29.99/{{ $t('dashboard.subscription.month') }}</p>
              </div>

              <div class="bg-primary-gray p-4 rounded-xl">
                <h3 class="font-semibold text-primary-teal mb-2">{{ $t('dashboard.subscription.nextBilling') }}</h3>
                <p class="text-2xl font-bold text-primary-teal">{{ formatDate(subscription?.nextBillingDate) }}</p>
                <p class="text-gray-600">{{ getDaysUntilExpiry() }} {{ $t('dashboard.subscription.daysLeft') }}</p>
              </div>
            </div>

            <div class="flex flex-col sm:flex-row gap-4">
              <button 
                @click="renewSubscription"
                :disabled="loading"
                class="btn-primary"
              >
                <span v-if="loading" class="flex items-center justify-center">
                  <div class="spinner w-5 h-5 mr-2"></div>
                  {{ $t('dashboard.subscription.processing') }}
                </span>
                <span v-else>
                  {{ $t('dashboard.subscription.renew') }}
                </span>
              </button>
              <button 
                @click="cancelSubscription"
                class="btn-secondary bg-red-600 hover:bg-red-700"
              >
                {{ $t('dashboard.subscription.cancel') }}
              </button>
            </div>
          </div>

          <!-- Payment Methods -->
          <div class="card mt-8 animate-slide-up" style="animation-delay: 0.2s">
            <h2 class="text-2xl font-bold text-primary-teal mb-6">{{ $t('dashboard.payment.title') }}</h2>
            
            <div v-if="subscription?.paymentMethod" class="bg-primary-gray p-4 rounded-xl mb-4">
              <div class="flex items-center justify-between">
                <div class="flex items-center space-x-3">
                  <CreditCardIcon class="w-6 h-6 text-primary-teal" />
                  <div>
                    <p class="font-medium">{{ $t('dashboard.payment.card') }} •••• {{ subscription.paymentMethod.lastFour }}</p>
                    <p class="text-sm text-gray-600">{{ $t('dashboard.payment.expires') }} {{ subscription.paymentMethod.expiryDate }}</p>
                  </div>
                </div>
                <button @click="removePaymentMethod" class="text-red-600 hover:text-red-800">
                  {{ $t('dashboard.payment.remove') }}
                </button>
              </div>
            </div>

            <button @click="updatePaymentMethod" class="btn-secondary">
              {{ subscription?.paymentMethod ? $t('dashboard.payment.update') : $t('dashboard.payment.add') }}
            </button>
          </div>
        </div>

        <!-- Quick Stats -->
        <div class="space-y-6">
          <div class="card animate-slide-up" style="animation-delay: 0.4s">
            <h3 class="text-lg font-semibold text-primary-teal mb-4">{{ $t('dashboard.stats.usage') }}</h3>
            <div class="space-y-4">
              <div class="flex justify-between items-center">
                <span class="text-gray-600">{{ $t('dashboard.stats.messages') }}</span>
                <span class="font-bold text-primary-teal">{{ stats.messagesThisMonth || 0 }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-gray-600">{{ $t('dashboard.stats.responses') }}</span>
                <span class="font-bold text-primary-teal">{{ stats.responsesThisMonth || 0 }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-gray-600">{{ $t('dashboard.stats.uptime') }}</span>
                <span class="font-bold text-green-600">99.9%</span>
              </div>
            </div>
          </div>

          <div class="card animate-slide-up" style="animation-delay: 0.6s">
            <h3 class="text-lg font-semibold text-primary-teal mb-4">{{ $t('dashboard.support.title') }}</h3>
            <p class="text-gray-600 mb-4">{{ $t('dashboard.support.description') }}</p>
            <button class="btn-primary w-full">
              {{ $t('dashboard.support.contact') }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useToast } from 'vue-toastification'
import { CreditCardIcon } from '@heroicons/vue/24/outline'
import api from '../utils/api.js'

const router = useRouter()
const { t } = useI18n()
const toast = useToast()

const loading = ref(false)
const user = ref(null)
const subscription = ref(null)
const stats = ref({
  messagesThisMonth: 0,
  responsesThisMonth: 0
})

const subscriptionStatusClass = computed(() => {
  if (!subscription.value) return 'bg-gray-400'
  
  const status = subscription.value.status
  if (status === 'active') return 'bg-green-500'
  if (status === 'expired') return 'bg-red-500'
  if (status === 'canceled') return 'bg-yellow-500'
  return 'bg-gray-400'
})

const subscriptionStatusTextClass = computed(() => {
  if (!subscription.value) return 'text-gray-600'
  
  const status = subscription.value.status
  if (status === 'active') return 'text-green-600'
  if (status === 'expired') return 'text-red-600'
  if (status === 'canceled') return 'text-yellow-600'
  return 'text-gray-600'
})

const getSubscriptionStatus = () => {
  if (!subscription.value) return t('dashboard.subscription.status.inactive')
  
  const status = subscription.value.status
  return t(`dashboard.subscription.status.${status}`)
}

const formatDate = (dateString) => {
  if (!dateString) return '-'
  return new Date(dateString).toLocaleDateString()
}

const getDaysUntilExpiry = () => {
  if (!subscription.value?.nextBillingDate) return 0
  
  const today = new Date()
  const expiryDate = new Date(subscription.value.nextBillingDate)
  const diffTime = expiryDate - today
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))
  
  return Math.max(0, diffDays)
}

const loadUserData = async () => {
  try {
    const userData = localStorage.getItem('user')
    if (userData) {
      user.value = JSON.parse(userData)
    }

    const response = await api.get('/api/user/dashboard')

    if (response.data.success) {
      subscription.value = response.data.subscription
      stats.value = response.data.stats
    }
  } catch (error) {
    console.error('Error loading user data:', error)
    if (error.response?.status === 401) {
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      router.push('/login')
    }
  }
}

const renewSubscription = async () => {
  try {
    loading.value = true
    
    const response = await api.post('/api/subscriptions/renew')

    if (response.data.success) {
      if (response.data.paymentUrl) {
        window.location.href = response.data.paymentUrl
      } else {
        toast.success(t('dashboard.subscription.renewSuccess'))
        await loadUserData()
      }
    } else {
      toast.error(t('dashboard.subscription.renewError'))
    }
  } catch (error) {
    console.error('Renew subscription error:', error)
    toast.error(t('dashboard.subscription.renewError'))
  } finally {
    loading.value = false
  }
}

const cancelSubscription = async () => {
  if (!confirm(t('dashboard.subscription.cancelConfirm'))) return

  try {
    const response = await api.post('/api/subscriptions/cancel')

    if (response.data.success) {
      toast.success(t('dashboard.subscription.cancelSuccess'))
      await loadUserData()
    } else {
      toast.error(t('dashboard.subscription.cancelError'))
    }
  } catch (error) {
    console.error('Cancel subscription error:', error)
    toast.error(t('dashboard.subscription.cancelError'))
  }
}

const updatePaymentMethod = () => {
  // Implement payment method update logic
  toast.info(t('dashboard.payment.updateInfo'))
}

const removePaymentMethod = async () => {
  if (!confirm(t('dashboard.payment.removeConfirm'))) return

  try {
    const response = await api.delete('/api/payment-methods')

    if (response.data.success) {
      toast.success(t('dashboard.payment.removeSuccess'))
      await loadUserData()
    } else {
      toast.error(t('dashboard.payment.removeError'))
    }
  } catch (error) {
    console.error('Remove payment method error:', error)
    toast.error(t('dashboard.payment.removeError'))
  }
}

onMounted(() => {
  loadUserData()
})
</script>