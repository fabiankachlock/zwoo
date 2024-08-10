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
      const excludes = (props.exclude ?? []).includes(currentPlatform);
      const includes = (props.include ?? []).includes(currentPlatform);
      if ((!excludes && !includes) || includes) {
        show.value = true;
      } else {
        show.value = !excludes;
      }
    });
  }
});
</script>
<template>
  <slot v-if="show"></slot>
</template>
