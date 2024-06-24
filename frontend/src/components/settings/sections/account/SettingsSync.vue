<template>
  <SettingsSwitch :model-value="value" @update:model-value="toggle">
    <div class="text-text text-lg">
      <Icon v-show="value" icon="mdi:cloud-sync-outline" />
      <Icon v-show="!value" icon="mdi:cloud-off-outline" />
    </div>
  </SettingsSwitch>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { Icon } from '@/components/misc/Icon';
import SettingsSwitch from '@/components/settings/SettingsSwitch.vue';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

const config = useConfig();
const value = computed(() => config.get(ZwooConfigKey.Sync));

const toggle = (enabled: boolean) => {
  config.set(ZwooConfigKey.Sync, enabled);
};
</script>
