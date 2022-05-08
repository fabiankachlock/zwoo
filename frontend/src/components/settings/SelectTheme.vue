<template>
  <button
    class="flex justify-center items-center bg-light border-2 border-transparent px-2 rounded transition hover:bg-main cursor-pointer select-none"
    @click="selectTheme"
  >
    <p class="tc-main-light text-center">{{ t('theme.select') }}</p>
  </button>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
const router = useRouter();

const selectTheme = () => {
  router.push('/themes/gallery');
};
</script>
