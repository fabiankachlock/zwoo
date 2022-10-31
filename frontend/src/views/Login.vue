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
import { useRedirect } from '@/composables/useRedirect';
import { useAuth } from '@/core/adapter/auth';
import { useCookies } from '@/core/adapter/cookies';
import { ReCaptchaResponse } from '@/core/services/api/Captcha';
import { joinQuery } from '@/core/services/utils';
import { RecaptchaValidator } from '@/core/services/validator/recaptcha';
import FormLayout from '@/layouts/FormLayout.vue';

const { t } = useI18n();
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
const error = ref<string[]>([]);
const isSubmitEnabled = computed(() => reCaptchaValidator.validate(reCaptchaResponse.value).isValid && email.value?.trim() && password.value?.trim());

const logIn = async () => {
  error.value = [];

  try {
    await auth.login(email.value, password.value, reCaptchaResponse.value);
    if (!applyRedirectReplace()) {
      router.push('/home');
    }
  } catch (e: unknown) {
    reCaptchaResponse.value = undefined;
    setTimeout(() => {
      error.value = Array.isArray(e) ? e : [(e as Error).toString()];
    });
  }
};
</script>
