<template>
  <div class="pile absolute transform top-1/2 -translate-y-1/2 -left-3 bg-darkest rounded-r-lg">
    <div class="relative h-full pile-card-wrapper">
      <div class="pile-card absolute top-1/2 right-2 transform -translate-y-1/2 h-full transition-all x-delay-0" style="max-height: 95%">
        <img :src="cardUrl" alt="card" class="transform rotate-0 max-h-full" style="max-width: unset" />
      </div>
      <div class="pile-card absolute top-1/2 right-3 transform -translate-y-1/2 h-full transition-all x-delay-30" style="max-height: 95%">
        <img :src="cardUrl" alt="card" class="transform rotate-0 max-h-full" style="max-width: unset; --ty-scale-x: 0.97; --tw-scale-y: 0.97" />
      </div>
      <div class="pile-card absolute top-1/2 right-4 transform -translate-y-1/2 h-full transition-all x-delay-60" style="max-height: 95%">
        <img :src="cardUrl" alt="card" class="transform rotate-0 max-h-full" style="max-width: unset; --ty-scale-x: 0.94; --tw-scale-y: 0.94" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useColorTheme } from '@/composables/colorTheme';
import { CardThemeManager } from '@/core/services/cards/ThemeManager';
import { computed } from '@vue/reactivity';

const colorTheme = useColorTheme();
const cardUrl = computed(() => CardThemeManager.getCardBackUrl('__default__', colorTheme.value, true));
</script>

<style scoped>
.pile {
  max-height: min(15rem, 35vh);
  max-width: min(15rem, 20vw);
  width: 100%;
  aspect-ratio: 1/2;
}

.x-delay-30 {
  transition-delay: 30ms;
}

.x-delay-60 {
  transition-delay: 60ms;
}

.pile-card-wrapper:hover .pile-card {
  @apply scale-100;
}
.pile-card-wrapper:hover .pile-card.x-delay-0 {
  --tw-translate-x: 14%;
}
.pile-card-wrapper .pile-card.x-delay-30 {
  --tw-translate-x: -2%;
}
.pile-card-wrapper:hover .pile-card.x-delay-30 {
  --tw-translate-x: 19%;
}

.pile-card-wrapper .pile-card.x-delay-60 {
  --tw-translate-x: -4%;
}
.pile-card-wrapper:hover .pile-card.x-delay-60 {
  --tw-translate-x: 24%;
}
</style>
