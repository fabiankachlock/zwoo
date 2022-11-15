<template>
  <!-- reset stacking context -->
  <div class="relative">
    <router-view />
  </div>

  <ChangelogManager />
  <ConsentManager />
  <Snackbar />
</template>

<script setup lang="ts">
import { defineAsyncComponent, onMounted } from 'vue';

import ConsentManager from './components/cookies/ConsentManager.vue';
import Snackbar from './components/misc/Snackbar.vue';
import { useRootApp } from './core/adapter/app';
import { useAuth } from './core/adapter/auth';
import { useConfig } from './core/adapter/config';
import { useCookies } from './core/adapter/cookies';
const ChangelogManager = defineAsyncComponent(() => import('./components/misc/changelog/ChangelogManager.vue'));

useRootApp().configure(); // init app
useAuth().configure(); // 'read' from may existing session
const cookies = useCookies();
cookies.setup();
const asyncSetup = async () => {
  useConfig().configure(); // load stored config from localStorage
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
});

asyncSetup();
</script>
