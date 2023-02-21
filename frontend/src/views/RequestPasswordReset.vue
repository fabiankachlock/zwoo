<template>
  <FormLayout>
    <Form show-back-button>
      <FormTitle> {{ t('requestPasswordReset.title') }} </FormTitle>
      <p class="m-2 tc-main-secondary text-sm">{{ t('requestPasswordReset.info') }}</p>
      <TextInput id="email" v-model="email" labelKey="requestPasswordReset.email" :placeholder="t('requestPasswordReset.email')" />
      <ReCaptchaButton @update:response="res => (reCaptchaResponse = res)" :validator="reCaptchaValidator" :response="reCaptchaResponse" />
      <FormError :error="error" />
      <FormActions>
        <FormSubmit @click="requestReset" :disabled="!isSubmitEnabled || showInfo">
          {{ t('requestPasswordReset.request') }}
        </FormSubmit>
        <FormSecondaryAction>
          <router-link class="w-full block text-center" :to="'/login?' + joinQuery(route.query)">{{ t('requestPasswordReset.login') }}</router-link>
        </FormSecondaryAction>
      </FormActions>
      <div v-if="showInfo" class="info border-2 rounded-lg bc-primary p-2 my-4 mx-2">
        <Icon icon="akar-icons:info" class="tc-primary text-xl mb-2" />
        <p class="tc-main-secondary">{{ t('requestPasswordReset.info') }}</p>
      </div>
    </Form>
  </FormLayout>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute } from 'vue-router';

import { Form, FormActions, FormError, FormSecondaryAction, FormSubmit, FormTitle, TextInput } from '@/components/forms/index';
import ReCaptchaButton from '@/components/forms/ReCaptchaButton.vue';
import { Icon } from '@/components/misc/Icon';
import { useRedirect } from '@/composables/useRedirect';
import { useAuth } from '@/core/adapter/auth';
import { useCookies } from '@/core/adapter/cookies';
import { ReCaptchaResponse } from '@/core/services/api/Captcha';
import { joinQuery } from '@/core/services/utils';
import { EmailValidator } from '@/core/services/validator/email';
import { RecaptchaValidator } from '@/core/services/validator/recaptcha';
import FormLayout from '@/layouts/FormLayout.vue';

const { t } = useI18n();
const auth = useAuth();
const route = useRoute();
const { applyRedirectReplace } = useRedirect();

onMounted(() => {
  useCookies().loadRecaptcha();
});

const reCaptchaValidator = new RecaptchaValidator();
const emailValidator = new EmailValidator();

const email = ref('');
const reCaptchaResponse = ref<ReCaptchaResponse | undefined>(undefined);
const error = ref<string[]>([]);
const showInfo = ref(false);
const isLoading = ref<boolean>(false);
const isSubmitEnabled = computed(
  () => !isLoading.value && emailValidator.validate(email.value).isValid && reCaptchaValidator.validate(reCaptchaResponse.value).isValid
);

watch([email, reCaptchaResponse], () => {
  // clear error since there are changes
  error.value = [];
});

const requestReset = async () => {
  error.value = [];
  isLoading.value = true;

  try {
    await auth.requestPasswordReset(email.value, reCaptchaResponse.value);
    showInfo.value = true;
    applyRedirectReplace();
  } catch (e: unknown) {
    reCaptchaResponse.value = undefined;
    setTimeout(() => {
      error.value = Array.isArray(e) ? e : [(e as Error).toString()];
    });
  }
  isLoading.value = false;
};
</script>
