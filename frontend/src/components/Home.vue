<template>
  <div class="animate-fade-in">
    <!-- Hero Section -->
    <section class="gradient-bg text-white py-20">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-12 items-center">
          <div class="animate-slide-up">
            <h1 class="text-4xl md:text-6xl font-bold mb-6 leading-tight">
              {{ $t('home.hero.title') }}
            </h1>
            <p class="text-xl mb-8 text-gray-100">
              {{ $t('home.hero.subtitle') }}
            </p>
            <div class="flex flex-col sm:flex-row gap-4">
              <router-link to="/purchase" class="btn-primary bg-white text-primary-orange hover:bg-gray-100">
                {{ $t('home.hero.startNow') }}
              </router-link>
              <button class="btn-secondary border-2 border-white text-white hover:bg-white hover:text-primary-teal">
                {{ $t('home.hero.learnMore') }}
              </button>
            </div>
          </div>
          <div class="float-animation">
            <div class="relative">
              <div class="w-full h-96 bg-white bg-opacity-10 rounded-3xl flex items-center justify-center backdrop-blur-sm">
                <div class="text-center">
                  <CpuChipIcon class="w-24 h-24 mx-auto mb-4 text-white" />
                  <div class="space-y-2">
                    <div class="w-32 h-3 bg-white bg-opacity-30 rounded-full mx-auto"></div>
                    <div class="w-24 h-3 bg-white bg-opacity-30 rounded-full mx-auto"></div>
                    <div class="w-28 h-3 bg-white bg-opacity-30 rounded-full mx-auto"></div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Features Section -->
    <section class="py-20 bg-white">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-16">
          <h2 class="text-3xl md:text-4xl font-bold text-primary-teal mb-4">
            {{ $t('home.features.title') }}
          </h2>
          <p class="text-lg text-gray-600 max-w-2xl mx-auto">
            {{ $t('home.features.subtitle') }}
          </p>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
          <div class="card text-center animate-slide-up">
            <div class="w-16 h-16 bg-gradient-to-br from-primary-teal to-primary-orange rounded-2xl flex items-center justify-center mx-auto mb-6">
              <ChatBubbleLeftRightIcon class="w-8 h-8 text-white" />
            </div>
            <h3 class="text-xl font-semibold mb-4 text-primary-teal">{{ $t('home.features.whatsapp.title') }}</h3>
            <p class="text-gray-600">{{ $t('home.features.whatsapp.description') }}</p>
          </div>

          <div class="card text-center animate-slide-up" style="animation-delay: 0.2s">
            <div class="w-16 h-16 bg-gradient-to-br from-primary-teal to-primary-orange rounded-2xl flex items-center justify-center mx-auto mb-6">
              <CpuChipIcon class="w-8 h-8 text-white" />
            </div>
            <h3 class="text-xl font-semibold mb-4 text-primary-teal">{{ $t('home.features.ai.title') }}</h3>
            <p class="text-gray-600">{{ $t('home.features.ai.description') }}</p>
          </div>

          <div class="card text-center animate-slide-up" style="animation-delay: 0.4s">
            <div class="w-16 h-16 bg-gradient-to-br from-primary-teal to-primary-orange rounded-2xl flex items-center justify-center mx-auto mb-6">
              <ClockIcon class="w-8 h-8 text-white" />
            </div>
            <h3 class="text-xl font-semibold mb-4 text-primary-teal">{{ $t('home.features.support.title') }}</h3>
            <p class="text-gray-600">{{ $t('home.features.support.description') }}</p>
          </div>
        </div>
      </div>
    </section>

    <!-- Pricing Section -->
    <section class="py-20 bg-primary-gray">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-16">
          <h2 class="text-3xl md:text-4xl font-bold text-primary-teal mb-4">
            {{ $t('home.pricing.title') }}
          </h2>
          <p class="text-lg text-gray-600 max-w-2xl mx-auto">
            {{ $t('home.pricing.subtitle') }}
          </p>
        </div>

        <div class="max-w-lg mx-auto">
          <div class="card text-center border-2 border-primary-orange animate-slide-up">
            <div class="mb-6">
              <div class="w-20 h-20 bg-gradient-to-br from-primary-teal to-primary-orange rounded-full flex items-center justify-center mx-auto mb-4">
                <StarIcon class="w-10 h-10 text-white" />
              </div>
              <h3 class="text-2xl font-bold text-primary-teal mb-2">{{ $t('home.pricing.plan.title') }}</h3>
              <div class="text-4xl font-bold text-primary-orange mb-2">
                  <template v-if="planAmount !== null">
                    ${{ planAmount }}
                  </template>
                  <template v-else>
                    <svg
                      class="animate-spin h-6 w-6 text-primary-orange inline"
                      xmlns="http://www.w3.org/2000/svg"
                      fill="none"
                      viewBox="0 0 24 24">
                      <circle class="opacity-25" cx="12" cy="12" r="10"
                              stroke="currentColor" stroke-width="4"></circle>
                      <path class="opacity-75" fill="currentColor"
                            d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z"></path>
                    </svg>
                  </template>
              </div>
              <p class="text-gray-600">{{ $t('home.pricing.plan.period') }}</p>
            </div>

            <div class="space-y-4 mb-8">
              <div class="flex items-center justify-center space-x-2">
                <CheckIcon class="w-5 h-5 text-green-500" />
                <span class="text-gray-700">{{ $t('home.pricing.plan.features.whatsapp') }}</span>
              </div>
              <div class="flex items-center justify-center space-x-2">
                <CheckIcon class="w-5 h-5 text-green-500" />
                <span class="text-gray-700">{{ $t('home.pricing.plan.features.ai') }}</span>
              </div>
              <div class="flex items-center justify-center space-x-2">
                <CheckIcon class="w-5 h-5 text-green-500" />
                <span class="text-gray-700">{{ $t('home.pricing.plan.features.support') }}</span>
              </div>
              <div class="flex items-center justify-center space-x-2">
                <CheckIcon class="w-5 h-5 text-green-500" />
                <span class="text-gray-700">{{ $t('home.pricing.plan.features.analytics') }}</span>
              </div>
            </div>

            <router-link to="/purchase" class="btn-primary w-full">
              {{ $t('home.pricing.plan.cta') }}
            </router-link>
          </div>
        </div>
      </div>
    </section>

    <!-- CTA Section -->
    <section class="py-20 bg-primary-teal text-white">
      <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
        <h2 class="text-3xl md:text-4xl font-bold mb-6">
          {{ $t('home.cta.title') }}
        </h2>
        <p class="text-xl mb-8 text-gray-100">
          {{ $t('home.cta.subtitle') }}
        </p>
        <router-link to="/purchase" class="btn-primary bg-primary-orange hover:bg-opacity-90">
          {{ $t('home.cta.button') }}
        </router-link>
      </div>
    </section>
  </div>
</template>

<script setup>
import { 
  CpuChipIcon, 
  ChatBubbleLeftRightIcon, 
  ClockIcon, 
  StarIcon, 
  CheckIcon 
} from '@heroicons/vue/24/outline'

import { ref, onMounted } from 'vue'
import api from '../utils/api.js'

const planAmount = ref(null) 


onMounted(async () => {
  try {
    
    const { data } = await api.post('/api/BoldPayment/price-ava-bot')
    planAmount.value = data.amount
    } catch (err) {
      console.error('Error al cargar el precio:', err)
      planAmount.value = '' // fallback por si falla
    }
  })

</script>