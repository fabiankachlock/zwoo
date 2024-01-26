<template>
  <div id="icon" :ref="ref => (elementRef = ref as HTMLDivElement)" v-html="iconValue"></div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';

import { IconCache } from './IconCache';

// eslint-disable-next-line no-undef
const props = defineProps<{
  icon: string;
  svgClass?: string;
}>();

const iconValue = ref<string | undefined>(undefined);
const elementRef = ref<HTMLDivElement | undefined>(undefined);

watch(
  () => props.icon,
  async newIcon => {
    const result = await IconCache.getIcon(newIcon);
    iconValue.value = result.icon;
    setTimeout(() => {
      if (elementRef.value && elementRef.value.children.length > 0) {
        elementRef.value.children[0].setAttribute('class', props.svgClass ?? '');
      }
    }, 0);
  },
  { immediate: true }
);

watch(
  [elementRef, () => props.svgClass],
  ([elm, childClass]) => {
    if (elm && elm.children.length > 0) {
      elm.children[0].setAttribute('class', childClass ?? '');
    }
  },
  { immediate: true }
);
</script>
