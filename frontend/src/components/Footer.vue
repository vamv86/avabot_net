<template>
  <footer class="bg-primary-teal text-white mt-20">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-8">
        <!-- Company Info -->
        <div class="space-y-4">
          <div class="flex items-center space-x-2">
            <div class="w-8 h-8 bg-primary-orange rounded-lg flex items-center justify-center">
              <CpuChipIcon class="w-5 h-5 text-white" />
            </div>
            <span class="text-xl font-bold">AI Agent</span>
          </div>
          <p class="text-gray-300">{{ $t('footer.description') }}</p>
        </div>

        <!-- Links -->
        <div>
          <h3 class="font-semibold mb-4">{{ $t('footer.links') }}</h3>
          <div class="space-y-2">
            <router-link to="/" class="block text-gray-300 hover:text-white transition-colors">
              {{ $t('nav.home') }}
            </router-link>
            <router-link to="/purchase" class="block text-gray-300 hover:text-white transition-colors">
              {{ $t('nav.purchase') }}
            </router-link>
            <a href="#" class="block text-gray-300 hover:text-white transition-colors">
              {{ $t('footer.support') }}
            </a>
          </div>
        </div>

        <!-- Features -->
        <div>
          <h3 class="font-semibold mb-4">{{ $t('footer.features') }}</h3>
          <div class="space-y-2">
            <p class="text-gray-300">{{ $t('footer.whatsappIntegration') }}</p>
            <p class="text-gray-300">{{ $t('footer.aiPowered') }}</p>
            <p class="text-gray-300">{{ $t('footer.support247') }}</p>
          </div>
        </div>

        <!-- Contact -->
        <div>
          <h3 class="font-semibold mb-4">{{ $t('footer.contact') }}</h3>
          <div class="space-y-2">
            <p class="text-gray-300">{{ $t('footer.email') }}: support@aiagent.com</p>
            <p class="text-gray-300">{{ $t('footer.phone') }}: +1 (555) 123-4567</p>
          </div>
        </div>
      </div>

      <div class="border-t border-gray-600 mt-8 pt-8 text-center">
        <p class="text-gray-300">© 2024 AI Agent. {{ $t('footer.allRightsReserved') }}</p>
      </div>
    </div>
  </footer>
</template>

<script setup>
import { CpuChipIcon } from '@heroicons/vue/24/outline'
</script>