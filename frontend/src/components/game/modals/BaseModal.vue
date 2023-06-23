<template>
  <!-- TODO tmp(beta): @click-outside="closeModal" -->
  <FloatingDialog :content-class="contentClass">
    <div class="mb-2 flex justify-between items-center">
      <h1 class="tc-main text-2xl">
        {{ t(title ?? '') }}
      </h1>
      <!-- TODO tmp(beta): breaks game currently <button @click="closeModal">
        <Icon class="text-2xl tc-main transition-transform hover:scale-110" icon="akar-icons:cross" />
      </button> -->
    </div>
    <div class="mb-4">
      <p class="tc-main-secondary text-lg">
        {{ t(info ?? '') }}
      </p>
    </div>
    <div class="relative mb-2">
      <slot></slot>
    </div>
  </FloatingDialog>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';

import FloatingDialog from '../../misc/FloatingDialog.vue';

const { t } = useI18n();
// const modalState = useGameModal();

defineProps<{
  title?: string;
  info?: string;
  contentClass?: string;
}>();

// const closeModal = () => {
//   modalState.closeSelf(undefined);
// };
</script>
