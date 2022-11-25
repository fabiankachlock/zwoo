<template>
  <div class="relative w-full h-full select-none pointer-events-none" :ref="forwardRef">
    <img
      v-for="(layer, i) in cardData.layers"
      :src="layer"
      :key="i"
      :alt="`${cardData.description} - layer ${i}`"
      class="zwoo-card"
      :class="imageClass"
      :style="`z-index: ${i + 1};${imageStyle}`"
    />
  </div>
</template>

<script setup lang="ts">
import { ComponentPublicInstance, computed, defineProps, Ref } from 'vue';

import { useCardTheme } from '@/core/adapter/play/cardTheme';
import { CardTheme } from '@/core/services/cards/CardTheme';
import { CardDescriptor } from '@/core/services/cards/CardThemeConfig';
import { Card } from '@/core/services/game/CardTypes';

const props = defineProps<{
  card:
    | CardDescriptor
    | string
    | Card
    | {
        color: number;
        type: number;
      };
  imageStyle?: string;
  imageClass?: string;
  overrideTheme?: CardTheme;
  forwardRef?: string | Ref | ((ref: Element | ComponentPublicInstance | null) => void);
}>();

const cardTheme = useCardTheme();
const cardData = computed(() => (props.overrideTheme ?? cardTheme.theme).getCard(props.card));
</script>
<style scoped>
.zwoo-card:not(:first-of-type) {
  @apply absolute top-0 left-1/2 -translate-x-1/2;
}
</style>
