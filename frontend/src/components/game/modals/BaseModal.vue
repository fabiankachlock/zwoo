<template>
  <FloatingDialog @click-outside="closeModal">
    <div class="mb-2 flex justify-between items-center">
      <h1 class="tc-main text-2xl">
        {{ t(title ?? '') }}
      </h1>
      <button @click="closeModal">
        <Icon class="text-2xl transition-transform hover:scale-110" icon="akar-icons:cross" />
      </button>
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
import FloatingDialog from '../../misc/FloatingDialog.vue';
import { Icon } from '@iconify/vue';
import { defineProps } from 'vue';
import { useI18n } from 'vue-i18n';
import { useGameModal } from '@/core/adapter/play/modal';

const { t } = useI18n();
const modalState = useGameModal();

defineProps<{
  title?: string;
  info?: string;
}>();

const closeModal = () => {
  modalState.closeSelf(undefined);
};
</script>
