<template>
  <RuleSwitch :model-value="value" @update:model-value="toggle" />
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

import RuleSwitch from '../lobby/rules/contentTypes/RuleSwitch.vue';

const config = useConfig();
const value = computed(() => config.get(ZwooConfigKey.Sync));

const toggle = (enabled: boolean) => {
  config.set(ZwooConfigKey.Sync, enabled);
};
</script>
