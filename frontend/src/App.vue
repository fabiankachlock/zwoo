<template>
  <template v-if="isLoading">
    <div>
      <Loader />
    </div>
  </template>
  <template v-else>
    <!-- reset stacking context -->
    <div class="relative">
      <router-view />
    </div>
  </template>

  <ChangelogManager />
  <ConsentManager />
  <Snackbar />
</template>

<script setup lang="ts">
import { computed, defineAsyncComponent, onMounted } from 'vue';

import ConsentManager from './components/cookies/ConsentManager.vue';
import Loader from './components/misc/Loader.vue';
import Snackbar from './components/misc/Snackbar.vue';
import { useRootApp } from './core/adapter/app';
import { useAuth } from './core/adapter/auth';
import { useConfig } from './core/adapter/config';
import { useCookies } from './core/adapter/cookies';
import { CardTheme } from './core/domain/cards/CardTheme';
import { useRuntimeConfig } from './core/runtimeConfig';
const ChangelogManager = defineAsyncComponent(() => import('./components/misc/changelog/ChangelogManager.vue'));

const app = useRootApp();
const isLoading = computed(() => app.isLoading);

app.configure(); // init app
useAuth().configure(); // 'read' from may existing session
const cookies = useCookies();
cookies.setup();
const asyncSetup = async () => {
  useConfig().configure(); // load stored config from localStorage
  useRuntimeConfig().configure();

  if (cookies.recaptchaCookie) {
    setTimeout(() => {
      cookies.loadRecaptcha();
    });
  }
};

onMounted(() => {
  import('./core/adapter/shortcuts/ShortcutManager').then(module => {
    module.ShortcutManager.global.activate();
  });

  setTimeout(() => {
    Promise.all([
      import('./core/domain/cards/ThemeManager'),
      import('./core/services/themeCache/BrowserCardThemeCache'),
      import('./core/helper/QueuedCache')
    ]).then(([{ CardThemeManager }, { BrowserCardThemeCache }, { AsyncQueuedCache }]) => {
      CardThemeManager.global = new CardThemeManager(new BrowserCardThemeCache(), new AsyncQueuedCache<CardTheme>(Infinity));
      import('./core/adapter/game/cardTheme').then(({ useCardTheme }) => useCardTheme().__init__());
    });
  });
});

asyncSetup();
</script>
