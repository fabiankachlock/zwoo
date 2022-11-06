<template>
  <FlatDialog>
    <Form show-back-button>
      <FormTitle>
        {{ t('createAccount.title') }}
      </FormTitle>
      <TextInput
        id="username"
        v-model="username"
        labelKey="createAccount.username"
        :placeholder="t('createAccount.username')"
        :validator="usernameValidator"
      />
      <TextInput
        id="email"
        v-model="email"
        labelKey="createAccount.email"
        :placeholder="t('createAccount.emailExample', ['@'])"
        :validator="emailValidator"
      />
      <TextInput id="password" v-model="password" labelKey="createAccount.password" is-password placeholder="******" :validator="passwordValidator" />
      <TextInput id="passwordRepeat" v-model="passwordRepeat" labelKey="createAccount.passwordRepeat" is-password placeholder="******" />
      <FormError :error="matchError" />
      <template v-if="isBeta">
        <TextInput id="beta-code" v-model="betaCode" labelKey="createAccount.beta" placeholder="xxx-xxx" />
      </template>
      <ReCaptchaButton @update:response="res => (reCaptchaResponse = res)" :validator="reCaptchaValidator" :response="reCaptchaResponse" />
      <FormError :error="error" />
      <FormActions>
        <FormSubmit @click="create">
          {{ t('createAccount.create') }}
        </FormSubmit>
        <FormAlternativeAction>
          <router-link :to="'/login?' + joinQuery(route.query)">{{ t('nav.login') }}</router-link>
        </FormAlternativeAction>
      </FormActions>
      <div v-if="showInfo" class="info border-2 rounded-lg bc-primary p-2 my-4 mx-2">
        <Icon icon="akar-icons:info" class="tc-primary text-xl mb-2" />
        <p class="tc-main-secondary">{{ t('createAccount.info') }}</p>
        <p class="tc-main-secondary">{{ t('createAccount.emailInfo') }}</p>
        <button @click="resendVerifyEmail" class="tc-primary mt-2 bg-main rounded-sm px-2 py-1 text-center">
          {{ t('createAccount.resendVerifyEmail') }}
        </button>
      </div>
    </Form>
  </FlatDialog>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { onMounted, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute } from 'vue-router';

import { Form, FormActions, FormAlternativeAction, FormError, FormSubmit, FormTitle, TextInput } from '@/components/forms/index';
import ReCaptchaButton from '@/components/forms/ReCaptchaButton.vue';
import FlatDialog from '@/components/misc/FlatDialog.vue';
import { useAuth } from '@/core/adapter/auth';
import { useCookies } from '@/core/adapter/cookies';
import { AuthenticationService } from '@/core/services/api/Authentication';
import { ReCaptchaResponse } from '@/core/services/api/Captcha';
import { getBackendErrorTranslation, unwrapBackendError } from '@/core/services/api/Errors';
import { joinQuery } from '@/core/services/utils';
import { EmailValidator } from '@/core/services/validator/email';
import { PasswordValidator } from '@/core/services/validator/password';
import { PasswordMatchValidator } from '@/core/services/validator/passwordMatch';
import { RecaptchaValidator } from '@/core/services/validator/recaptcha';
import { UsernameValidator } from '@/core/services/validator/username';

const { t } = useI18n();
const auth = useAuth();
const route = useRoute();
const isBeta = import.meta.env.VUE_APP_BETA === 'true';

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
const reCaptchaResponse = ref<ReCaptchaResponse | undefined>(undefined);
const error = ref<string[]>([]);
const showInfo = ref(false);
const showResend = ref(true);

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

  try {
    await auth.createAccount(username.value, email.value, password.value, passwordRepeat.value, reCaptchaResponse.value, betaCode.value);
    showInfo.value = true;
  } catch (e: unknown) {
    reCaptchaResponse.value = undefined;
    setTimeout(() => {
      error.value = Array.isArray(e) ? e : [(e as Error).toString()];
    });
  }
};

const resendVerifyEmail = async () => {
  if (!showResend.value) return;
  showResend.value = false;
  const res = await AuthenticationService.resendVerificationEmail(email.value);
  const [, err] = unwrapBackendError(res);
  if (err !== undefined) {
    error.value = [getBackendErrorTranslation(err)];
  }
};
</script>
