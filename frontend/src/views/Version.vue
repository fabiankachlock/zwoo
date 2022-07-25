<template>
  <div class="max-height-ios p-4">
    <div class="max-w-xl mx-auto m-4">
      <Icon icon="material-symbols:block" class="text-[6em] mx-auto mb-4 text-error-light-border dark:text-error-dark-border"></Icon>
      <h1 class="tc-main text-center text-4xl mb-2">{{ t('wrongVersion.title') }}</h1>
      <p class="tc-main-secondary text-center">{{ t('wrongVersion.info', [config.clientVersion, config.serverVersion]) }}</p>
      <div class="flex flex-wrap justify-center items-center my-2">
        <button
          class="flex justify-center items-center bg-lightest border-2 border-transparent px-4 py-1 rounded transition hover:bg-light cursor-pointer select-none m-1"
          @click="reload"
        >
          <p class="tc-main-secondary text-center">{{ t('wrongVersion.reload') }}</p>
        </button>
        <button
          class="flex justify-center items-center bg-lightest border-2 border-transparent px-4 py-1 rounded transition hover:bg-light cursor-pointer select-none m-1"
          @click="toSettings"
        >
          <p class="tc-main-secondary text-center">{{ t('wrongVersion.toSettings') }}</p>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';
import { Icon } from '@iconify/vue';
import { useConfig } from '@/core/adapter/config';

const { t } = useI18n();
const router = useRouter();
const config = useConfig();

const reload = () => {
  window.location.replace('/');
};

const toSettings = () => {
  router.push('/settings?reason=update');
};
</script>
