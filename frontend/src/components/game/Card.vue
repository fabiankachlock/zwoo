<template>
  <div class="relative w-full h-full select-none pointer-events-none">
    <img
      v-for="(layer, i) in cardData.layers"
      :key="i"
      :src="layer"
      :alt="`${cardData.description} - layer ${i}`"
      class="zwoo-card"
      :class="imageClass"
      :style="`z-index: ${i + 1};${imageStyle}`"
    />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { useCardTheme } from '@/core/adapter/game/cardTheme';
import { CardTheme } from '@/core/domain/cards/CardTheme';
import { CardDescriptor } from '@/core/domain/cards/CardThemeConfig';
import { Card } from '@/core/domain/game/CardTypes';

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
}>();

const cardTheme = useCardTheme();
const cardData = computed(() => (props.overrideTheme ?? cardTheme.theme).getCard(props.card));
</script>
<style scoped>
.zwoo-card:not(:first-of-type) {
  @apply absolute top-0 left-1/2 -translate-x-1/2;
}
</style>
