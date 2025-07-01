<template>
  <div class="min-h-screen py-20 animate-fade-in">
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="text-center mb-12">
        <h1 class="text-3xl md:text-4xl font-bold text-primary-teal mb-4">
          {{ $t('purchase.title') }}
        </h1>
        <p class="text-lg text-gray-600 max-w-2xl mx-auto">
          {{ $t('purchase.subtitle') }}
        </p>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-2 gap-12">
        <!-- Purchase Form -->
        <div class="card animate-slide-up">
          <h2 class="text-2xl font-bold text-primary-teal mb-6">{{ $t('purchase.form.title') }}</h2>

          <form @submit.prevent="processPayment" class="space-y-6">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                {{ $t('purchase.form.email') }}
              </label>
              <input v-model="formData.email" type="email" required class="input-field"
                :placeholder="$t('purchase.form.emailPlaceholder')" />
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                {{ $t('purchase.form.whatsapp') }}
              </label>
              <input v-model="formData.whatsapp" type="tel" required class="input-field"
                :placeholder="$t('purchase.form.whatsappPlaceholder')" />
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">
                {{ $t('purchase.form.name') }}
              </label>
              <input v-model="formData.name" type="text" required class="input-field"
                :placeholder="$t('purchase.form.namePlaceholder')" />
            </div>

            <!-- Bold Payment Button -->
            <div class="bg-gray-50 p-6 rounded-xl">
              <h3 class="text-lg font-semibold text-primary-teal mb-4">
                {{ $t('purchase.payment.title') }}
              </h3>
              <div class="flex items-center justify-between mb-4">
                <span class="text-gray-700">{{ $t('purchase.payment.plan') }}</span>
                <span class="text-2xl font-bold text-primary-orange">$29.99</span>
              </div>

              <!-- Custom Payment Trigger -->
              <div id="bold-payment-button" class="mb-4">
                <button type="button" @click="openBoldCheckout" :disabled="loading" class="btn-primary w-full">
                  <span v-if="loading" class="flex items-center justify-center">
                    <div class="spinner w-5 h-5 mr-2"></div>
                    {{ $t('purchase.payment.processing') }}
                  </span>
                  <span v-else>
                    {{ $t('purchase.payment.payWithBold') }}
                  </span>
                </button>
              </div>

              <!-- Embedded container -->
              <div ref="boldEmbedContainer" class="mt-4"></div>

              <p class="text-sm text-gray-500 text-center">
                {{ $t('purchase.payment.securePayment') }}
              </p>
            </div>
          </form>
        </div>

        <!-- Plan Details -->
        <div class="card animate-slide-up" style="animation-delay: 0.2s">
          <h2 class="text-2xl font-bold text-primary-teal mb-6">{{ $t('purchase.plan.title') }}</h2>

          <div class="space-y-4 mb-8">
            <div class="flex items-center space-x-3">
              <CheckIcon class="w-5 h-5 text-green-500 flex-shrink-0" />
              <span class="text-gray-700">{{ $t('purchase.plan.features.whatsapp') }}</span>
            </div>
            <div class="flex items-center space-x-3">
              <CheckIcon class="w-5 h-5 text-green-500 flex-shrink-0" />
              <span class="text-gray-700">{{ $t('purchase.plan.features.ai') }}</span>
            </div>
            <div class="flex items-center space-x-3">
              <CheckIcon class="w-5 h-5 text-green-500 flex-shrink-0" />
              <span class="text-gray-700">{{ $t('purchase.plan.features.support') }}</span>
            </div>
            <div class="flex items-center space-x-3">
              <CheckIcon class="w-5 h-5 text-green-500 flex-shrink-0" />
              <span class="text-gray-700">{{ $t('purchase.plan.features.analytics') }}</span>
            </div>
            <div class="flex items-center space-x-3">
              <CheckIcon class="w-5 h-5 text-green-500 flex-shrink-0" />
              <span class="text-gray-700">{{ $t('purchase.plan.features.updates') }}</span>
            </div>
          </div>

          <div class="bg-primary-gray p-4 rounded-xl">
            <h3 class="font-semibold text-primary-teal mb-2">{{ $t('purchase.plan.whatHappensNext') }}</h3>
            <ol class="text-sm text-gray-600 space-y-2">
              <li>1. {{ $t('purchase.plan.steps.payment') }}</li>
              <li>2. {{ $t('purchase.plan.steps.account') }}</li>
              <li>3. {{ $t('purchase.plan.steps.email') }}</li>
              <li>4. {{ $t('purchase.plan.steps.setup') }}</li>
            </ol>
          </div>
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
import { CheckIcon } from '@heroicons/vue/24/outline'
import api from '../utils/api.js'

const router = useRouter()
const { t } = useI18n()
const toast = useToast()

const boldEmbedContainer = ref(null)
const loading = ref(false)
const formData = ref({
  email: '',
  whatsapp: '',
  name: ''
})

const openBoldCheckout = async () => {
  if (!formData.value.email || !formData.value.whatsapp || !formData.value.name) {
    toast.error(t('purchase.errors.fillAllFields'))
    return
  }

  try {
    loading.value = true

    // Load Bold script if not present
    if (!window.BoldCheckout) {
      await new Promise((resolve, reject) => {
        const script = document.createElement('script')
        script.src = 'https://checkout.bold.co/library/boldPaymentButton.js'
        script.onload = resolve
        script.onerror = reject
        document.head.appendChild(script)
      })
    }

    const { data } = await api.post('/api/BoldPayment/signature')
    const { orderId, amount, currency, signature } = data

    const checkout = new window.BoldCheckout({
      orderId,
      amount: amount.toString(),
      currency,
      apiKey: 'gb6KQPBVWgamX8lh6Z9DwBjub81jUiUw-bANSL-vwhE',
      integritySignature: signature,
      description: 'Suscripción mensual WhatsApp IA',
      redirectionUrl: 'https://avabot-3b5c6.web.app',
      renderMode: 'embedded',
      container: boldEmbedContainer.value,
      customerData: JSON.stringify({
        email: formData.value.email,
        fullName: formData.value.name,
        phone: formData.value.whatsapp,
        dialCode: '+57',
        documentNumber: '123456789',
        documentType: 'CC'
      })
    })

    boldEmbedContainer.value.innerHTML = ''
    checkout.open()
  } catch (err) {
    console.error(err)
    toast.error(t('purchase.errors.paymentFailed'))
  } finally {
    loading.value = false
  }
}
</script>
