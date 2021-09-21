import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import components from 'vite-plugin-components';
import icons, { ViteIconsResolver } from 'vite-plugin-icons';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    components({
      customComponentResolvers: ViteIconsResolver({
        componentPrefix: 'icon'
      })
    }),
    icons()
  ],
  clearScreen: false,
  build: {
    outDir: '../dist'
  },
  server: {
    proxy: {
      '/api': {
        target: '',
        changeOrigin: true
      }
    }
  }
});
