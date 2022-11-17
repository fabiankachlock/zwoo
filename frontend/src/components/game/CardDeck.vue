<template>
  <div class="deck bg-darkest rounded-lg py-1 px-3 relative">
    <div
      :class="{ 'pointer-events-none': !cardsActive }"
      :style="`height: ${cardWidth * CARD_ASPECT_RATIO}`"
      class="absolute left-0 right-0 bottom-2 z-10 overflow-hidden px-2 flex flex-nowrap justify-center"
    >
      <div
        v-for="(card, index) of cards"
        :key="`${index}-${card.color}-${card.type}`"
        :data-x="`${index}-${card.color}-${card.type}`"
        @click="selectCard(card, index)"
        :style="getComputedCardWrapperStyle"
        :class="{ active: cardsActive, idle: !cardsActive, overlap: isCardOverlap }"
        class="card-wrapper relative overflow-visible"
      >
        <div class="card relative" :style="getComputedCardStyle">
          <Card :card="card" image-class="transition-all" />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted, reactive, ref, watch } from 'vue';

import { useGameCardDeck } from '@/core/adapter/play/deck';
import { useGameState } from '@/core/adapter/play/gameState';
import { Card as CardTyping } from '@/core/services/game/CardTypes';

import Card from './Card.vue';
const CARD_ASPECT_RATIO = 420 / 720;
const CARD_BASE_WIDTH_MULTIPLIER = 0.25;
const CARD_BASE_HEIGHT_MULTIPLIER = 0.3;

const deckStore = useGameCardDeck();
const stateStore = useGameState();

const cardsActive = computed(() => stateStore.isActivePlayer);
const cards = computed(() => deckStore.cards);
watch(cards, () => {
  resizeCards();
});

const isCardOverlap = ref(false);
const cardWidth = ref(0);
const deckWidth = ref(0);
const dimensions = reactive({
  width: window.innerWidth,
  height: window.innerHeight
});

// register resize events
onMounted(() => {
  window.addEventListener('resize', onResize);
  onResize();
});

onUnmounted(() => {
  window.removeEventListener('resize', onResize);
});

// handle resize
const onResize = () => {
  dimensions.height = window.innerHeight;
  dimensions.width = window.innerWidth;
  deckWidth.value = dimensions.width - 60;
  resizeCards();
};

const deckFilledFactor = computed(() => {
  return (cards.value.length * dimensions.width * CARD_BASE_WIDTH_MULTIPLIER) / deckWidth.value;
});

const resizeCards = () => {
  const dimensionsMultiplier = adjustCardSize(deckFilledFactor.value);

  const cardMaxWidth = dimensions.width * dimensionsMultiplier.width;
  const cardMaxHeight = dimensions.height * dimensionsMultiplier.height;
  const cardCalculatedHeight = cardMaxWidth / CARD_ASPECT_RATIO;
  const cardCalculatedWidth = cardMaxHeight * CARD_ASPECT_RATIO;

  if (cardCalculatedHeight > cardMaxHeight) {
    cardWidth.value = cardCalculatedWidth;
  } else {
    cardWidth.value = cardMaxWidth;
  }

  isCardOverlap.value = cardWidth.value * cards.value.length > deckWidth.value * 0.9; // 10% threshold
};

const adjustCardSize = (filledFactor: number): { width: number; height: number } => {
  if (filledFactor < 3) {
    return { width: CARD_BASE_WIDTH_MULTIPLIER, height: CARD_BASE_HEIGHT_MULTIPLIER };
  } else if (filledFactor < 7) {
    return { width: CARD_BASE_WIDTH_MULTIPLIER * 0.8, height: CARD_BASE_HEIGHT_MULTIPLIER * 0.8 };
  } else if (filledFactor < 11) {
    return { width: CARD_BASE_WIDTH_MULTIPLIER * 0.5, height: CARD_BASE_HEIGHT_MULTIPLIER * 0.5 };
  }
  return { width: CARD_BASE_WIDTH_MULTIPLIER * 0.3, height: CARD_BASE_HEIGHT_MULTIPLIER * 0.3 };
};

// compute styles
const getComputedCardWrapperStyle = computed((): { [key: string]: string } => {
  if (isCardOverlap.value) {
    return {
      width: Math.round(deckWidth.value / (cards.value.length + Math.log2(cards.value.length))).toString() + 'px'
    };
  }

  return {
    width: Math.round(cardWidth.value).toString() + 'px'
  };
});

const getComputedCardStyle = computed((): { [key: string]: string } => {
  return {
    width: Math.round(cardWidth.value) + 'px',
    transform: `translateX(${(-28 - Math.log2(cards.value.length) * 2).toString()}%`
  };
});

const selectCard = (card: CardTyping, index: number) => {
  deckStore.selectCard(card, index);
};
</script>

<style scoped>
.deck {
  height: calc(0.5rem + 6vh);
}

.card-wrapper .card div {
  @apply transition-transform;
}

.card-wrapper {
  @apply transition-all relative;
}

.card-wrapper.active {
  transform: translateY(30%);
}

.card-wrapper.active .card:hover div {
  transform: scale(1.3, 1.3);
}

.card-wrapper.idle {
  transform: translateY(70%);
  filter: grayscale(0.2);
}

.card {
  max-width: unset;
}

.card-wrapper:not(.overlap) .card {
  transform: unset !important;
}
</style>
