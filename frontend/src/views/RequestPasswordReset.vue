<template>
  <FormLayout>
    <Form show-back-button>
      <FormTitle> {{ t('requestPasswordReset.title') }} </FormTitle>
      <p class="m-2 tc-main-secondary text-sm">{{ t('requestPasswordReset.info') }}</p>
      <TextInput id="email" v-model="email" label-key="requestPasswordReset.email" :placeholder="t('requestPasswordReset.email')" />
      <CaptchaButton :validator="captchaValidator" :token="captchaResponse" @update:response="res => (captchaResponse = res)" />
      <FormError :error="error" />
      <FormActions>
        <FormSubmit :disabled="!isSubmitEnabled || showInfo" @click="requestReset">
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

import CaptchaButton from '@/components/forms/CaptchaButton.vue';
import { Form, FormActions, FormError, FormSecondaryAction, FormSubmit, FormTitle, TextInput } from '@/components/forms/index';
import { Icon } from '@/components/misc/Icon';
import { useRedirect } from '@/composables/useRedirect';
import { useAuth } from '@/core/adapter/auth';
import { useCookies } from '@/core/adapter/cookies';
import { joinQuery } from '@/core/helper/utils';
import { CaptchaValidator } from '@/core/services/validator/captcha';
import { EmailValidator } from '@/core/services/validator/email';
import FormLayout from '@/layouts/FormLayout.vue';

const { t } = useI18n();
const auth = useAuth();
const route = useRoute();
const { applyRedirectReplace } = useRedirect();

onMounted(() => {
  useCookies().loadRecaptcha();
});

const captchaValidator = new CaptchaValidator();
const emailValidator = new EmailValidator();

const email = ref('');
const captchaResponse = ref<string | undefined>(undefined);
const error = ref<string[]>([]);
const showInfo = ref(false);
const isLoading = ref<boolean>(false);
const isSubmitEnabled = computed(
  () => !isLoading.value && emailValidator.validate(email.value).isValid && captchaValidator.validate(captchaResponse.value).isValid
);

watch([email, captchaResponse], () => {
  // clear error since there are changes
  error.value = [];
});

const requestReset = async () => {
  error.value = [];
  isLoading.value = true;

  try {
    await auth.requestPasswordReset(email.value, captchaResponse.value);
    showInfo.value = true;
    applyRedirectReplace();
  } catch (e: unknown) {
    captchaResponse.value = undefined;
    setTimeout(() => {
      error.value = Array.isArray(e) ? e : [(e as Error).toString()];
    });
  }
  isLoading.value = false;
};
</script>
