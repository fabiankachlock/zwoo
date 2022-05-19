<template>
  <BaseModal title="Color Modal" info="Select a color">
    <div>
      <button @click="close(CardColor.green)">Green</button>
      <button @click="close(CardColor.red)">Red</button>
      <button @click="close(CardColor.blue)">Blue</button>
      <button @click="close(CardColor.yellow)">Yellow</button>
    </div>
  </BaseModal>
</template>

<script setup lang="ts">
import { useGameModal } from '@/core/adapter/play/modal';
import { CardColor } from '@/core/services/game/card';
import BaseModal from './BaseModal.vue';

const modalState = useGameModal();

const close = (color: CardColor) => {
  modalState.closeSelf(color);
};
</script>
