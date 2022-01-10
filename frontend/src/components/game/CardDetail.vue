<template>
  <div class="fixed z-50 inset-0" v-if="selectedCard" @click="closeDetail()">
    <div class="backdrop absolute inset-0 z-0 transition-all"></div>
    <div class="z-10 relative h-full w-full flex justify-center items-center flex-nowrap">
      <div class="absolute top-10 right-10">
        <button @click.stop="closeDetail" class="bg-lightest hover:bg-light flex flex-nowrap tc-main text-2xl p-3 m-3 md:p-2 md:m-2 rounded">
          <Icon icon="akar-icons:cross" />
        </button>
      </div>
      <div class="w-18 md:w-14 mr-4 md:mr-0">
        <button
          @click.stop="handleNextBefore"
          :class="{ 'opacity-0': !nextBefore }"
          class="bg-lightest hover:bg-light flex flex-nowrap tc-main text-2xl p-3 m-3 md:p-2 md:m-2 rounded"
        >
          <Icon icon="akar-icons:arrow-left" />
        </button>
      </div>
      <div class="flex flex-col justify-center items-center flex-nowrap">
        <div>
          <img class="target-card relative" src="/img/dummy_card.svg" alt="" />
        </div>
        <div class="card-to-play flex flex-col flex-nowrap justify-center items-center">
          <div class="relative">
            <div class="absolute" :class="{ 'animation-from-left z-10': isAnimatingFromLeft }">
              <img class="selected-card relative" src="/img/dummy_card.svg" alt="" />
            </div>
            <div class="absolute" :class="{ 'animation-from-right z-10': isAnimatingFromRight }">
              <img class="selected-card relative" src="/img/dummy_card.svg" alt="" />
            </div>
            <img
              id="detailCard"
              :ref="r => (detailCard = r as HTMLElement)"
              class="selected-card relative cursor-pointer"
              :class="{ 'play-card-animation': isPlayingCard }"
              src="/img/dummy_card.svg"
              alt=""
            />
          </div>
          <button @click.stop="handlePlayCard()" class="bg-lightest hover:bg-light px-4 py-2 my-2 rounded tc-main">#play card#</button>
        </div>
      </div>
      <div class="w-18 md:w-14 ml-4 md:ml-0">
        <button
          :class="{ 'opacity-0': !nextAfter }"
          @click.stop="handleNextAfter"
          class="bg-lightest hover:bg-light flex flex-nowrap tc-main text-2xl p-3 m-3 md:p-2 md:m-2 rounded"
        >
          <Icon icon="akar-icons:arrow-right" />
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { useGameCardDeck } from '@/core/adapter/play/deck';
import { Card } from '@/core/type/game';
import { computed, onMounted, ref, watch } from 'vue';
import { SWIPE_DIRECTION, useSwipeGesture } from '@/composables/SwipeGesture';

const ANIMATION_DURATION = 300;

const deckState = useGameCardDeck();

const selectedCard = computed(() => deckState.selectedCard);
const nextBefore = ref<Card | undefined>(undefined);
const nextAfter = ref<Card | undefined>(undefined);
const isAnimatingFromLeft = ref<boolean>(false);
const isAnimatingFromRight = ref<boolean>(false);
const isPlayingCard = ref<boolean>(false);
const detailCard = ref<HTMLElement | undefined>(undefined);

onMounted(() => {
  useSwipeGesture(detailCard, () => handlePlayCard(), SWIPE_DIRECTION.up, 200);
  useSwipeGesture(detailCard, () => handleNextBefore(), SWIPE_DIRECTION.left);
  useSwipeGesture(detailCard, () => handleNextAfter(), SWIPE_DIRECTION.right);
});

watch(selectedCard, () => {
  console.log('current', selectedCard.value);
  nextBefore.value = deckState.prefetchNext(false);
  console.log('before', nextBefore.value);
  nextAfter.value = deckState.prefetchNext(true);
  console.log('after', nextAfter.value);
});

const handleNextBefore = () => {
  if (nextBefore.value && !isAnimatingFromRight.value) {
    deckState.selectCard(nextBefore.value.id);
    isAnimatingFromRight.value = true;
    setTimeout(() => {
      isAnimatingFromRight.value = false;
    }, ANIMATION_DURATION);
  }
};

const handleNextAfter = () => {
  if (nextAfter.value && !isAnimatingFromLeft.value) {
    deckState.selectCard(nextAfter.value.id);
    isAnimatingFromLeft.value = true;
    setTimeout(() => {
      isAnimatingFromLeft.value = false;
    }, ANIMATION_DURATION);
  }
};

const handlePlayCard = () => {
  if (!isPlayingCard.value) {
    isPlayingCard.value = true;
    setTimeout(() => {
      isPlayingCard.value = false;
    }, ANIMATION_DURATION);
  }
};

const closeDetail = () => {
  deckState.$patch({
    selectedCard: undefined
  });
};
</script>

<style>
.backdrop {
  backdrop-filter: blur(12px);
}

.card-to-play {
  margin-top: -3vh;
}

.target-card {
  max-height: 30vh;
  max-width: 30vw;
}

.selected-card {
  max-height: 50vh;
  max-width: 40vw;
}

.animate-from-left-card {
  transform: scale(0.6, 0.6) translate(-120%, 30%);
  opacity: 0;
}

.animation-from-left {
  animation: slideAndGrowFromLeft 0.3s cubic-bezier(0.075, 0.82, 0.165, 1);
}

@keyframes slideAndGrowFromLeft {
  from {
    transform: scale(0.6, 0.6) translate(-120%, 30%) rotateZ(30deg);
    opacity: 0;
  }
  to {
    transform: scale(1, 1) translate(0, 0) rotateZ(0deg);
    opacity: 1;
  }
}

.animate-from-right-card {
  transform: scale(0.6, 0.6) translate(120%, 30%);
  opacity: 0;
}

.animation-from-right {
  animation: slideAndGrowFromRight 0.3s cubic-bezier(0.075, 0.82, 0.165, 1);
}

@keyframes slideAndGrowFromRight {
  from {
    transform: scale(0.6, 0.6) translate(120%, 30%) rotateZ(-30deg);
    opacity: 0;
  }
  to {
    transform: scale(1, 1) translate(0, 0) rotateZ(0deg);
    opacity: 1;
  }
}

.play-card-animation {
  animation: playCard 0.3s ease-in;
}

@keyframes playCard {
  from {
    transform: scale(1, 1) translate(0, 0);
  }
  to {
    transform: scale(0.5, 0.5) translate(0, -170%);
  }
}
</style>
