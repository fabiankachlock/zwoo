<template>
  <div class="w-full h-8 overflow-hidden">
    <div class="overflow-x-scroll flex flex-row flex-nowrap items-center h-full">
      <Opponent
        v-for="player in players"
        :key="player.name"
        :name="player.name"
        :card-amount="player.cards"
        :is-active="player.name === activePlayer"
        :is-muted="mutedState[player.name]"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { useChat } from '@/core/adapter/play/chat';
import { useGameState } from '@/core/adapter/play/gameState';
import { computed } from 'vue';
import Opponent from './Opponent.vue';

const game = useGameState();
const chat = useChat();

const players = computed(() => game.players);
const activePlayer = computed(() => game.activePlayer);
const mutedState = computed(() => chat.muted);
</script>
