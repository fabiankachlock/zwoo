<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute, useRouter } from 'vue-router';

import { Form, FormActions, FormError, FormSubmit, FormTitle, TextInput } from '@/components/forms';
import { Icon } from '@/components/misc/Icon';
import { useRedirect } from '@/composables/useRedirect';
import { AppConfig } from '@/config';
import { useRootApp } from '@/core/adapter/app';
import { useAuth } from '@/core/adapter/auth';
import { useApi } from '@/core/adapter/helper/useApi';
import { BackendErrorType, getBackendErrorTranslation } from '@/core/api/ApiError';
import FormLayout from '@/layouts/FormLayout.vue';

const { t } = useI18n();
const app = useRootApp();
const auth = useAuth();
const route = useRoute();
const router = useRouter();
const { applyRedirectReplace } = useRedirect();

const name = ref('');
const server = ref('');
const error = ref<string[]>([]);
const isLoading = ref<boolean>(false);
const serverHidden = ref<boolean>(false);
const isSubmitEnabled = computed(() => !isLoading.value && !!name.value?.trim() && !!server.value?.trim());
const isLocked = AppConfig.LockEnv;

onMounted(async () => {
  if (auth.isLoggedIn) {
    name.value = auth.username;
  }

  if (isLocked && AppConfig.DefaultEnv === 'local') {
    serverHidden.value = true;
    server.value = '/api/';
    return;
  }

  if (app.environment === 'local') {
    serverHidden.value = true;
    server.value = useApi().getServer();
    return;
  }

  if (route.query['target']) {
    server.value = route.query['target'] as string;
  } else if (AppConfig.DefaultEnv === 'local') {
    server.value = '/api/';
  } else if (AppConfig.IsTauri) {
    const serverModule = await import('@/core/adapter/tauri/localServer');
    const localServer = serverModule.useLocalServer();
    if (localServer.isRunning) {
      server.value = await localServer.getUrl();
    }
  }
});

const logIn = async () => {
  error.value = [];
  isLoading.value = true;

  try {
    const wasSuccess = await auth.loginToLocalServer(name.value, server.value);
    if (!wasSuccess) return;

    if (!applyRedirectReplace()) {
      router.push('/home');
    }
  } catch (e: unknown) {
    error.value = Array.isArray(e) ? e : [getBackendErrorTranslation(e as BackendErrorType)];
  }
  isLoading.value = false;
};
</script>

<template>
  <FormLayout>
    <Form show-back-button>
      <FormTitle>{{ t('loginLocal.title') }}</FormTitle>
      <TextInput v-if="!serverHidden" v-model="server" id="serverUrl" label-key="loginLocal.server" placeholder="http://192.168.0.17/" />
      <div v-else class="m-2">
        <label class="block text-text-secondary text-sm font-bold mb-2">
          {{ t('loginLocal.server') }}
          <slot></slot>
        </label>
        <p class="rounded text-text w-full py-2 px-3">{{ server }}</p>
      </div>
      <TextInput v-model="name" id="username" label-key="loginLocal.name" :placeholder="t('loginLocal.namePlaceholder')" />
      <FormError :error="error" />

      <div class="info border-2 rounded-lg border-primary p-2 my-4 mx-2">
        <Icon icon="akar-icons:info" class="text-primary-text text-xl mb-2" />
        <p class="text-text-secondary">{{ t('loginLocal.infoServer') }}</p>
      </div>

      <FormActions>
        <FormSubmit :disabled="!isSubmitEnabled" @click="logIn" :loading="isLoading">
          {{ t('loginLocal.submit') }}
        </FormSubmit>
      </FormActions>
    </Form>
  </FormLayout>
</template>
