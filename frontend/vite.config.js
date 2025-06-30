import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { resolve } from 'path'

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': resolve(__dirname, 'src')
    }
  },
  server: {
    port: 3000,
    proxy: {
      '/api': {
        target: 'https://localhost:58402',
        changeOrigin: true,
        secure: false // Para desarrollo con certificados auto-firmados
      }
    }
  },
  define: {
    // Hacer las variables de entorno disponibles en el cliente
    __API_URL__: JSON.stringify(process.env.VITE_API_URL || 'https://localhost:58402')
  }
})