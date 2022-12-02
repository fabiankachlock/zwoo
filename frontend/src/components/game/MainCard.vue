<template>
  <div class="main-card-wrapper absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 h-full w-full" style="max-height: 70%">
    <div class="absolute left-1/2 top-1/2 -translate-y-1/2 -translate-x-1/2 h-full w-full max-h-full" style="min-width: 0">
      <CardVue :card="mainCard ?? 'back_u'" image-class="max-h-full mx-auto" :forward-ref="r => (elementRef = r as HTMLElement)" />
      <div v-if="newCard">
        <CardVue style="display: none" :card="newCard ?? 'back_u'" image-class="mx-auto" :forward-ref="r => (newCardRef = r as HTMLElement)" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';

import { useAnimate } from '@/composables/useAnimate';
import { useAnimationState } from '@/core/adapter/play/animationState';
import { useGameState } from '@/core/adapter/play/gameState';
import { CardDescriptor } from '@/core/services/cards/CardThemeConfig';
import { Card } from '@/core/services/game/CardTypes';

import CardVue from './Card.vue';

const animate = useAnimate();
const gameState = useGameState();
const animationState = useAnimationState();
const mainCard = ref<Card | CardDescriptor | undefined>();
const newCard = ref<Card | CardDescriptor | undefined>();
const isAnimating = ref<boolean>(false);
const elementRef = ref<HTMLElement | undefined>();
const newCardRef = ref<HTMLElement | undefined>();

const animateChange = async (card: Card | CardDescriptor) => {
  if (!elementRef.value) {
    if (isAnimating.value) {
      newCard.value = card;
    }
    return;
  }

  isAnimating.value = true;
  newCard.value = card;

  setTimeout(async () => {
    if (!elementRef.value || !newCardRef.value) {
      return;
    }
    newCardRef.value.setAttribute('style', '');

    const target = elementRef.value.getBoundingClientRect();
    await animate({
      duration: 300,
      element: newCardRef.value,
      target: elementRef.value,
      transitionProperties: ['left', 'top', 'width', 'height'],
      initialState: {
        top: target.top - target.width,
        left: target.left + target.width * 0.2,
        width: target.width * 0.6,
        height: target.height
      },
      unmount: true,
      remount: true
    });

    mainCard.value = newCard.value;
    newCard.value = undefined;
    newCardRef.value.setAttribute('style', 'display: none');

    isAnimating.value = false;
  }, 1);
};

watch(
  () => gameState.topCard,
  card => {
    animateChange(card);
  },
  { immediate: true }
);

watch(
  elementRef,
  elm => {
    animationState.mainCard = elm;
  },
  { immediate: true }
);
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
