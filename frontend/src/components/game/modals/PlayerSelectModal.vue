<template>
  <BaseModal title="dialogs.selectPlayer.title" info="dialogs.selectPlayer.info">
    <div>
      <button v-for="option in players" :key="option.key" @click="close(option.key)">
        {{ option.name }}
      </button>
    </div>
  </BaseModal>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { useGameModal } from '@/core/adapter/game/modal';
import { usePlayerManager } from '@/core/adapter/game/playerManager';

import BaseModal from './BaseModal.vue';

const modalState = useGameModal();
const playerManager = usePlayerManager();

const players = computed(() =>
  modalState.currentOptions.map((pid, idx) => ({
    key: idx,
    name: playerManager.getPlayer(pid).username
  }))
);

const close = (option: number) => {
  modalState.closeSelf(option);
};
</script>

<style scoped></style>
