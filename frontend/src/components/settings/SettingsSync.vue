<template>
  <SettingsSwitch :model-value="value" @update:model-value="toggle">
    <div class="tc-main text-lg">
      <Icon v-show="value" icon="mdi:cloud-sync-outline" />
      <Icon v-show="!value" icon="mdi:cloud-off-outline" />
    </div>
  </SettingsSwitch>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

import { Icon } from '../misc/Icon';
import SettingsSwitch from './common/SettingsSwitch.vue';

const config = useConfig();
const value = computed(() => config.get(ZwooConfigKey.Sync));

const toggle = (enabled: boolean) => {
  config.set(ZwooConfigKey.Sync, enabled);
};
</script>
