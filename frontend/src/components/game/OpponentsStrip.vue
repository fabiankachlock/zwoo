<template>
  <div class="w-full overflow-hidden">
    <div class="opponents overflow-x-scroll flex flex-row flex-nowrap items-center h-12 select-none">
      <Opponent
        v-for="player in players"
        :key="player.id"
        :id="player.id"
        :name="player.name"
        :card-amount="player.cards"
        :is-active="player.id === activePlayer"
        :is-muted="mutedState[player.name]"
        :is-connected="player.isConnected"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { useChatStore } from '@/core/adapter/play/chat';
import { useGameState } from '@/core/adapter/play/gameState';

import Opponent from './Opponent.vue';

const game = useGameState();
const chat = useChatStore();

const players = computed(() => game.players);
const activePlayer = computed(() => game.activePlayerId);
const mutedState = computed(() => chat.muted);
</script>

<style scoped>
.opponents::-webkit-scrollbar {
  height: 0.4rem !important;
}
</style>
