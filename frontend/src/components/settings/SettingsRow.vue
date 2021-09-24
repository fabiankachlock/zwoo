<template>
  <div class="w-full flex flex-row justify-between items-center bg-dark px-1 py-3 my-4 rounded-lg">
    <div class="h-full flex-1 mx-2 tc-main-secondary">
      {{ t('settings.' + title) }}
    </div>
    <div class="mx-2 h-full flex items-center justify-end">
      <slot></slot>
    </div>
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

defineProps<{
  title: string;
}>();
</script>
