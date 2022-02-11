<template>
  <div
    class="flex justify-center items-center bg-light border-2 border-transparent px-2 rounded transition hover:bg-main cursor-pointer select-none"
    @click="deleteAccount"
  >
    <p class="text-error-dark-border dark:text-error-light-border text-center">{{ t('settings.deleteAccount') }}</p>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';
const { t } = useI18n();

const deleteAccount = () => {
  console.log('hi');
};
</script>
