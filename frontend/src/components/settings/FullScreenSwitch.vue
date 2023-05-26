<template>
  <SettingsSwitch :model-value="useFullScreen" @update:model-value="toggleFullScreen">
    <div class="tc-main text-xl">
      <Icon v-show="useFullScreen" icon="mdi:fullscreen" />
      <Icon v-show="!useFullScreen" icon="mdi:fullscreen-exit" />
    </div>
  </SettingsSwitch>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { Icon } from '@/components/misc/Icon';
import { useConfig } from '@/core/adapter/config';

import SettingsSwitch from './common/SettingsSwitch.vue';

const config = useConfig();
const useFullScreen = computed(() => config.useFullScreen);

const toggleFullScreen = () => {
  config.setFullScreen(!useFullScreen.value);
};
</script>
