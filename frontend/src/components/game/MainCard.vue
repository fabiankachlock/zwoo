<template>
  <div class="main-card-wrapper absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 h-full w-full" style="max-height: 70%">
    <!-- <div class="absolute left-0 right-0 top-1/2 -translate-y-1/2 h-full"> -->
    <Card :layers="cardData.layers" alt="main card" />
    <!-- </div> -->
  </div>
</template>

<script setup lang="ts">
import { useCardTheme } from '@/core/adapter/play/cardTheme';
import { computed } from 'vue';
import Card from './Card.vue';

const cardTheme = useCardTheme();
const cardData = computed(() => cardTheme.theme.getCard({ type: 1, color: 1, id: '' }));
</script>

<style scoped>
.main-card {
  width: auto;
  height: auto;
  max-width: 100%;
  max-height: 100%;
}

.main-card-wrapper {
  max-width: 40vw;
}

@media only screen and (min-width: 440px) {
  .main-card-wrapper {
    max-width: 30vw;
  }
}

@media only screen and (min-width: 620px) {
  .main-card-wrapper {
    max-width: 20vw;
  }
}
</style>
