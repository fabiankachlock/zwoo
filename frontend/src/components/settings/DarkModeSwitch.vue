<template>
  <button @click="toggleDarkMode" class="tc-main text-xl transform ease-linear transition-transform hover:scale-110">
    <Icon icon="ri:moon-fill" v-show="!isDarkMode" />
    <Icon icon="ri:sun-fill" v-show="isDarkMode" />
  </button>
</template>

<script setup lang="ts">
import { useConfig } from '@/stores/config';
import { Icon } from '@iconify/vue';
import { ref } from 'vue';

const config = useConfig();
const isDarkMode = ref(config.useDarkMode);

const toggleDarkMode = () => {
  isDarkMode.value = !isDarkMode.value;
  config.setDarkMode(isDarkMode.value);
};
</script>
