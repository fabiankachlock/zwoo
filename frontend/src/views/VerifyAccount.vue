<template>
  <MaxWidthLayout size="small">
    <h1 class="text-text text-center text-4xl mb-2">{{ t('verifyAccount.title') }}</h1>
    <div v-if="isLoading" class="flex flex-row justify-center flex-nowrap items-center text-text">
      <p class="text-text-secondary text-center my-6 mr-4">
        {{ t(`verifyAccount.verifying`) }}
      </p>
      <Icon icon="iconoir:system-restart" class="text-2xl text-text animate-spin-slow mr-3" />
    </div>
    <div v-else>
      <p class="text-text-secondary text-center my-6">
        {{ t(`verifyAccount.${displayText}`) }}
      </p>
      <button
        v-if="isSuccess"
        class="mx-auto flex justify-center items-center bg-bg border-2 border-transparent px-4 py-1 rounded transition hover:bg-bg cursor-pointer select-none"
        @click="goToLogin()"
      >
        <p class="text-text-secondary text-center">{{ t('verifyAccount.login') }}</p>
      </button>
    </div>
  </MaxWidthLayout>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute, useRouter } from 'vue-router';

import { Icon } from '@/components/misc/Icon';
import { useApi } from '@/core/adapter/helper/useApi';
import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';

const route = useRoute();
const router = useRouter();
const { t } = useI18n();
const { verifyUserAccount } = useApi();

const isLoading = ref(true);
const isSuccess = ref(false);
const displayText = ref('');

onMounted(async () => {
  const id = Array.isArray(route.query['id']) ? route.query['id'][0] : route.query['id'];
  const code = Array.isArray(route.query['code']) ? route.query['code'][0] : route.query['code'];

  const response = await verifyUserAccount(id ?? '', code ?? '');
  isLoading.value = false;
  if (response.wasSuccessful) {
    isSuccess.value = true;
    displayText.value = 'success';
  } else {
    displayText.value = 'error'; // TODO: display general error
  }
});

const goToLogin = () => {
  router.push('/login');
};
</script>
