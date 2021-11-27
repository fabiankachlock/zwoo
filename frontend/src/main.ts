import { createApp } from 'vue';
import { createPinia } from 'pinia';
import App from './App.vue';
import './registerServiceWorker';
import router from './router';
import i18n from './i18n';
import './index.css';
import { Tooltip } from './custom/Tooltip';

const app = createApp(App);
app.use(createPinia());
app.use(i18n);
app.use(router);

app.directive('tooltip', Tooltip);

app.mount('#app');

