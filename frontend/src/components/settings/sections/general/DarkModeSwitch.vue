<template>
  <SettingsSwitch :model-value="isDarkMode" @update:model-value="toggleDarkMode">
    <div class="tc-main text-xl">
      <Icon v-show="isDarkMode" icon="ri:moon-fill" />
      <Icon v-show="!isDarkMode" icon="ri:sun-fill" />
    </div>
  </SettingsSwitch>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { Icon } from '@/components/misc/Icon';
import SettingsSwitch from '@/components/settings/SettingsSwitch.vue';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

const config = useConfig();
const isDarkMode = computed(() => config.get(ZwooConfigKey.UiMode) === 'dark');

const toggleDarkMode = () => {
  config.set(ZwooConfigKey.UiMode, !isDarkMode.value ? 'dark' : 'light');
};
</script>
