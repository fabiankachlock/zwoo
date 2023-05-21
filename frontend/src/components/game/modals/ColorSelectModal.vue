<template>
  <BaseModal title="dialogs.selectColor.title" info="dialogs.selectColor.info">
    <div class="color-select-modal-grid">
      <button
        class="rounded-tl-full hover:scale-110 hover:-translate-x-3 hover:-translate-y-3 transition-transform"
        :style="`background-color: ${colors[CardColor.green]}`"
        aria-label="green"
        @click="close(CardColor.green)"
      ></button>
      <button
        class="rounded-tr-full hover:scale-110 hover:translate-x-3 hover:-translate-y-3 transition-transform"
        :style="`background-color: ${colors[CardColor.red]}`"
        aria-label="red"
        @click="close(CardColor.red)"
      ></button>
      <button
        class="rounded-bl-full hover:scale-110 hover:-translate-x-3 hover:translate-y-3 transition-transform"
        :style="`background-color: ${colors[CardColor.blue]}`"
        aria-label="blue"
        @click="close(CardColor.blue)"
      ></button>
      <button
        class="rounded-br-full hover:scale-110 hover:translate-x-3 hover:translate-y-3 transition-transform"
        :style="`background-color: ${colors[CardColor.yellow]}`"
        aria-label="yellow"
        @click="close(CardColor.yellow)"
      ></button>
    </div>
  </BaseModal>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { useCardTheme } from '@/core/adapter/game/cardTheme';
import { useGameModal } from '@/core/adapter/game/modal';
import { CardColor } from '@/core/domain/game/CardTypes';

import BaseModal from './BaseModal.vue';

const modalState = useGameModal();
const theme = useCardTheme();

const colors = computed(() => theme.theme.colors);

const close = (color: CardColor) => {
  modalState.closeSelf(color);
};
</script>

<style scoped>
.color-select-modal-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  grid-template-rows: 1fr 1fr;
  aspect-ratio: 1/1;
  max-width: 20rem;
  width: 100%;
  margin: 0 auto;
}
</style>
