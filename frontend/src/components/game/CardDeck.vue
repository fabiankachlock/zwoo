<template>
  <div class="bg-darkest rounded-lg py-1 px-3 h-12 relative">
    <div class="absolute left-0 right-0 bottom-2 z-10 overflow-hidden px-2 flex flex-nowrap justify-center">
      <img
        v-for="card of cards"
        :key="card"
        :style="getComputedCardStyle()"
        :class="{ active: cardsActive, idle: !cardsActive }"
        class="card"
        src="/img/dummy_card.svg"
        alt=""
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted, reactive, ref } from 'vue';
const CARD_ASPECT_RATIO = 476 / 716;

const cardsActive = ref(false);
const dimensions = reactive({
  width: window.innerWidth,
  height: window.innerHeight
});
const cardWidth = ref(0);
const deckWidth = ref(0);

onMounted(() => {
  window.addEventListener('resize', onResize);
  onResize();
});

onUnmounted(() => {
  window.removeEventListener('resize', onResize);
});

const onResize = () => {
  dimensions.height = window.innerHeight;
  dimensions.width = window.innerWidth;
  deckWidth.value = dimensions.width - 60;
  calculateCardWidth();
};

const calculateCardWidth = () => {
  const cardMaxWidth = dimensions.width * 0.25;
  const cardMaxHeight = dimensions.height * 0.3;
  const cardCalculatedHeight = cardMaxWidth / CARD_ASPECT_RATIO;
  const cardCalculatedWidth = cardMaxHeight * CARD_ASPECT_RATIO;

  if (cardCalculatedHeight > cardMaxHeight) {
    cardWidth.value = cardCalculatedWidth;
  } else {
    cardWidth.value = cardMaxWidth;
  }
};

const cards = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
const getComputedCardStyle = (): { [key: string]: string } => {
  return {
    width: Math.round(deckWidth.value / cards.length).toString() + 'px'
  };
};
</script>

<style>
.card {
  max-height: 30vh;
  max-width: 25vw;
  @apply transition-all;
}

.card.active {
  transform: translateY(30%);
}

.card.idle {
  transform: translateY(70%);
  opacity: 0.8;
  filter: grayscale(0.5);
}
</style>
