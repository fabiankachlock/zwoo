<template>
  <div class="max-w-xl mx-auto m-4">
    <h1 class="tc-main text-center text-4xl mb-2">{{ t('verifyAccount.verifying') }}</h1>
    <div v-if="isLoading || true" class="flex flex-row justify-center flex-nowrap items-center tc-main">
      <Icon icon="iconoir:system-restart" class="text-3xl tc-main-light animate-spin-slow mr-3" />
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
import { AuthenticationService } from '@/core/services/api/Authentication';
import { unwrapBackendError } from '@/core/services/api/errors';
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute, useRouter } from 'vue-router';
import { Icon } from '@iconify/vue';

const route = useRoute();
const router = useRouter();
const { t } = useI18n();

const isLoading = ref(false);
const isSuccess = ref(false);
const displayText = ref('');

onMounted(async () => {
  const id = Array.isArray(route.params['id']) ? route.params['id'][0] : route.params['id'];
  const code = Array.isArray(route.params['code']) ? route.params['code'][0] : route.params['code'];

  const response = await AuthenticationService.verifyAccount(id, code);
  const [success, error] = typeof response === 'object' ? unwrapBackendError(response) : [response, undefined];
  isLoading.value = false;
  if (success) {
    isSuccess.value = true;
    displayText.value = 'success';
  } else {
    displayText.value = error?.message ?? 'error'; // TODO: display general error
  }
});

const goToLogin = () => {
  router.push('/login');
};
</script>
