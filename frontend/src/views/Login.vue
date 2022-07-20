<template>
  <FlatDialog>
    <Form show-back-button>
      <FormTitle> {{ t('login.title') }} </FormTitle>
      <TextInput id="email" v-model="email" labelKey="login.email" :placeholder="t('login.email')" />
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
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute, useRouter } from 'vue-router';
import { joinQuery } from '@/core/services/utils';
import { useAuth } from '@/core/adapter/auth';
import { ReCaptchaResponse } from '@/core/services/api/reCAPTCHA';
import { RecaptchaValidator } from '@/core/services/validator/recaptcha';
import ReCaptchaButton from '@/components/forms/ReCaptchaButton.vue';
import { useCookies } from '@/core/adapter/cookies';

const { t } = useI18n();
const auth = useAuth();
const route = useRoute();
const router = useRouter();

onMounted(() => {
  useCookies().loadRecaptcha();
});

const reCaptchaValidator = new RecaptchaValidator();

const email = ref('');
const password = ref('');
const reCaptchaResponse = ref<ReCaptchaResponse | undefined>(undefined);
const error = ref<string[]>([]);

const logIn = async () => {
  error.value = [];

  try {
    await auth.login(email.value, password.value, reCaptchaResponse.value);
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
