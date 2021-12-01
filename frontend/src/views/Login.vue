<template>
  <FlatDialog>
    <Form>
      <FormTitle> {{ t('login.title') }} </FormTitle>
      <TextInput id="username" v-model="username" labelKey="login.username" :placeholder="t('login.username')" />
      <TextInput id="password" v-model="password" labelKey="login.password" is-password placeholder="******" />
      <ReCaptchaButton @update:response="res => (reCaptchaResponse = res)" :validator="reCaptchaValidator" />
      <FormError :error="error" />
      <FormActions>
        <FormSubmit @click="logIn">
          {{ t('login.login') }}
        </FormSubmit>
        <FormSecondaryAction>
          {{ t('login.resetPassword') }}
        </FormSecondaryAction>
        <FormAlternativeAction>
          <router-link :to="'/create-account?' + joinQuery(route.query)">{{ t('nav.createAccount') }}</router-link>
        </FormAlternativeAction>
      </FormActions>
    </Form>
  </FlatDialog>
</template>

<script setup lang="ts">
import { Form, FormActions, FormAlternativeAction, FormError, FormSecondaryAction, FormSubmit, FormTitle, TextInput } from '@/components/forms/index';
import FlatDialog from '@/components/misc/FlatDialog.vue';
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute, useRouter } from 'vue-router';
import { joinQuery } from '@/core/services/utils';
import { useAuth } from '@/core/adapter/auth';
import { ReCaptchaResponse } from '@/core/services/api/reCAPTCHA';
import { RecaptchaValidator } from '@/core/services/validator/recaptcha';
import ReCaptchaButton from '@/components/forms/ReCaptchaButton.vue';

const { t } = useI18n();
const auth = useAuth();
const route = useRoute();
const router = useRouter();

const reCaptchaValidator = new RecaptchaValidator();

const username = ref('');
const password = ref('');
const reCaptchaResponse = ref<ReCaptchaResponse | undefined>(undefined);
const error = ref<string[]>([]);

const logIn = async () => {
  error.value = [];

  try {
    await auth.login(username.value, password.value, reCaptchaResponse.value);
    const redirect = route.query['redirect'] as string;

    if (redirect) {
      router.replace(redirect);
      return;
    }

    router.push('/home');
  } catch (e: any) {
    error.value = Array.isArray(e) ? e : [e.toString()];
  }
};
</script>
