<template>
  <button @click="toggleDarkMode" class="tc-main text-xl transform ease-linear transition-transform hover:scale-110">
    <Icon icon="ri:moon-fill" v-show="!isDarkMode" />
    <Icon icon="ri:sun-fill" v-show="isDarkMode" />
  </button>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
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
