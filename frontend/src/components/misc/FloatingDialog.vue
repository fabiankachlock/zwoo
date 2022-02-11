<template>
  <div class="fixed inset-0 z-10" @click="handleClick">
    <div class="relative grid place-items-center w-full h-full">
      <div class="absolute inset-0 backdrop-blur"></div>
      <div class="absolute inset-0 backdrop-color z-10"></div>
      <div class="w-full sm:max-w-3xl mx-auto z-20">
        <div class="bg-lightest shadow-md sm:rounded-xl p-5 m-3 relative" @click="handleDialogClick">
          <slot></slot>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { defineEmits } from 'vue';

const emit = defineEmits<{
  (event: 'clickOutside'): void;
}>();

const handleClick = () => {
  emit('clickOutside');
};

const handleDialogClick = (event: Event) => {
  event.stopPropagation();
};
</script>

<style scoped>
.backdrop-blur {
  backdrop-filter: blur(8px);
}
.backdrop-color {
  background-color: rgba(37, 37, 37, 0.4);
}
</style>
