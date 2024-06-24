<template>
  <div class="w-full flex flex-row justify-between py-3 items-center bg-surface border border-transparent">
    <div class="h-full flex-1 mr-2 text-text flex justify-start items-center">
      <span>{{ title }}</span>
      <span v-if="status" class="text-sm mx-1 text-text-secondary">({{ status }})</span>
      <Environment show="online">
        <SettingsSyncToggle v-if="settingsKey && isLoggedIn" :settings-key="settingsKey"></SettingsSyncToggle>
      </Environment>
    </div>
    <div class="h-full flex items-center justify-end">
      <Tooltip v-if="tooltip" :title="tooltip">
        <slot></slot>
      </Tooltip>
      <slot v-else></slot>
    </div>
  </div>
  <div class="border-t border-border-light last:border-0"></div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import Tooltip from '@/components/misc/Tooltip.vue';
import { useAuth } from '@/core/adapter/auth';
import { ZwooConfigKey } from '@/core/adapter/config';

import Environment from '../misc/Environment.vue';
import SettingsSyncToggle from './SettingsSyncToggle.vue';

defineProps<{
  title: string;
  status?: string;
  tooltip?: string;
  settingsKey?: ZwooConfigKey;
}>();

const auth = useAuth();
const isLoggedIn = computed(() => auth.isLoggedIn);
</script>
