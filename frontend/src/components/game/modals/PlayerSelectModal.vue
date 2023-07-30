<template>
  <BaseModal title="dialogs.selectPlayer.title" info="dialogs.selectPlayer.info" content-class="sm:max-w-xl">
    <div class="flex flex-col items-stretch justify-center gap-2">
      <button
        v-for="option in players"
        :key="option.key"
        @click="close(option.key)"
        class="block bg-main hover:bg-dark rounded-lg px-4 py-2 text-center tc-main"
      >
        {{ option.name }} {{ option.amount ? `(${option.amount})` : '' }}
      </button>
    </div>
  </BaseModal>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { useGameState } from '@/core/adapter/game/gameState';
import { useGameModal } from '@/core/adapter/game/modal';
import { usePlayerManager } from '@/core/adapter/game/playerManager';

import BaseModal from './BaseModal.vue';

const modalState = useGameModal();
const playerManager = usePlayerManager();
const gameState = useGameState();

const players = computed(() =>
  modalState.currentOptions.map((pid, idx) => ({
    key: idx,
    name: playerManager.getPlayer(parseInt(pid)).username,
    amount: gameState.players.find(p => p.id.toString() === pid)?.cards
  }))
);

const close = (option: number) => {
  modalState.closeSelf(option);
};
</script>

<style scoped></style>
