<template>
  <div class="relative w-full h-full max-w-full max-h-full">
    <img
      v-for="(layer, i) in cardData.layers"
      :src="layer"
      :key="i"
      :alt="`${cardData.description} - layer ${i}`"
      class="absolute"
      :class="imageClass"
      :style="`z-index: ${i + 1};${imageStyle}`"
    />
  </div>
</template>

<script setup lang="ts">
import { useCardTheme } from '@/core/adapter/play/cardTheme';
import { CardDescriptor } from '@/core/services/cards/CardThemeConfig';
import { Card } from '@/core/services/game/card';
import { computed, defineProps } from 'vue';

const props = defineProps<{
  card: Card | CardDescriptor;
  imageStyle?: string;
  imageClass?: string;
}>();

const cardTheme = useCardTheme();
const cardData = computed(() => cardTheme.theme.getCard(props.card));
</script>
