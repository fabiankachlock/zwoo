<template>
  <SettingsSwitch :model-value="showQuickMenu" @update:model-value="toggleQuickMenu">
    <div class="text-text text-xl">
      <Icon v-show="showQuickMenu" icon="mdi:eye-outline" />
      <Icon v-show="!showQuickMenu" icon="mdi:eye-off-outline" />
    </div>
  </SettingsSwitch>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { Icon } from '@/components/misc/Icon';
import SettingsSwitch from '@/components/settings/SettingsSwitch.vue';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

const config = useConfig();
const showQuickMenu = computed(() => config.get(ZwooConfigKey.QuickMenu));

const toggleQuickMenu = () => {
  config.set(ZwooConfigKey.QuickMenu, !showQuickMenu.value);
};
</script>
