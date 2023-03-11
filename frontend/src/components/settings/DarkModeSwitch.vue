<template>
  <SettingsSwitch :model-value="isDarkMode" @update:model-value="toggleDarkMode">
    <div class="tc-main text-xl">
      <Icon icon="ri:moon-fill" v-show="isDarkMode" />
      <Icon icon="ri:sun-fill" v-show="!isDarkMode" />
    </div>
  </SettingsSwitch>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { Icon } from '@/components/misc/Icon';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

import SettingsSwitch from './common/SettingsSwitch.vue';

const config = useConfig();
const isDarkMode = computed(() => config.get(ZwooConfigKey.UiMode) === 'dark');

const toggleDarkMode = () => {
  config.set(ZwooConfigKey.UiMode, !isDarkMode.value ? 'dark' : 'light');
};
</script>
