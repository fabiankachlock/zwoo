<template>
  <div class="max-w-xl mx-auto m-4">
    <h1 class="tc-main text-center text-4xl mb-2">{{ t('cookies.disallowed.title') }}</h1>
    <p class="tc-main-secondary text-center">{{ t('cookies.disallowed.info') }}</p>
    <div class="flex justify-center items-center my-2">
      <button
        class="flex justify-center items-center bg-lightest border-2 border-transparent px-4 py-1 rounded transition hover:bg-light cursor-pointer select-none"
        @click="manageSelection"
      >
        <p class="tc-main-secondary text-center">{{ t('cookies.manage') }}</p>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useCookies } from '@/core/adapter/cookies';
import { watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute, useRouter } from 'vue-router';

const { t } = useI18n();
const cookies = useCookies();
const router = useRouter();
const route = useRoute();

watch(
  () => cookies.popupOpen,
  open => {
    if (!open) {
      const redirect = route.query['redirect'] as string;

      if (redirect) {
        router.replace(redirect);
        return;
      }
      router.push('/home');
    }
  }
);
const manageSelection = () => {
  cookies.popupOpen = true;
};
</script>
