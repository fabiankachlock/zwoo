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
import { useAuth } from './core/adapter/auth';
import { useConfig } from './core/adapter/config';
import { useCookies } from './core/adapter/cookies';
const ChangelogManager = defineAsyncComponent(() => import(/* webpackChunkName: "changelog" */ '././components/misc/changelog/ChangelogManager.vue'));

useConfig().configure(); // load stored config from localStorage
useAuth().configure(); // 'read' from may existing session
const cookies = useCookies();
cookies.setup();
const asyncSetup = async () => {
  if (cookies.recaptchaCookie) {
    setTimeout(() => {
      cookies.loadRecaptcha();
    });
  }
};

onMounted(() => {
  import(/* webpackChunkName: "shortcuts" */ './core/adapter/shortcuts/ShortcutManager').then(module => {
    module.ShortcutManager.global.activate();
  });
});

asyncSetup();
</script>
