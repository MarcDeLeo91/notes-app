// filepath: c:\Users\macot\Documents\Repositorios\notes-app\frontend\vite.config.js
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    host: 'localhost', // Asegura que el servidor escuche en localhost
    port: 5173, // Puerto del servidor de desarrollo
    hmr: {
      protocol: 'ws', // Usa WebSocket para HMR
      host: 'localhost', // Host para las conexiones HMR
      port: 5173, // Puerto explícito para HMR
    },
  },
})