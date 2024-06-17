<template>
  <button class="text-primary-text ml-3 bg-alt hover:bg-alt-hover p-0.5 rounded border border-border" @click.prevent="toggle">
    <Icon v-show="!synced" icon="mdi:cloud-off-outline" class="text-lg" />
    <Icon v-show="synced" icon="mdi:cloud-sync-outline" class="text-lg" />
  </button>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { Icon } from '@/components/misc/Icon';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

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
