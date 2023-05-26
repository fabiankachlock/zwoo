<template>
  <SettingsSwitch :model-value="showQuickMenu" @update:model-value="toggleQuickMenu">
    <div class="tc-main text-xl">
      <Icon v-show="showQuickMenu" icon="mdi:eye-outline" />
      <Icon v-show="!showQuickMenu" icon="mdi:eye-off-outline" />
    </div>
  </SettingsSwitch>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { Icon } from '@/components/misc/Icon';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

import SettingsSwitch from './common/SettingsSwitch.vue';

const config = useConfig();
const showQuickMenu = computed(() => config.get(ZwooConfigKey.QuickMenu));

const toggleQuickMenu = () => {
  config.set(ZwooConfigKey.QuickMenu, !showQuickMenu.value);
};
</script>
