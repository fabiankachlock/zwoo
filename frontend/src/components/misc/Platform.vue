<script setup lang="ts">
import { onMounted, ref } from 'vue';

import { AppConfig } from '@/config';

const props = defineProps<{
  include?: string[];
  exclude?: string[];
}>();

const show = ref(!AppConfig.IsTauri);

onMounted(() => {
  if (AppConfig.IsTauri) {
    import('@tauri-apps/plugin-os').then(async ({ platform }) => {
      const currentPlatform = await platform();
      if ((props.exclude ?? []).includes(currentPlatform)) {
        show.value = false;
      } else {
        show.value = (props.include ?? []).includes(currentPlatform);
      }
    });
  }
});
</script>
<template>
  <slot v-if="show"></slot>
</template>
