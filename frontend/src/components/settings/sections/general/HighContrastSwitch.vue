<template>
  <SettingsSwitch :model-value="isHighContrast" @update:model-value="toggleDarkMode">
    <div class="text-text text-xl">
      <Icon v-show="isHighContrast" icon="ri:contrast-fill" />
      <Icon v-show="!isHighContrast" icon="ri:circle-line" />
    </div>
  </SettingsSwitch>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { Icon } from '@/components/misc/Icon';
import SettingsSwitch from '@/components/settings/SettingsSwitch.vue';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

const config = useConfig();
const isHighContrast = computed(() => config.get(ZwooConfigKey.UiContrastMode));

const toggleDarkMode = () => {
  config.set(ZwooConfigKey.UiContrastMode, !isHighContrast.value);
};
</script>
