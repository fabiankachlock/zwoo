<template>
  <FormLayout>
    <Form show-back-button>
      <FormTitle>
        {{ t('createAccount.title') }}
      </FormTitle>
      <TextInput
        id="username"
        v-model="username"
        label-key="createAccount.username"
        :placeholder="t('createAccount.username')"
        :validator="usernameValidator"
      />
      <TextInput
        id="email"
        v-model="email"
        label-key="createAccount.email"
        :placeholder="t('createAccount.emailExample', ['@'])"
        :validator="emailValidator"
      />
      <TextInput
        id="password"
        v-model="password"
        label-key="createAccount.password"
        is-password
        placeholder="******"
        :validator="passwordValidator"
      />
      <TextInput id="passwordRepeat" v-model="passwordRepeat" label-key="createAccount.passwordRepeat" is-password placeholder="******" />
      <FormError :error="matchError" />
      <template v-if="isBeta">
        <TextInput id="beta-code" v-model="betaCode" label-key="createAccount.beta" placeholder="xxx-xxx" />
      </template>
      <ReCaptchaButton :validator="reCaptchaValidator" :response="reCaptchaResponse" @update:response="res => (reCaptchaResponse = res)" />
      <FormError :error="error" />
      <FormActions>
        <FormSubmit :disabled="!isSubmitEnabled || showInfo" @click="create">
          {{ t('createAccount.create') }}
        </FormSubmit>
        <FormAlternativeAction>
          <router-link class="w-full block text-center" :to="'/login?' + joinQuery(route.query)">{{ t('nav.login') }}</router-link>
        </FormAlternativeAction>
      </FormActions>
      <div v-if="showInfo" class="info border-2 rounded-lg bc-primary p-2 my-4 mx-2">
        <Icon icon="akar-icons:info" class="tc-primary text-xl mb-2" />
        <p class="tc-main-secondary">{{ t('createAccount.info') }}</p>
        <p class="tc-main-secondary">{{ t('createAccount.emailInfo') }}</p>
        <button class="tc-primary mt-2 bg-main hover:bg-dark rounded-sm px-2 py-1 text-center" @click="resendVerifyEmail">
          {{ t('createAccount.resendVerifyEmail') }}
        </button>
      </div>
    </Form>
  </FormLayout>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute } from 'vue-router';

import { Form, FormActions, FormAlternativeAction, FormError, FormSubmit, FormTitle, TextInput } from '@/components/forms/index';
import ReCaptchaButton from '@/components/forms/ReCaptchaButton.vue';
import { Icon } from '@/components/misc/Icon';
import { AppConfig } from '@/config';
import { useAuth } from '@/core/adapter/auth';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';
import { useCookies } from '@/core/adapter/cookies';
import { useApi } from '@/core/adapter/helper/useApi';
import { getBackendErrorTranslation, unwrapBackendError } from '@/core/api/ApiError';
import { CaptchaResponse } from '@/core/api/entities/Captcha';
import { joinQuery } from '@/core/helper/utils';
import { EmailValidator } from '@/core/services/validator/email';
import { PasswordValidator } from '@/core/services/validator/password';
import { PasswordMatchValidator } from '@/core/services/validator/passwordMatch';
import { RecaptchaValidator } from '@/core/services/validator/recaptcha';
import { UsernameValidator } from '@/core/services/validator/username';
import FormLayout from '@/layouts/FormLayout.vue';

const { t } = useI18n();
const { resendVerificationEmail } = useApi();
const config = useConfig();
const auth = useAuth();
const route = useRoute();
const isBeta = AppConfig.IsBeta;

onMounted(() => {
  useCookies().loadRecaptcha();
  const code = route.query['b'];
  if (code) {
    betaCode.value = code as string;
  }
});

const username = ref('');
const email = ref('');
const password = ref('');
const passwordRepeat = ref('');
const betaCode = ref('');
const matchError = ref<string[]>([]);
const reCaptchaResponse = ref<CaptchaResponse | undefined>(undefined);
const error = ref<string[]>([]);
const showInfo = ref(false);
const isLoading = ref<boolean>(false);
const showResend = ref(true);
const isSubmitEnabled = computed(
  () =>
    !isLoading.value &&
    reCaptchaValidator.validate(reCaptchaResponse.value).isValid &&
    emailValidator.validate(email.value).isValid &&
    passwordValidator.validate(password.value).isValid &&
    passwordMatchValidator.validate([password.value, passwordRepeat.value]).isValid &&
    usernameValidator.validate(username.value).isValid
);

const emailValidator = new EmailValidator();
const usernameValidator = new UsernameValidator();
const passwordValidator = new PasswordValidator();
const passwordMatchValidator = new PasswordMatchValidator();
const reCaptchaValidator = new RecaptchaValidator();

watch([password, passwordRepeat], ([password, passwordRepeat]) => {
  const result = passwordMatchValidator.validate([password, passwordRepeat]);
  matchError.value = result.isValid ? [] : result.getErrors();
});

watch([username, email, password, passwordRepeat], () => {
  // clear error since there are changes
  error.value = [];
});

const create = async () => {
  error.value = [];
  isLoading.value = true;
  try {
    await auth.createAccount(username.value, email.value, password.value, passwordRepeat.value, reCaptchaResponse.value, betaCode.value);
    showInfo.value = true;
  } catch (e: unknown) {
    reCaptchaResponse.value = undefined;
    setTimeout(() => {
      error.value = Array.isArray(e) ? e : [(e as Error).toString()];
    });
  }
  isLoading.value = false;
};

const resendVerifyEmail = async () => {
  if (!showResend.value) return;
  showResend.value = false;
  const res = await resendVerificationEmail(email.value, config.get(ZwooConfigKey.Language));
  const [, err] = unwrapBackendError(res);
  if (err !== undefined) {
    error.value = [getBackendErrorTranslation(err)];
  }
};
</script>
