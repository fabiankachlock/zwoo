import vueI18n from '@intlify/unplugin-vue-i18n/vite';
import vue from '@vitejs/plugin-vue';
import path from 'path';
import { visualizer } from 'rollup-plugin-visualizer';
import { defineConfig, UserConfig } from 'vite';
import { VitePWA } from 'vite-plugin-pwa';

const manifestIcons = [
  {
    src: 'android-chrome-192x192.png',
    sizes: '192x192',
    type: 'image/png',
    purpose: 'any'
  },
  {
    src: 'android-chrome-512x512.png',
    sizes: '512x512',
    type: 'image/png',
    purpose: 'any'
  },
  {
    src: 'android-chrome-maskable-192x192.png',
    sizes: '192x192',
    type: 'image/png',
    purpose: 'maskable'
  },
  {
    src: 'android-chrome-maskable-512x512.png',
    sizes: '512x512',
    type: 'image/png',
    purpose: 'maskable'
  },
  {
    src: 'apple-touch-icon-180x180.png',
    sizes: '180x180',
    type: 'image/png',
    purpose: 'any'
  },
  {
    src: 'coast-228x228.png',
    sizes: '228x228',
    type: 'image/png',
    purpose: 'any'
  },
  {
    src: 'favicon-16x16.png',
    sizes: '16x16',
    type: 'image/png',
    purpose: 'any'
  },
  {
    src: 'favicon-32x32.png',
    sizes: '32x32',
    type: 'image/png',
    purpose: 'any'
  },
  {
    src: 'favicon-48x48.png',
    sizes: '48x48',
    type: 'image/png',
    purpose: 'any'
  },
  {
    src: 'favicon-64x64.png',
    sizes: '64x64',
    type: 'image/png',
    purpose: 'any'
  },
  {
    src: 'favicon-96x96.png',
    sizes: '96x96',
    type: 'image/png',
    purpose: 'any'
  },
  {
    src: 'favicon-128x128.png',
    sizes: '128x128',
    type: 'image/png',
    purpose: 'any'
  },
  {
    src: 'favicon-256x256.png',
    sizes: '256x256',
    type: 'image/png',
    purpose: 'any'
  }
].map(icon => ({
  ...icon,
  src: `/img/icons/${icon.src}`
}));

// TODO: configure reload (settings and invalid version): https://vite-pwa-org.netlify.app/guide/prompt-for-update.html

// https://vitejs.dev/config/
export default defineConfig(
  ({ mode }) =>
    ({
      plugins: [
        vue(),
        vueI18n({
          compositionOnly: true,
          include: path.resolve(__dirname, './src/locales/**')
        }),
        VitePWA({
          injectRegister: 'inline',
          strategies: 'generateSW',
          manifest: {
            id: '@zwoo/zwoo',
            name: 'zwoo',
            description: 'zwoo - The Second Challenge.',
            background_color: '#404254',
            orientation: 'any',
            short_name: 'zwoo',
            theme_color: '#3066BE',
            start_url: '.',
            display: 'standalone',
            icons: manifestIcons
          },
          workbox: {
            globIgnores: ['**/config.json'], // dont cache config.json, since it should be dynamic
            globPatterns: ['**/*.{js,css,html,ico,png,svg,json}', '**/wasm/**'],
            cleanupOutdatedCaches: true,
            maximumFileSizeToCacheInBytes: 10 * 1024 * 1024 // dont cache more than 10 mib
          }
        }),
        ...(process.env.ANALYZE !== undefined
          ? [
              visualizer({
                open: true,
                template: 'treemap'
              })
            ]
          : [])
      ],
      envPrefix: 'VUE_APP',
      envDir: 'env',
      server: {
        port: 8080
      },
      build: {
        sourcemap: mode === 'dev-instance'
      },
      resolve: {
        extensions: ['.mjs', '.js', '.ts', '.jsx', '.tsx', '.json', '.vue'],
        alias: {
          '@': path.resolve(__dirname, './src')
        }
      }
    }) as UserConfig
);
