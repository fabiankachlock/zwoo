<template>
  <FormLayout>
    <Form>
      <FormTitle>
        {{ t('resetPassword.title') }}
      </FormTitle>
      <TextInput
        id="password"
        v-model="password"
        label-key="resetPassword.password"
        is-password
        placeholder="******"
        :validator="passwordValidator"
      />
      <TextInput id="passwordRepeat" v-model="passwordRepeat" label-key="resetPassword.passwordRepeat" is-password placeholder="******" />
      <FormError :error="matchError" />
      <CaptchaButton :validator="captchaValidator" :token="captchaResponse" @update:response="res => (captchaResponse = res)" />
      <FormError :error="error" />
      <FormActions>
        <FormSubmit :disabled="!isSubmitEnabled || showInfo" @click="reset">
          {{ t('resetPassword.reset') }}
        </FormSubmit>
      </FormActions>
      <div v-if="showInfo" class="info border-2 rounded-lg bc-primary p-2 my-4 mx-2">
        <Icon icon="akar-icons:info" class="tc-primary text-xl mb-2" />
        <p class="tc-main-secondary">{{ t('resetPassword.info') }}</p>
        <router-link to="/login">
          <p class="tc-primary mt-2 bg-main rounded-sm px-2 py-1 text-center">
            {{ t('resetPassword.login') }}
          </p>
        </router-link>
      </div>
    </Form>
  </FormLayout>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute } from 'vue-router';

import CaptchaButton from '@/components/forms/CaptchaButton.vue';
import { Form, FormActions, FormError, FormSubmit, FormTitle, TextInput } from '@/components/forms/index';
import { Icon } from '@/components/misc/Icon';
import { useAuth } from '@/core/adapter/auth';
import { useCookies } from '@/core/adapter/cookies';
import { CaptchaValidator } from '@/core/services/validator/captcha';
import { PasswordValidator } from '@/core/services/validator/password';
import { PasswordMatchValidator } from '@/core/services/validator/passwordMatch';
import FormLayout from '@/layouts/FormLayout.vue';

const { t } = useI18n();
const auth = useAuth();
const route = useRoute();

onMounted(() => {
  useCookies().loadRecaptcha();
});

const code = route.query['code'] as string;
const password = ref('');
const passwordRepeat = ref('');
const matchError = ref<string[]>([]);
const captchaResponse = ref<string | undefined>(undefined);
const error = ref<string[]>([]);
const showInfo = ref(false);
const isLoading = ref<boolean>(false);
const isSubmitEnabled = computed(
  () =>
    !isLoading.value &&
    captchaValidator.validate(captchaResponse.value).isValid &&
    passwordValidator.validate(password.value).isValid &&
    passwordMatchValidator.validate([password.value, passwordRepeat.value]).isValid
);

const passwordValidator = new PasswordValidator();
const passwordMatchValidator = new PasswordMatchValidator();
const captchaValidator = new CaptchaValidator();

watch([password, passwordRepeat], ([password, passwordRepeat]) => {
  const result = passwordMatchValidator.validate([password, passwordRepeat]);
  matchError.value = result.isValid ? [] : result.getErrors();
});

watch([password, passwordRepeat, captchaResponse], () => {
  // clear error since there are changes
  error.value = [];
});

const reset = async () => {
  error.value = [];
  isLoading.value = true;

  try {
    await auth.resetPassword(code, password.value, passwordRepeat.value, captchaResponse.value);
    showInfo.value = true;
  } catch (e: unknown) {
    captchaResponse.value = undefined;
    setTimeout(() => {
      error.value = Array.isArray(e) ? e : [(e as Error).toString()];
    });
  }
  isLoading.value = false;
};
</script>
