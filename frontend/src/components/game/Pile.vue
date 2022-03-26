<template>
  <div class="pile absolute transform top-1/2 -translate-y-1/2 -left-3 bg-darkest rounded-r-lg">
    <div>
      <img :src="cardUrl" alt="card" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { useColorTheme } from '@/composables/colorTheme';
import { CardThemeManager } from '@/core/services/cards/ThemeManager';
import { computed } from '@vue/reactivity';

const colorTheme = useColorTheme();
const cardUrl = computed(() => CardThemeManager.getCardBackUrl('__default__', colorTheme.value));
</script>

<style scoped>
.pile {
  max-height: min(15rem, 35vh);
  max-width: min(15rem, 20vw);
  width: 100%;
  aspect-ratio: 1/2;
}
</style>
