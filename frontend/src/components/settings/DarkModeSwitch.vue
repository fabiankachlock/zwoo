<template>
  <button @click="toggleDarkMode" class="tc-main text-xl ease-linear transition-transform hover:scale-110">
    <Icon icon="ri:moon-fill" v-show="!isDarkMode" />
    <Icon icon="ri:sun-fill" v-show="isDarkMode" />
  </button>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { Icon } from '@/components/misc/Icon';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

const config = useConfig();
const isDarkMode = computed(() => config.get(ZwooConfigKey.UiMode) === 'dark');

const toggleDarkMode = () => {
  config.set(ZwooConfigKey.UiMode, !isDarkMode.value ? 'dark' : 'light');
};
</script>
