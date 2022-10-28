<template>
  <FlatDialog>
    <Form show-back-button>
      <FormTitle> {{ t('requestPasswordReset.title') }} </FormTitle>
      <p class="m-2 tc-main-secondary text-sm">{{ t('requestPasswordReset.info') }}</p>
      <TextInput id="email" v-model="email" labelKey="requestPasswordReset.email" :placeholder="t('requestPasswordReset.email')" />
      <ReCaptchaButton @update:response="res => (reCaptchaResponse = res)" :validator="reCaptchaValidator" :response="reCaptchaResponse" />
      <FormError :error="error" />
      <FormActions>
        <FormSubmit @click="requestReset">
          {{ t('requestPasswordReset.request') }}
        </FormSubmit>
        <FormSecondaryAction>
          <router-link :to="'/login?' + joinQuery(route.query)">{{ t('requestPasswordReset.login') }}</router-link>
        </FormSecondaryAction>
      </FormActions>
    </Form>
  </FlatDialog>
</template>

<script setup lang="ts">
import { onMounted, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute, useRouter } from 'vue-router';

import { Form, FormActions, FormError, FormSecondaryAction, FormSubmit, FormTitle, TextInput } from '@/components/forms/index';
import ReCaptchaButton from '@/components/forms/ReCaptchaButton.vue';
import FlatDialog from '@/components/misc/FlatDialog.vue';
import { useAuth } from '@/core/adapter/auth';
import { useCookies } from '@/core/adapter/cookies';
import { ReCaptchaResponse } from '@/core/services/api/Captcha';
import { joinQuery } from '@/core/services/utils';
import { RecaptchaValidator } from '@/core/services/validator/recaptcha';

const { t } = useI18n();
const auth = useAuth();
const route = useRoute();
const router = useRouter();

onMounted(() => {
  useCookies().loadRecaptcha();
});

const reCaptchaValidator = new RecaptchaValidator();

const email = ref('');
const reCaptchaResponse = ref<ReCaptchaResponse | undefined>(undefined);
const error = ref<string[]>([]);

watch([email, reCaptchaResponse], () => {
  // clear error since there are changes
  error.value = [];
});

const requestReset = async () => {
  error.value = [];
  try {
    await auth.requestPasswordReset(email.value, reCaptchaResponse.value);
    const redirect = route.query['redirect'] as string;

    if (redirect) {
      router.replace(redirect);
      return;
    }
  } catch (e: unknown) {
    error.value = Array.isArray(e) ? e : [(e as Error).toString()];
    reCaptchaResponse.value = undefined;
  }
};
</script>
