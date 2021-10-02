<template>
  <button @click="toggleQuickMenu" class="tc-main text-xl transform ease-linear transition-transform hover:scale-110">
    <Icon icon="mdi:eye-outline" v-show="!showQuickMenu" />
    <Icon icon="mdi:eye-off-outline" v-show="showQuickMenu" />
  </button>
</template>

<script setup lang="ts">
import { useConfig } from '@/core/adapter/config';
import { Icon } from '@iconify/vue';
import { ref } from 'vue';

const config = useConfig();
const showQuickMenu = ref(config.showQuickMenu);

const toggleQuickMenu = () => {
  showQuickMenu.value = !showQuickMenu.value;
  config.setQuickMenu(showQuickMenu.value);
};
</script>
