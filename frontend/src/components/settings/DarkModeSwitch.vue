<template>
  <button @click="toggleDarkMode" class="tc-main text-xl ease-linear transition-transform hover:scale-110">
    <Icon icon="ri:moon-fill" v-show="!isDarkMode" />
    <Icon icon="ri:sun-fill" v-show="isDarkMode" />
  </button>
</template>

<script setup lang="ts">
import { useConfig } from '@/core/adapter/config';
import { Icon } from '@iconify/vue';
import { computed } from 'vue';

const config = useConfig();
const isDarkMode = computed(() => config.useDarkMode);

const toggleDarkMode = () => {
  config.setDarkMode(!isDarkMode.value);
};
</script>
