<template>
  <MaxWidthLayout size="small" class="mt-8">
    <Icon icon="material-symbols:block" class="text-[6em] mx-auto mb-4 w-fit text-error"></Icon>
    <h1 class="text-text text-center text-4xl mb-2">{{ t('wrongVersion.title') }}</h1>
    <p class="text-text-secondary text-center">{{ t('wrongVersion.info', [config.versionInfo.version, serverVersion || 'unknown']) }}</p>
    <div v-if="!isReady" class="flex flex-row justify-center flex-nowrap items-center text-text mt-4">
      <ZwooIcon icon="iconoir:system-restart" class="text-xl text-warning-text animate-spin-slow mr-3" />
      <p class="text-lg text-text-secondary">{{ t('wrongVersion.loading') }}</p>
    </div>
    <div class="flex flex-wrap justify-center items-center my-2">
      <button
        class="flex justify-center items-center bg-alt border border-border px-4 py-1 rounded transition hover:bg-alt-hover cursor-pointer select-none m-1"
        @click="reload"
      >
        <p class="text-center" :class="{ 'text-primary-text': isReady, 'text-text': !isReady }">{{ t('wrongVersion.reload') }}</p>
      </button>
      <button
        class="flex justify-center items-center bg-alt border border-border px-4 py-1 rounded transition hover:bg-alt-hover cursor-pointer select-none m-1"
        @click="toSettings"
      >
        <p class="text-text text-center">{{ t('wrongVersion.toSettings') }}</p>
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
const serverVersion = computed(() => app.serverVersion);

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
