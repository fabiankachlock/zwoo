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
        <div v-if="targetCard" class="target-card">
          <Card :card="targetCard" image-class="h-full"></Card>
        </div>
        <div class="card-to-play flex flex-col flex-nowrap justify-center items-center z-10">
          <div class="relative">
            <div class="absolute" :class="{ 'animation-from-left z-10': isAnimatingFromLeft, 'animate-from-left-card': !isAnimatingFromLeft }">
              <div v-if="nextAfter" class="selected-card relative">
                <Card :card="nextAfter" image-class="h-full"></Card>
              </div>
            </div>
            <div class="absolute" :class="{ 'animation-from-right z-10': isAnimatingFromRight, 'animate-from-right-card': !isAnimatingFromRight }">
              <div v-if="nextBefore" class="selected-card relative">
                <Card :card="nextBefore" image-class="h-full"></Card>
              </div>
            </div>
            <div
              id="detailCard"
              :ref="r => (detailCard = r as HTMLElement)"
              class="selected-card relative cursor-pointer"
              :class="{
                'play-card-animation': isPlayingCard
              }"
              src="/img/dummy_card.svg"
              alt=""
            >
              <Card :card="displayCard" image-class="h-full"></Card>
            </div>
          </div>
          <button
            @click.stop="handlePlayCard()"
            class="bg-lightest hover:bg-light px-4 py-2 my-2 rounded tc-main border-2 border-transparent"
            :class="{
              'border-green-600': canPlayCard === CardState.allowed,
              'border-red-500': canPlayCard === CardState.disallowed
            }"
          >
            #play card#
          </button>
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
import { computed, onMounted, onUnmounted, ref, watch } from 'vue';
import { SWIPE_DIRECTION, useSwipeGesture } from '@/composables/SwipeGesture';
import { CardChecker } from '@/core/services/api/CardCheck';
import { Card as CardTyping } from '@/core/services/game/card';
import Card from './Card.vue';
import { useGameState } from '@/core/adapter/play/gameState';
import { Key, useKeyPress } from '@/composables/KeyPress';

enum CardState {
  allowed,
  disallowed,
  none
}

const ANIMATION_DURATION = 300;

const deckState = useGameCardDeck();
const gameState = useGameState();

const selectedCard = computed(() => deckState.selectedCard);
// eslint-disable-next-line @typescript-eslint/no-non-null-assertion
const displayCard = ref<CardTyping>(selectedCard.value!);
const _targetCardOverride = ref<CardTyping | undefined>(undefined);
const targetCard = computed<CardTyping>({
  get() {
    // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
    return _targetCardOverride.value ?? gameState.mainCard!;
  },
  set(newValue) {
    _targetCardOverride.value = newValue;
  }
});
const nextBefore = ref<(CardTyping & { index: number }) | undefined>(undefined);
const nextAfter = ref<(CardTyping & { index: number }) | undefined>(undefined);
const isAnimatingFromLeft = ref<boolean>(false);
const isAnimatingFromRight = ref<boolean>(false);
const isPlayingCard = ref<boolean>(false);
const detailCard = ref<HTMLElement | undefined>(undefined);
const canPlayCard = ref<CardState>(CardState.none);

onMounted(() => {
  useSwipeGesture(detailCard, () => handlePlayCard(), SWIPE_DIRECTION.up, 200);
  useSwipeGesture(detailCard, () => handleNextBefore(), SWIPE_DIRECTION.left);
  useSwipeGesture(detailCard, () => handleNextAfter(), SWIPE_DIRECTION.right);
});

watch(selectedCard, async card => {
  if (!card) return;
  updateView(card);
});

const updateView = async (card: CardTyping) => {
  if (deckState.hasNext('before')) {
    const [cardBefore, indexBefore] = deckState.getNext('before');
    nextBefore.value = {
      // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
      ...cardBefore!,
      index: indexBefore
    };
  } else {
    nextBefore.value = undefined;
  }

  if (deckState.hasNext('after')) {
    const [cardAfter, indexAfter] = deckState.getNext('after');
    nextAfter.value = {
      // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
      ...cardAfter!,
      index: indexAfter
    };
  } else {
    nextAfter.value = undefined;
  }
  displayCard.value = {
    color: card.color,
    type: card.type
  };
  canPlayCard.value = CardState.none;
  if (selectedCard.value) {
    canPlayCard.value = (await CardChecker.canPlayCard(card)) ? CardState.allowed : CardState.disallowed;
  }
};

const handleKeyPress = (key: string) => {
  if (key === Key.ArrowLeft || key === Key.a) {
    handleNextBefore();
  } else if (key === Key.ArrowRight || key === Key.d) {
    handleNextAfter();
  }
};

const handleNextBefore = () => {
  if (nextBefore.value && !isAnimatingFromRight.value) {
    isAnimatingFromRight.value = true;
    setTimeout(() => {
      // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
      deckState.selectCard(nextBefore.value!, nextBefore.value!.index);
      isAnimatingFromRight.value = false;
    }, ANIMATION_DURATION);
  }
};

const handleNextAfter = () => {
  if (nextAfter.value && !isAnimatingFromLeft.value) {
    isAnimatingFromLeft.value = true;
    setTimeout(() => {
      // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
      deckState.selectCard(nextAfter.value!, nextAfter.value!.index);
      isAnimatingFromLeft.value = false;
    }, ANIMATION_DURATION);
  }
};

const handlePlayCard = () => {
  if (!isPlayingCard.value) {
    isPlayingCard.value = true;
    setTimeout(() => {
      // TODO: just temp - is it?
      targetCard.value = displayCard.value;
      setTimeout(() => {
        deckState.playCard(displayCard.value);
        closeDetail();
      }, ANIMATION_DURATION);
      isPlayingCard.value = false;
    }, ANIMATION_DURATION);
  }
};

const closeDetail = () => {
  deckState.$patch({
    selectedCard: undefined
  });
};

const unregisterKeyListener = useKeyPress([Key.ArrowRight, Key.ArrowLeft, Key.a, Key.d], handleKeyPress);

onUnmounted(() => {
  unregisterKeyListener();
});
</script>

<style>
/* slightly transparent fallback */
.backdrop {
  background-color: rgba(0, 0, 0, 0.7);
}

/* if backdrop support: very transparent and blurred */
@supports ((-webkit-backdrop-filter: blur(8px)) or (backdrop-filter: blur(8px))) {
  .backdrop {
    background-color: transparent;
    backdrop-filter: blur(8px);
  }
}

.card-to-play {
  margin-top: -3vh;
}

.target-card {
  max-height: 30vh;
  height: 30vh;
  max-width: 30vw;
}

.selected-card {
  height: 50vh;
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
