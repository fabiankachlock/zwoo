<template>Color Modal <button @click="close">Close</button></template>

<script setup lang="ts">
import { useGameModal } from '@/core/adapter/play/modal';
import { CardColor } from '@/core/services/game/card';
import { ref } from 'vue';

const modalState = useGameModal();
const selectedColor = ref<CardColor | undefined>(undefined);

const close = () => {
  if (selectedColor.value) {
    modalState.closeSelf(selectedColor.value);
  }
};
</script>
