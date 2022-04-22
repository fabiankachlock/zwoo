<template>
  <div class="pile absolute top-1/2 -translate-y-1/2 -left-3 bg-darkest rounded-r-lg z-10">
    <div class="relative h-full pile-card-wrapper">
      <div
        class="pile-card absolute top-1/2 right-2 -translate-y-1/2 h-full transition-all x-delay-0"
        style="max-height: 95%; max-width: fit-content"
      >
        <Card :layers="cardData.layers" :alt="cardData.description" image-class="max-h-full ml-auto mr-0" image-style="max-width: unset" />
      </div>
      <div
        class="pile-card absolute top-1/2 right-3 -translate-y-1/2 h-full transition-all x-delay-30"
        style="max-height: 95%; max-width: fit-content"
      >
        <Card :layers="cardData.layers" :alt="cardData.description" image-class="max-h-full ml-auto mr-0" image-style="max-width: unset" />
      </div>
      <div
        class="pile-card absolute top-1/2 right-4 -translate-y-1/2 h-full transition-all x-delay-60"
        style="max-height: 95%; max-width: fit-content"
      >
        <Card :layers="cardData.layers" :alt="cardData.description" image-class="max-h-full ml-auto mr-0" image-style="max-width: unset" />
      </div>
      <div
        @click="drawCard()"
        class="pile-card draw-card absolute top-1/2 right-4 h-full transition-all -translate-y-1/2 x-delay-60"
        :class="{ animating: isAnimating }"
        style="max-height: 95%"
      >
        <Card :layers="cardData.layers" :alt="cardData.description" image-class="max-h-full ml-auto mr-0" image-style="max-width: unset" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useCardTheme } from '@/core/adapter/play/cardTheme';
import { CardDescriptor } from '@/core/services/cards/CardThemeConfig';
import { computed, ref } from '@vue/reactivity';
import Card from './Card.vue';

const { theme } = useCardTheme();
const cardData = computed(() => theme.value.getCard(CardDescriptor.BackSideways));
const isAnimating = ref(false);

const drawCard = () => {
  if (!isAnimating.value) {
    isAnimating.value = true;
    setTimeout(() => {
      isAnimating.value = false;
    }, 750);
  }
};
</script>

<style scoped>
.pile {
  max-height: min(12rem, 25vh);
  max-width: min(12rem, 14vw);
  height: 100%;
  width: auto;
  aspect-ratio: 1/3;
}

@media only screen and (min-width: 620px) {
  .pile {
    aspect-ratio: 1/2;
  }
}

@media only screen and (min-width: 1080px) and (min-height: 720px) {
  .pile {
    max-height: min(12rem, 35vh);
    aspect-ratio: 1/2;
  }
}

.x-delay-30 {
  transition-delay: 30ms;
}

.x-delay-60 {
  transition-delay: 60ms;
}

@media (hover: hover) {
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
}

.draw-card.animating {
  animation: DrawCard 0.7s linear 0s;
}

@keyframes DrawCard {
  0% {
    transform: translateY(-50%) translateX(24%) rotate(0deg) scale(1, 1);
    opacity: 1;
  }
  33% {
    transform: translateY(-50%) translateX(54%) rotate(0deg) scale(1, 1);
    opacity: 1;
  }
  50% {
    transform: translateY(-50%) translateX(69%) rotate(-45deg) scale(1, 1);
    opacity: 1;
  }
  66% {
    transform: translateY(0%) translateX(69%) rotate(-90deg) scale(0.9, 0.9);
    opacity: 1;
  }
  99% {
    transform: translateY(150%) translateX(69%) rotate(-90deg) scale(0.6, 0.6);
    opacity: 0.8;
  }
  100% {
    transform: translateY(150%) translateX(69%) rotate(-90deg) scale(0.4, 0.4);
    opacity: 0;
  }
}
</style>
