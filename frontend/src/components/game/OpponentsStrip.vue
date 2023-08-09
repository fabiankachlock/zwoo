<template>
  <div class="w-full overflow-hidden">
    <div class="opponents overflow-x-scroll flex flex-row flex-nowrap items-center h-12 select-none">
      <Opponent
        v-for="player in players"
        :id="player.id"
        :key="player.id"
        :is-self="player.id === self"
        :name="player.name"
        :card-amount="player.cards"
        :is-active="player.id === activePlayer"
        :is-muted="mutedState[player.name]"
        :is-connected="player.isConnected"
        :is-bot="isBot(player.id)"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { useGameConfig } from '@/core/adapter/game';
import { useChatStore } from '@/core/adapter/game/chat';
import { useGameState } from '@/core/adapter/game/gameState';
import { usePlayerManager } from '@/core/adapter/game/playerManager';

import Opponent from './Opponent.vue';

const playerManager = usePlayerManager();
const gameConfig = useGameConfig();
const game = useGameState();
const chat = useChatStore();

const players = computed(() => game.players);
const activePlayer = computed(() => game.activePlayerId);
const mutedState = computed(() => chat.muted);
const self = computed(() => gameConfig.lobbyId);

const isBot = (id: number) => {
  return playerManager.bots.some(b => b.id == id);
};
</script>

<style scoped>
.opponents::-webkit-scrollbar {
  height: 0.4rem !important;
}
</style>
