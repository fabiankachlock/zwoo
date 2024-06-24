import './registerServiceWorker';
import './index.css';
import './fonts.css';

import { createPinia } from 'pinia';
import { registerSW } from 'virtual:pwa-register';
import { createApp } from 'vue';

import App from './App.vue';
import { useRootApp } from './core/adapter/app';
import { RouterService } from './core/global/Router';
import { Tooltip } from './directives/tooltip/Tooltip';
import i18n from './i18n';
import router from './router';

(() => {
  /* generate unique device id */
  const existingId = localStorage.getItem('zwoo:did');
  if (existingId) {
    window.DEVICE_ID = existingId;
  } else {
    let id = '';
    const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    while (id.length < 12) {
      id += chars[Math.floor(Math.random() * chars.length)];
    }
    localStorage.setItem('zwoo:did', id);
    window.DEVICE_ID = id;
  }
})();

RouterService.registerRouter(router);

const app = createApp(App);
app.use(createPinia());
app.use(i18n);
app.use(router);

app.directive('tooltip', Tooltip);

app.mount('#app');

router.isReady().then(async () => {
  const app = useRootApp();
  const updateSW = registerSW({
    immediate: true,
    onNeedRefresh() {
      app.onNeedsRefresh();
    },
    onOfflineReady() {
      app.onOfflineReady();
    }
  });
  app._setUpdateFunc(updateSW);
});
