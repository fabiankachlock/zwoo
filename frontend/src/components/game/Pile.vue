<template>
  <div class="pile absolute top-1/2 -translate-y-1/2 -left-3 bg-darkest rounded-r-lg z-10" :class="{ 'select-none pointer-events-none': !isActive }">
    <div class="relative h-full pile-card-wrapper">
      <div class="pile-card absolute top-1/2 right-2 -translate-y-1/2 h-full transition-all x-delay-0" style="max-height: 95%">
        <Card :card="CardDescriptor.BackSideways" image-class="max-h-full ml-auto mr-0 absolute right-0" image-style="max-width: unset" />
      </div>
      <div class="pile-card absolute top-1/2 right-3 -translate-y-1/2 h-full transition-all x-delay-30" style="max-height: 95%">
        <Card :card="CardDescriptor.BackSideways" image-class="max-h-full ml-auto mr-0 absolute right-0" image-style="max-width: unset" />
      </div>
      <div class="pile-card absolute top-1/2 right-4 -translate-y-1/2 h-full transition-all x-delay-60" style="max-height: 95%">
        <Card :card="CardDescriptor.BackSideways" image-class="max-h-full ml-auto mr-0 absolute right-0" image-style="max-width: unset" />
      </div>
      <div
        class="pile-card draw-card absolute top-1/2 right-4 h-full transition-all -translate-y-1/2 x-delay-60"
        :class="{ animating: isAnimating }"
        style="max-height: 95%"
        @click="drawCard()"
      >
        <Card :card="CardDescriptor.BackSideways" image-class="max-h-full ml-auto mr-0 absolute right-0" image-style="max-width: unset" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';

import { useGameCardDeck } from '@/core/adapter/game/deck';
import { useGameState } from '@/core/adapter/game/gameState';
import { CardDescriptor } from '@/core/domain/cards/CardThemeConfig';

import Card from './Card.vue';

const deckState = useGameCardDeck();
const gameState = useGameState();
const isAnimating = ref(false);
const isActive = computed(() => gameState.isActivePlayer);

const drawCard = () => {
  if (!isAnimating.value) {
    isAnimating.value = true;
    setTimeout(() => {
      deckState.drawCard();
    }, 400);
    setTimeout(() => {
      isAnimating.value = false;
    }, 850);
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

.pile-card {
  width: 100%;
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
  animation: DrawCard 0.8s linear 0s;
}

@keyframes DrawCard {
  0% {
    transform: translateY(-50%) translateX(24%) rotate(0deg) scale(1, 1);
    opacity: 1;
  }
  33% {
    transform: translateY(-50%) translateX(80%) rotate(0deg) scale(1, 1);
    opacity: 1;
  }
  50% {
    transform: translateY(-50%) translateX(180%) rotate(-45deg) scale(1, 1);
    opacity: 1;
  }
  66% {
    transform: translateY(0%) translateX(180%) rotate(-90deg) scale(0.9, 0.9);
    opacity: 1;
  }
  99% {
    transform: translateY(60%) translateX(180%) rotate(-90deg) scale(0.6, 0.6);
    opacity: 0.8;
  }
  100% {
    transform: translateY(70%) translateX(180%) rotate(-90deg) scale(0.4, 0.4);
    opacity: 0;
  }
}
</style>
