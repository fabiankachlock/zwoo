<template>
  <button @click="toggleQuickMenu" class="tc-main text-xl ease-linear transition-transform hover:scale-110">
    <Icon icon="mdi:eye-outline" v-show="!showQuickMenu" />
    <Icon icon="mdi:eye-off-outline" v-show="showQuickMenu" />
  </button>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { Icon } from '@/components/misc/Icon';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

const config = useConfig();
const showQuickMenu = computed(() => config.get(ZwooConfigKey.QuickMenu));

const toggleQuickMenu = () => {
  config.set(ZwooConfigKey.QuickMenu, !showQuickMenu.value);
};
</script>
