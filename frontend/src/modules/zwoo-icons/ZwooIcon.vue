<template>
  <div v-html="icon"></div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';

import { IconCache } from './IconCache';

// eslint-disable-next-line no-undef
const props = defineProps<{
  icon: string;
}>();

const icon = ref<string | undefined>(undefined);

watch(
  () => props.icon,
  async newIcon => {
    const result = await IconCache.getIcon(newIcon);
    icon.value = result.icon;
  },
  { immediate: true }
);
</script>
