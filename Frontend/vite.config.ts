import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [
    vue(),
    tailwindcss(),
  ],
  server: {
    // In dev mode, proxy API and uploads to the backend.
    // In Docker, nginx handles this instead.
    proxy: {
      '/api':     'http://localhost:8080',
      '/uploads': 'http://localhost:8080',
    },
  },
})
