<template>
  <MaxWidthLayout size="small" classes="m-4">
    <h1 class="text-text text-center text-4xl mb-2">{{ t('cookies.disallowed.title') }}</h1>
    <p class="text-text-secondary text-center">{{ t('cookies.disallowed.info') }}</p>
    <div class="flex justify-center items-center my-2">
      <button
        class="flex justify-center items-center bg-bg-surface border-2 border-transparent px-4 py-1 rounded transition hover:bg-darkest cursor-pointer select-none"
        @click="manageSelection"
      >
        <p class="text-text-secondary text-center">{{ t('cookies.manage') }}</p>
      </button>
    </div>
  </MaxWidthLayout>
</template>

<script setup lang="ts">
import { watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';

import { useRedirect } from '@/composables/useRedirect';
import { useCookies } from '@/core/adapter/cookies';
import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';

const { t } = useI18n();
const cookies = useCookies();
const router = useRouter();
const { applyRedirectReplace } = useRedirect();

watch(
  () => cookies.popupOpen,
  open => {
    if (!open) {
      if (!applyRedirectReplace()) {
        router.push('/home');
        return;
      }
    }
  }
);
const manageSelection = () => {
  cookies.popupOpen = true;
};
</script>
