<template>
  <SettingsSwitch :model-value="showQuickMenu" @update:model-value="toggleQuickMenu">
    <div class="tc-main text-xl">
      <Icon icon="mdi:eye-outline" v-show="showQuickMenu" />
      <Icon icon="mdi:eye-off-outline" v-show="!showQuickMenu" />
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
