<template>
  <button @click="toggleDarkMode">use {{ isDarkMode ? 'light' : 'dark' }}</button>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
const isDarkMode = ref(false);
const storageKey = 'zwoo:ui';
const stored = localStorage.getItem(storageKey);

const setDarkMode = (isOn: boolean) => {
  if (isOn) {
    document.body.classList.add('dark');
  } else {
    document.body.classList.remove('dark');
  }
};

const toggleDarkMode = () => {
  isDarkMode.value = !isDarkMode.value;
};

if (stored) {
  const isDark = stored === 'dark';
  isDarkMode.value = isDark;
} else {
  const userPreference = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
  isDarkMode.value = userPreference;
}

setDarkMode(isDarkMode.value);

watch(isDarkMode, () => {
  localStorage.setItem(storageKey, isDarkMode.value ? 'dark' : 'ligth');
  setDarkMode(isDarkMode.value);
});
</script>
