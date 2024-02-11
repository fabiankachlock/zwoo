<script setup lang="ts">
import { computed, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';

import { Form, FormActions, FormError, FormSubmit, FormTitle, TextInput } from '@/components/forms';
import { Icon } from '@/components/misc/Icon';
import { useRedirect } from '@/composables/useRedirect';
import { useAuth } from '@/core/adapter/auth';
import { BackendErrorType, getBackendErrorTranslation } from '@/core/api/ApiError';
import FormLayout from '@/layouts/FormLayout.vue';

const { t } = useI18n();
const auth = useAuth();
const router = useRouter();
const { applyRedirectReplace } = useRedirect();

const name = ref('Test');
const server = ref('http://127.0.0.1:8001/api/');
const error = ref<string[]>([]);
const isLoading = ref<boolean>(false);
const isSubmitEnabled = computed(() => !isLoading.value && !!name.value?.trim() && !!server.value?.trim());

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
      <TextInput v-model="server" id="serverUrl" label-key="loginLocal.server" placeholder="http://192.168.0.17/" />
      <TextInput v-model="name" id="username" label-key="loginLocal.name" placeholder="loginLocal.namePlaceholder" />
      <FormError :error="error" />

      <div class="info border-2 rounded-lg bc-primary p-2 my-4 mx-2">
        <Icon icon="akar-icons:info" class="tc-secondary text-xl mb-2" />
        <p class="tc-main-secondary">{{ t('loginLocal.infoServer') }}</p>
      </div>

      <FormActions>
        <FormSubmit :disabled="!isSubmitEnabled" @click="logIn" :loading="isLoading">
          {{ t('loginLocal.submit') }}
        </FormSubmit>
      </FormActions>
    </Form>
  </FormLayout>
</template>
