<template>
  <BaseModal title="dialogs.selectColor.title" info="dialogs.selectColor.info">
    <div class="color-select-modal-grid">
      <button
        v-for="(option, idx) in colors"
        :key="option.key"
        :class="classes[idx]"
        :style="`background-color: ${option.color}`"
        aria-label="green"
        @click="close(option.key)"
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

const classes = [
  'rounded-tl-full hover:scale-110 hover:-translate-x-3 hover:-translate-y-3 transition-transform',
  'rounded-tr-full hover:scale-110 hover:translate-x-3 hover:-translate-y-3 transition-transform',
  'rounded-bl-full hover:scale-110 hover:-translate-x-3 hover:translate-y-3 transition-transform',
  'rounded-br-full hover:scale-110 hover:translate-x-3 hover:translate-y-3 transition-transform'
];
const colors = computed(() =>
  modalState.currentOptions.map((key, idx) => ({
    key: idx,
    color: theme.theme.colors[key as unknown as CardColor]
  }))
);

const close = (item: number) => {
  modalState.closeSelf(item);
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
