<template>
  <BaseModal title="Color Modal" info="Select a color">
    <div>
      <button :style="`background-color: ${colors[CardColor.green]}`" @click="close(CardColor.green)">Green</button>
      <button :style="`background-color: ${colors[CardColor.red]}`" @click="close(CardColor.red)">Red</button>
      <button :style="`background-color: ${colors[CardColor.blue]}`" @click="close(CardColor.blue)">Blue</button>
      <button :style="`background-color: ${colors[CardColor.yellow]}`" @click="close(CardColor.yellow)">Yellow</button>
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
