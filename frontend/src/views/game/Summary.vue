<template>
  <div class="game-summary overflow-y-scroll">
    <template v-if="players.length > 1">
      <div class="sticky top-0 m-1 bg-dark hover:bg-darkest rounded-lg px-3 py-2 border bc-primary">
        <p class="tc-main text-xl">{{ players[0].name }} - Winner!</p>
      </div>
      <div v-for="player in notWinners" :key="player.name" class="player m-1 bg-lightest hover:bg-light rounded px-3 py-2 border bc-dark">
        <p class="tc-main-dark">{{ player.position }}. {{ player.name }}</p>
      </div>
      <div class="bottom-spacer h-8"></div>
    </template>
    <template v-else> fetching results </template>
  </div>
</template>

<script setup lang="ts">
import { useGameSummary } from '@/core/adapter/play/summary';
import { computed } from '@vue/reactivity';
import { onMounted, ref } from 'vue';

const summary = useGameSummary();
const players = ref<{ name: string; position: number }[]>([]);
const notWinners = computed(() => [...players.value].slice(1) ?? []);

onMounted(async () => {
  players.value = await summary.requestGameSummary();
});
</script>

<style scoped>
.game-summary {
  position: absolute;
  top: 10%;
  left: 3rem;
  right: 3rem;
  bottom: 0;
}

.player {
  @apply mx-3;
}

@media only screen and (min-width: 672px) {
  .player {
    @apply mx-8;
  }
}
</style>
