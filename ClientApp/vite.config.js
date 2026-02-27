import vue from '@vitejs/plugin-vue'
import {fileURLToPath, URL} from 'node:url'
import {defineConfig} from 'vite'
import vueDevTools from 'vite-plugin-vue-devtools'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    vueDevTools(),
  ],
  resolve: {
    alias: {'@': fileURLToPath(new URL('./src', import.meta.url))},
  },
  build: {
    outDir: '../HttpService/wwwroot',
    emptyOutDir: true,
    minify: false,  // This disables minification entirely
  },
  define: {__VUE_PROD_DEVTOOLS__: true}
})
