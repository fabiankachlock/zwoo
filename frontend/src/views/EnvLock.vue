<script setup lang="ts">
import { useI18n } from 'vue-i18n';

import { Icon } from '@/components/misc/Icon';
import { useRootApp } from '@/core/adapter/app';
import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';

const { t } = useI18n();

const app = useRootApp();
const target = new URLSearchParams(location.search).get('target');

const reload = () => {
  location.reload();
};
</script>

<template>
  <MaxWidthLayout size="small" class="mt-8">
    <Icon icon="material-symbols:block" class="text-[6em] mx-auto mb-4 w-fit text-error-light-border dark:text-error-dark-border"></Icon>
    <h1 class="tc-main text-center text-4xl mb-2">{{ t('envLock.title') }}</h1>
    <p class="tc-main-secondary text-center">{{ t('envLock.info', [app.environment, target]) }}</p>

    <div class="flex flex-wrap justify-center items-center my-2">
      <button
        class="flex justify-center items-center bg-lightest border-2 border-transparent px-4 py-1 rounded transition hover:bg-light cursor-pointer select-none m-1"
        @click="reload"
      >
        <p class="text-center tc-main-secondary">{{ t('envLock.reload') }}</p>
      </button>
    </div>
  </MaxWidthLayout>
</template>
