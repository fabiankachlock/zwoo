<script setup lang="ts">
import { useI18n } from 'vue-i18n';

import FloatingDialog from '../FloatingDialog.vue';

defineProps<{
  themeName: string;
  themeZrpVersion: string;
  currentZrpVersion: string;
}>();

const emit = defineEmits<{
  (event: 'close'): void;
}>();

const { t } = useI18n();

const close = () => {
  emit('close');
};
</script>

<template>
  <FloatingDialog>
    <h2 class="text-text text-xl text-center mt-2">
      {{ t('cardThemes.themeIncompatible') }}
    </h2>

    <div class="py-1">
      <p class="text-text">
        {{ t('cardThemes.incompatibleInfo') }}
      </p>
    </div>

    <div class="sticky -bottom-[0.5px] bg-bg px-5 pb-3">
      <div class="flex justify-end items-center border-border border-t">
        <button
          class="mt-4 flex justify-center items-center bg-alt border border-border px-4 py-1 rounded transition hover:bg-alt-hover cursor-pointer select-none"
          @click="close"
        >
          <p class="text-text text-center">{{ t('changelog.close') }}</p>
        </button>
      </div>
    </div>
  </FloatingDialog>
</template>
