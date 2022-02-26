<template>
  <div class="max-height-ios relative">
    <router-view />
  </div>

  <ConsentManager />
</template>

<script setup lang="ts">
import { useAuth } from './core/adapter/auth';
import { useConfig } from './core/adapter/config';
import ConsentManager from './components/cookies/ConsentManager.vue';
import { useCookies } from './core/adapter/cookies';

useConfig().configure(); // load stored config from localStorage
useAuth().configure(); // 'read' from may existing session
const cookies = useCookies();
cookies.setup();

const asyncSetup = async () => {
  if (cookies.recaptchaCookie) {
    cookies.loadRecaptcha();
  }
};

asyncSetup();
</script>
