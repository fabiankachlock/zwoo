<script setup lang="ts">
import { computed } from 'vue';

import { useRootApp } from '@/core/adapter/app';

const props = defineProps<{
  show?: 'offline' | 'online' | 'local';
  exclude?: ('offline' | 'online' | 'local')[];
  include?: ('offline' | 'online' | 'local')[];
}>();

const app = useRootApp();
const env = computed(() => app.environment);
const exclude = computed(() => props.exclude);
const include = computed(() => props.include);
</script>

<template>
  <template v-if="(show === env || (include ?? []).includes(env)) && exclude && !(exclude ?? []).includes(env)">
    <slot></slot>
  </template>
</template>
