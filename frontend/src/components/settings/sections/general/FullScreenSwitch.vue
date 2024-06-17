<template>
  <SettingsSwitch :model-value="useFullScreen" @update:model-value="toggleFullScreen">
    <div class="text-text text-xl">
      <Icon v-show="useFullScreen" icon="mdi:fullscreen" />
      <Icon v-show="!useFullScreen" icon="mdi:fullscreen-exit" />
    </div>
  </SettingsSwitch>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { Icon } from '@/components/misc/Icon';
import SettingsSwitch from '@/components/settings/SettingsSwitch.vue';
import { useConfig } from '@/core/adapter/config';

const config = useConfig();
const useFullScreen = computed(() => config.useFullScreen);

const toggleFullScreen = () => {
  config.setFullScreen(!useFullScreen.value);
};
</script>
