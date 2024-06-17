<template>
  <FormLayout>
    <Form show-back-button>
      <FormTitle> {{ t('login.title') }} </FormTitle>
      <TextInput id="email" v-model="email" label-key="login.email" :placeholder="t('login.email')" />
      <TextInput id="password" v-model="password" label-key="login.password" is-password placeholder="******" />
      <CaptchaButton :validator="reCaptchaValidator" :token="captchaResponse" @update:response="res => (captchaResponse = res)" />
      <FormError :error="error" />
      <FormActions>
        <FormSubmit :disabled="!isSubmitEnabled" @click="logIn">
          {{ t('login.login') }}
        </FormSubmit>
        <div v-if="showNotVerifiedInfo" class="info border-2 rounded-lg border-primary p-2 my-4 mx-2">
          <Icon icon="akar-icons:info" class="text-primary-text text-xl mb-2" />
          <p class="text-text-secondary">{{ t('login.notVerifiedInfo') }}</p>
          <button class="text-primary-text mt-2 bg-bg hover:bg-bg-surface rounded-sm px-2 py-1 text-center" @click="resendVerifyEmail">
            {{ t('login.resendVerifyEmail') }}
          </button>
        </div>
        <FormSecondaryAction>
          <router-link class="w-full block text-center" :to="'/request-password-reset?' + joinQuery(route.query)">{{
            t('login.resetPassword')
          }}</router-link>
        </FormSecondaryAction>
        <FormAlternativeAction>
          <router-link class="w-full block text-center" :to="'/create-account?' + joinQuery(route.query)">{{ t('nav.createAccount') }}</router-link>
        </FormAlternativeAction>
      </FormActions>
    </Form>
  </FormLayout>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute, useRouter } from 'vue-router';

import CaptchaButton from '@/components/forms/CaptchaButton.vue';
import { Form, FormActions, FormAlternativeAction, FormError, FormSecondaryAction, FormSubmit, FormTitle, TextInput } from '@/components/forms/index';
import { Icon } from '@/components/misc/Icon';
import { useRedirect } from '@/composables/useRedirect';
import { useAuth } from '@/core/adapter/auth';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';
import { useCookies } from '@/core/adapter/cookies';
import { useApi } from '@/core/adapter/helper/useApi';
import { BackendError, BackendErrorType, getBackendErrorTranslation } from '@/core/api/ApiError';
import { joinQuery } from '@/core/helper/utils';
import { CaptchaValidator } from '@/core/services/validator/captcha';
import FormLayout from '@/layouts/FormLayout.vue';

const { t } = useI18n();
const { resendVerificationEmail } = useApi();
const config = useConfig();
const auth = useAuth();
const route = useRoute();
const router = useRouter();
const { applyRedirectReplace } = useRedirect();

onMounted(() => {
  useCookies().loadRecaptcha();
});

const reCaptchaValidator = new CaptchaValidator();

const email = ref('');
const password = ref('');
const captchaResponse = ref<string | undefined>(undefined);
const showNotVerifiedInfo = ref(false);
const error = ref<string[]>([]);
const isLoading = ref<boolean>(false);
const isSubmitEnabled = computed(
  () => !isLoading.value && reCaptchaValidator.validate(captchaResponse.value).isValid && email.value?.trim() && password.value?.trim()
);

const logIn = async () => {
  error.value = [];
  isLoading.value = true;

  try {
    await auth.login(email.value, password.value, captchaResponse.value);
    if (!applyRedirectReplace()) {
      router.push('/home');
    }
  } catch (e: unknown) {
    captchaResponse.value = undefined;
    setTimeout(() => {
      if ((e as BackendErrorType)?.code === BackendError.UserNotVerified) {
        // show resend button
        showNotVerifiedInfo.value = true;
      }
      error.value = Array.isArray(e) ? e : [getBackendErrorTranslation(e as BackendErrorType)];
    });
  }
  isLoading.value = false;
};

const resendVerifyEmail = async () => {
  showNotVerifiedInfo.value = false;
  const res = await resendVerificationEmail(email.value, config.get(ZwooConfigKey.Language));
  if (res.isError) {
    error.value = [getBackendErrorTranslation(res.error)];
  }
};
</script>
