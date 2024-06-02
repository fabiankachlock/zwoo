<template>
  <div
    class="w-full flex flex-row justify-between items-center bg-dark px-1 py-3 my-3 rounded-lg border border-transparent mouse:hover:bc-primary mouse:hover:bg-darkest"
  >
    <div class="h-full flex-1 mx-2 tc-main-light flex justify-start items-center">
      <span>{{ title }}</span>
      <span v-if="status" class="text-sm mx-1 tc-main-secondary">({{ status }})</span>
      <Environment show="online">
        <SettingsSyncToggle v-if="settingsKey && isLoggedIn" :settings-key="settingsKey"></SettingsSyncToggle>
      </Environment>
    </div>
    <div class="mx-2 h-full flex items-center justify-end">
      <Tooltip v-if="tooltip" :title="tooltip">
        <slot></slot>
      </Tooltip>
      <slot v-else></slot>
    </div>
  </div>
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
