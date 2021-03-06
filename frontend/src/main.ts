import { createApp } from 'vue';
import { createPinia } from 'pinia';
import App from './App.vue';
import './registerServiceWorker';
import router from './router';
import i18n from './i18n';
import './index.css';
import { Tooltip } from './custom/Tooltip';

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

const app = createApp(App);
app.use(createPinia());
app.use(i18n);
app.use(router);

app.directive('tooltip', Tooltip);

app.mount('#app');
