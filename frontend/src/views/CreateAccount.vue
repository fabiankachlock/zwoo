<template>
  <FlatDialog>
    <Form>
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
      <ReCaptchaButton @update:response="res => (reCaptchaResponse = res)" :validator="reCaptchaValidator" />
      <FormError :error="error" />
      <FormActions>
        <FormSubmit @click="create">
          {{ t('createAccount.create') }}
        </FormSubmit>
        <FormAlternativeAction>
          <router-link :to="'/login?' + joinQuery(route.query)">{{ t('nav.login') }}</router-link>
        </FormAlternativeAction>
      </FormActions>
    </Form>
  </FlatDialog>
</template>

<script setup lang="ts">
import { Form, FormActions, FormAlternativeAction, FormError, FormSubmit, FormTitle, TextInput } from '@/components/forms/index';
import FlatDialog from '@/components/misc/FlatDialog.vue';
import { EmailValidator } from '@/core/services/validator/email';
import { PasswordValidator } from '@/core/services/validator/password';
import { PasswordMatchValidator } from '@/core/services/validator/passwordMatch';
import { ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { UsernameValidator } from '@/core/services/validator/username';
import { useRoute, useRouter } from 'vue-router';
import { joinQuery } from '@/core/services/utils';
import { useAuth } from '@/core/adapter/auth';
import ReCaptchaButton from '@/components/forms/ReCaptchaButton.vue';
import { RecaptchaValidator } from '@/core/services/validator/recaptcha';
import { ReCaptchaResponse } from '@/core/services/api/reCAPTCHA';

const { t } = useI18n();
const auth = useAuth();
const route = useRoute();
const router = useRouter();

const username = ref('');
const email = ref('');
const password = ref('');
const passwordRepeat = ref('');
const matchError = ref<string[]>([]);
const reCaptchaResponse = ref<ReCaptchaResponse | undefined>(undefined);
const error = ref<string[]>([]);

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
    await auth.createAccount(username.value, email.value, password.value, passwordRepeat.value, reCaptchaResponse.value);
    const redirect = route.query['redirect'] as string;

    if (redirect) {
      router.replace(redirect);
      return;
    }

    router.push('/home');
  } catch (e: unknown) {
    error.value = Array.isArray(e) ? e : [(e as Error).toString()];
  }
};
</script>
