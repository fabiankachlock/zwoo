<template>
  <FormLayout>
    <Form show-back-button>
      <FormTitle> {{ t('login.title') }} </FormTitle>
      <TextInput id="email" v-model="email" labelKey="login.email" :placeholder="t('login.email')" />
      <TextInput id="password" v-model="password" labelKey="login.password" is-password placeholder="******" />
      <ReCaptchaButton @update:response="res => (reCaptchaResponse = res)" :validator="reCaptchaValidator" :response="reCaptchaResponse" />
      <FormError :error="error" />
      <FormActions>
        <FormSubmit @click="logIn" :disabled="!isSubmitEnabled">
          {{ t('login.login') }}
        </FormSubmit>
        <div v-if="showNotVerifiedInfo" class="info border-2 rounded-lg bc-primary p-2 my-4 mx-2">
          <Icon icon="akar-icons:info" class="tc-primary text-xl mb-2" />
          <p class="tc-main-secondary">{{ t('login.notVerifiedInfo') }}</p>
          <button @click="resendVerifyEmail" class="tc-primary mt-2 bg-main hover:bg-dark rounded-sm px-2 py-1 text-center">
            {{ t('login.resendVerifyEmail') }}
          </button>
        </div>
        <FormSecondaryAction>
          <router-link :to="'/request-password-reset?' + joinQuery(route.query)">{{ t('login.resetPassword') }}</router-link>
        </FormSecondaryAction>
        <FormAlternativeAction>
          <router-link :to="'/create-account?' + joinQuery(route.query)">{{ t('nav.createAccount') }}</router-link>
        </FormAlternativeAction>
      </FormActions>
    </Form>
  </FormLayout>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute, useRouter } from 'vue-router';

import { Form, FormActions, FormAlternativeAction, FormError, FormSecondaryAction, FormSubmit, FormTitle, TextInput } from '@/components/forms/index';
import ReCaptchaButton from '@/components/forms/ReCaptchaButton.vue';
import { Icon } from '@/components/misc/Icon';
import { useRedirect } from '@/composables/useRedirect';
import { useAuth } from '@/core/adapter/auth';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';
import { useCookies } from '@/core/adapter/cookies';
import { AuthenticationService } from '@/core/services/api/Authentication';
import { ReCaptchaResponse } from '@/core/services/api/Captcha';
import { BackendError, BackendErrorType, getBackendErrorTranslation, unwrapBackendError } from '@/core/services/api/Errors';
import { joinQuery } from '@/core/services/utils';
import { RecaptchaValidator } from '@/core/services/validator/recaptcha';
import FormLayout from '@/layouts/FormLayout.vue';

const { t } = useI18n();
const config = useConfig();
const auth = useAuth();
const route = useRoute();
const router = useRouter();
const { applyRedirectReplace } = useRedirect();

onMounted(() => {
  useCookies().loadRecaptcha();
});

const reCaptchaValidator = new RecaptchaValidator();

const email = ref('');
const password = ref('');
const reCaptchaResponse = ref<ReCaptchaResponse | undefined>(undefined);
const showNotVerifiedInfo = ref(false);
const error = ref<string[]>([]);
const isLoading = ref<boolean>(false);
const isSubmitEnabled = computed(
  () => !isLoading.value && reCaptchaValidator.validate(reCaptchaResponse.value).isValid && email.value?.trim() && password.value?.trim()
);

const logIn = async () => {
  error.value = [];
  isLoading.value = true;

  try {
    await auth.login(email.value, password.value, reCaptchaResponse.value);
    if (!applyRedirectReplace()) {
      router.push('/home');
    }
  } catch (e: unknown) {
    reCaptchaResponse.value = undefined;
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
  const res = await AuthenticationService.resendVerificationEmail(email.value, config.get(ZwooConfigKey.Language));
  const [, err] = unwrapBackendError(res);
  if (err !== undefined) {
    error.value = [getBackendErrorTranslation(err)];
  }
};
</script>
