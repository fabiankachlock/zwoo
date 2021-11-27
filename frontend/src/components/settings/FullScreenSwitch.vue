<template>
  <button @click="toggleFullScreen" class="tc-main text-xl transform ease-linear transition-transform hover:scale-110">
    <Icon icon="mdi:fullscreen" v-show="!useFullScreen" />
    <Icon icon="mdi:fullscreen-exit" v-show="useFullScreen" />
  </button>
</template>

<script setup lang="ts">
import { useConfig } from '@/core/adapter/config';
import { Icon } from '@iconify/vue';
import { computed } from 'vue';

const config = useConfig();
const useFullScreen = computed(() => config.useFullScreen);

const toggleFullScreen = () => {
  config.setFullScreen(!useFullScreen.value);
};
</script>
