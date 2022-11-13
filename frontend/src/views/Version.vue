<template>
  <MaxWidthLayout size="small">
    <Icon icon="material-symbols:block" class="text-[6em] mx-auto mb-4 text-error-light-border dark:text-error-dark-border"></Icon>
    <h1 class="tc-main text-center text-4xl mb-2">{{ t('wrongVersion.title') }}</h1>
    <p class="tc-main-secondary text-center">{{ t('wrongVersion.info', [config.clientVersion, config.serverVersion]) }}</p>
    <div v-if="!isReady" class="flex flex-row justify-center flex-nowrap items-center tc-main mt-4">
      <ZwooIcon icon="iconoir:system-restart" class="text-xl tc-main-light animate-spin-slow mr-3" />
      <p class="text-lg tc-main-secondary">{{ t('wrongVersion.loading') }}</p>
    </div>
    <div class="flex flex-wrap justify-center items-center my-2">
      <button
        class="flex justify-center items-center bg-lightest border-2 border-transparent px-4 py-1 rounded transition hover:bg-light cursor-pointer select-none m-1"
        @click="reload"
      >
        <p class="text-center" :class="{ 'tc-primary': isReady, 'tc-main-secondary': !isReady }">{{ t('wrongVersion.reload') }}</p>
      </button>
      <button
        class="flex justify-center items-center bg-lightest border-2 border-transparent px-4 py-1 rounded transition hover:bg-light cursor-pointer select-none m-1"
        @click="toSettings"
      >
        <p class="tc-main-secondary text-center">{{ t('wrongVersion.toSettings') }}</p>
      </button>
    </div>
  </MaxWidthLayout>
</template>

<script setup lang="ts">
import { computed, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';

import { Icon } from '@/components/misc/Icon';
import { useRootApp } from '@/core/adapter/app';
import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';
import ZwooIcon from '@/modules/zwoo-icons/ZwooIcon.vue';

const { t } = useI18n();
const router = useRouter();
const app = useRootApp();
const config = useRootApp();
const isReady = computed(() => app.updateAvailable);

watch(
  () => app.updateAvailable,
  updateAvailable => {
    if (updateAvailable) {
      app.updateApp();
    }
  },
  { immediate: true }
);

const reload = () => {
  if (app.updateAvailable) {
    app.updateApp();
  }
};

const toSettings = () => {
  router.push('/settings?reason=update');
};
</script>
