<template>
  <div class="max-w-xl mx-auto m-4">
    <h1 class="tc-main text-center text-4xl mb-2">{{ t('verifyAccount.title') }}</h1>
    <div v-if="isLoading" class="flex flex-row justify-center flex-nowrap items-center tc-main">
      <p class="tc-main-secondary text-center my-6 mr-4">
        {{ t(`verifyAccount.verifying`) }}
      </p>
      <Icon icon="iconoir:system-restart" class="text-2xl tc-main-light animate-spin-slow mr-3" />
    </div>
    <div v-else>
      <p class="tc-main-secondary text-center my-6">
        {{ t(`verifyAccount.${displayText}`) }}
      </p>
      <button
        v-if="isSuccess"
        class="mx-auto flex justify-center items-center bg-lightest border-2 border-transparent px-4 py-1 rounded transition hover:bg-light cursor-pointer select-none"
        @click="goToLogin()"
      >
        <p class="tc-main-secondary text-center">{{ t('verifyAccount.login') }}</p>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute, useRouter } from 'vue-router';

import { AuthenticationService } from '@/core/services/api/Authentication';
import { unwrapBackendError } from '@/core/services/api/Errors';

const route = useRoute();
const router = useRouter();
const { t } = useI18n();

const isLoading = ref(true);
const isSuccess = ref(false);
const displayText = ref('');

onMounted(async () => {
  const id = Array.isArray(route.query['id']) ? route.query['id'][0] : route.query['id'];
  const code = Array.isArray(route.query['code']) ? route.query['code'][0] : route.query['code'];

  const response = await AuthenticationService.verifyAccount(id ?? '', code ?? '');
  const [success] = typeof response === 'object' ? unwrapBackendError(response) : [response, undefined];
  isLoading.value = false;
  if (success) {
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
