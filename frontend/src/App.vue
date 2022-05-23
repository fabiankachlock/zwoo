<template>
  <!-- reset stacking context -->
  <div class="relative">
    <router-view />
  </div>

  <ConsentManager />
  <Snackbar />
</template>

<script setup lang="ts">
import { useAuth } from './core/adapter/auth';
import { useConfig } from './core/adapter/config';
import ConsentManager from './components/cookies/ConsentManager.vue';
import { useCookies } from './core/adapter/cookies';
import Snackbar from './components/misc/Snackbar.vue';
import { onMounted, onUnmounted } from 'vue';
import type { ShortcutManager } from './core/adapter/shortcuts/ShortcutManager';

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

let shortcutManager: ShortcutManager | undefined;
onMounted(() => {
  import(/* webpackChunkName: "shortcuts" */ './core/adapter/shortcuts/ShortcutManager').then(module => {
    shortcutManager = new module.ShortcutManager();
    setTimeout(() => {
      shortcutManager?.activate();
    });
  });
});

onUnmounted(() => {
  shortcutManager?.deActivate();
});

asyncSetup();
</script>
