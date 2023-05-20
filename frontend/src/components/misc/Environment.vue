<script setup lang="ts">
import { computed } from 'vue';

import { useRootApp } from '@/core/adapter/app';

const props = defineProps<{
  show?: 'offline' | 'online';
  exclude?: ('offline' | 'online')[];
  include?: ('offline' | 'online')[];
}>();

const app = useRootApp();
const env = computed(() => app.environment);
const exclude = computed(() => props.exclude);
const include = computed(() => props.include);
</script>

<template>
  <template v-if="show === env || (exclude && !(exclude ?? []).includes(env)) || (include ?? []).includes(env)">
    <slot></slot>
  </template>
</template>
