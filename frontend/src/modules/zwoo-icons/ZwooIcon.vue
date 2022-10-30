<template>
  <Icon v-if="icon" :icon="icon" inline />
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue/offline';
import { ref, watch } from 'vue';

import { IconCache } from './IconCache';

// eslint-disable-next-line no-undef
const props = defineProps<{
  icon: string;
}>();

const icon = ref<any>(undefined);

watch(
  () => props.icon,
  async newIcon => {
    const result = await IconCache.getIcon(newIcon);
    //if (result.cacheHit) {
    //  addIcon(result.icon);
    //}
    icon.value = result.icon;
  },
  { immediate: true }
);
</script>
