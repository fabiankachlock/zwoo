<template>
  <BaseModal title="dialogs.selectColor.title" info="dialogs.selectColor.info">
    <div class="color-select-modal-grid">
      <button
        class="rounded-tl-full hover:scale-110 hover:-translate-x-3 hover:-translate-y-3 transition-transform"
        :style="`background-color: ${colors[CardColor.green]}`"
        @click="close(CardColor.green)"
        aria-label="green"
      ></button>
      <button
        class="rounded-tr-full hover:scale-110 hover:translate-x-3 hover:-translate-y-3 transition-transform"
        :style="`background-color: ${colors[CardColor.red]}`"
        @click="close(CardColor.red)"
        aria-label="red"
      ></button>
      <button
        class="rounded-bl-full hover:scale-110 hover:-translate-x-3 hover:translate-y-3 transition-transform"
        :style="`background-color: ${colors[CardColor.blue]}`"
        @click="close(CardColor.blue)"
        aria-label="blue"
      ></button>
      <button
        class="rounded-br-full hover:scale-110 hover:translate-x-3 hover:translate-y-3 transition-transform"
        :style="`background-color: ${colors[CardColor.yellow]}`"
        @click="close(CardColor.yellow)"
        aria-label="yellow"
      ></button>
    </div>
  </BaseModal>
</template>

<script setup lang="ts">
import { useCardTheme } from '@/core/adapter/play/cardTheme';
import { useGameModal } from '@/core/adapter/play/modal';
import { CardColor } from '@/core/services/game/card';
import { computed } from 'vue';
import BaseModal from './BaseModal.vue';

const modalState = useGameModal();
const theme = useCardTheme();

const colors = computed(() => theme.theme.colors);

const close = (color: CardColor) => {
  modalState.closeSelf(color);
};
</script>

<style>
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
