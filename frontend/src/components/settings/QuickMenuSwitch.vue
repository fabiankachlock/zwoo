<template>
  <button @click="toggleQuickMenu" class="tc-main text-xl ease-linear transition-transform hover:scale-110">
    <Icon icon="mdi:eye-outline" v-show="!showQuickMenu" />
    <Icon icon="mdi:eye-off-outline" v-show="showQuickMenu" />
  </button>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { Icon } from '@/components/misc/Icon';
import { useConfig } from '@/core/adapter/config';

const config = useConfig();
const showQuickMenu = computed(() => config.showQuickMenu);

const toggleQuickMenu = () => {
  config.setQuickMenu(!showQuickMenu.value);
};
</script>
