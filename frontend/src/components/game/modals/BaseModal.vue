<template>
  <FloatingDialog>
    <div class="mb-2">
      <h1 class="tc-main text-2xl">
        {{ t(title ?? '') }}
      </h1>
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
import { defineProps } from 'vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

defineProps<{
  title?: string;
  info?: string;
}>();
</script>
