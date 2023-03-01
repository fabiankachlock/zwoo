<template>
  <MaxWidthLayout size="small" classes="m-4">
    <h1 class="tc-main text-center text-4xl mb-2">{{ t('offline.title') }}</h1>
    <p class="tc-main-secondary text-center">{{ t('offline.info') }}</p>
    <router-link to="/create-game">HERE</router-link>
  </MaxWidthLayout>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';

import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';

const { t } = useI18n();
</script>
