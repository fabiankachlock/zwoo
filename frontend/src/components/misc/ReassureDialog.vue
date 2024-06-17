<template>
  <div v-if="isOpen">
    <FloatingDialog content-class="sm:max-w-lg">
      <div v-if="showCloseButton" class="absolute top-4 right-4 z-10">
        <button class="bg-bg hover:bg-bg p-2 text-text-dark rounded" @click="close(false)">
          <Icon icon="akar-icons:cross" class="text-2xl" />
        </button>
      </div>
      <div class="relative">
        <h2 class="text-text text-2xl mb-4">{{ t(title) }}</h2>
        <p class="mb-2 text-text-secondary">{{ t(body) }}</p>
        <div class="grid grid-cols-2 gap-2 mt-4">
          <button
            class="flex justify-center items-center bg-bg border-2 border-secondary px-4 py-1 rounded transition hover:bg-surface cursor-pointer select-none"
            @click="close(false)"
          >
            <p class="text-secondary-text text-center">{{ t(decline ?? 'reassureDialog.decline') }}</p>
          </button>
          <button
            class="flex justify-center items-center bg-bg border-2 border-primary px-4 py-1 rounded transition hover:bg-surface cursor-pointer select-none"
            @click="close(true)"
          >
            <p class="text-primary-text text-center">{{ t(accept ?? 'reassureDialog.accept') }}</p>
          </button>
        </div>
      </div>
    </FloatingDialog>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';

import { Icon } from '@/components/misc/Icon';

import FloatingDialog from './FloatingDialog.vue';

const { t } = useI18n();

defineProps<{
  showCloseButton?: boolean;
  isOpen: boolean;
  title: string;
  body: string;
  accept?: string;
  decline?: string;
}>();

const emit = defineEmits<{
  (event: 'close', accepted: boolean): void;
}>();

const close = (accept: boolean) => {
  emit('close', accept);
};
</script>
