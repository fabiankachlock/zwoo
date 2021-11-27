<template>
  <div class="fixed z-50 inset-0" v-if="selectedCard">
    <div class="backdrop absolute inset-0 z-0"></div>
    <div class="z-10 relative h-full w-full flex justify-center items-center flex-nowrap">
      <div class="w-14">
        <button v-if="nextBefore" @click="handleNextBefore" class="bg-lightest flex flex-nowrap tc-main text-2xl p-2 m-2 rounded">
          <Icon icon="akar-icons:arrow-left" />
        </button>
      </div>
      <div class="flex flex-col justify-center items-center flex-nowrap">
        <div>
          <img class="target-card relative" src="/img/dummy_card.svg" alt="" />
        </div>
        <div class="card-to-play flex flex-col flex-nowrap justify-center items-center">
          <div>
            <img class="selected-card relative" src="/img/dummy_card.svg" alt="" />
          </div>
          <button class="bg-lightest px-4 py-2 my-2 rounded tc-main">#play card#</button>
        </div>
      </div>
      <div class="w-14">
        <button v-if="nextAfter" @click="handleNextAfter" class="bg-lightest flex flex-nowrap tc-main text-2xl p-2 m-2 rounded">
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
import { computed, ref, watch } from 'vue';

const deckState = useGameCardDeck();

const selectedCard = computed(() => deckState.selectedCard);
const nextBefore = ref<Card | undefined>(undefined);
const nextAfter = ref<Card | undefined>(undefined);

watch(selectedCard, () => {
  nextBefore.value = deckState.prefetchNext(false);
  nextAfter.value = deckState.prefetchNext(true);
});

const handleNextBefore = () => {
  if (nextBefore.value) {
    deckState.selectCard(nextBefore.value.id);
  }
};

const handleNextAfter = () => {
  if (nextAfter.value) {
    deckState.selectCard(nextAfter.value.id);
  }
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
</style>

