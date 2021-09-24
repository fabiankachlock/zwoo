<template>
  <router-view />
</template>

<script setup lang="ts">
import { defaultLanguage, languageKey } from '@/i18n';
import { onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
const { locale } = useI18n();

onMounted(() => {
  const stored = localStorage.getItem(languageKey);
  if (stored) {
    locale.value = stored;
  } else {
    localStorage.setItem(languageKey, defaultLanguage);
  }
});
</script>
