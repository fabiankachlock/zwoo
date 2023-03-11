<template>
  <button class="tc-primary ml-3 bg-lightest hover:bg-main p-0.5 rounded border border-transparent hover:bc-light" @click.prevent="toggle">
    <Icon v-show="!synced" icon="mdi:cloud-off-outline" class="text-lg" />
    <Icon v-show="synced" icon="mdi:cloud-sync-outline" class="text-lg" />
  </button>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

import { Icon } from '../../misc/Icon';

// eslint-disable-next-line no-undef
const props = defineProps<{
  settingsKey: ZwooConfigKey;
}>();

const config = useConfig();
const synced = computed(() => config.isSynced(props.settingsKey));

const toggle = () => {
  config.toggleIgnore(props.settingsKey);
};
</script>
