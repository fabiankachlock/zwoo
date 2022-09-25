<template>
  <FlatDialog>
    <Form>
      <FormTitle>
        {{ t('resetPassword.title') }}
      </FormTitle>
      <TextInput id="password" v-model="password" labelKey="resetPassword.password" is-password placeholder="******" :validator="passwordValidator" />
      <TextInput id="passwordRepeat" v-model="passwordRepeat" labelKey="resetPassword.passwordRepeat" is-password placeholder="******" />
      <FormError :error="matchError" />
      <ReCaptchaButton @update:response="res => (reCaptchaResponse = res)" :validator="reCaptchaValidator" />
      <FormError :error="error" />
      <FormActions>
        <FormSubmit @click="create">
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
  </FlatDialog>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { Form, FormActions, FormError, FormSubmit, FormTitle, TextInput } from '@/components/forms/index';
import FlatDialog from '@/components/misc/FlatDialog.vue';
import { PasswordValidator } from '@/core/services/validator/password';
import { PasswordMatchValidator } from '@/core/services/validator/passwordMatch';
import { onMounted, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
// import { useAuth } from '@/core/adapter/auth';
import ReCaptchaButton from '@/components/forms/ReCaptchaButton.vue';
import { RecaptchaValidator } from '@/core/services/validator/recaptcha';
import { ReCaptchaResponse } from '@/core/services/api/reCAPTCHA';
import { useCookies } from '@/core/adapter/cookies';

const { t } = useI18n();
// const auth = useAuth();

onMounted(() => {
  useCookies().loadRecaptcha();
});

const password = ref('');
const passwordRepeat = ref('');
const matchError = ref<string[]>([]);
const reCaptchaResponse = ref<ReCaptchaResponse | undefined>(undefined);
const error = ref<string[]>([]);
const showInfo = ref(false);

const passwordValidator = new PasswordValidator();
const passwordMatchValidator = new PasswordMatchValidator();
const reCaptchaValidator = new RecaptchaValidator();

watch([password, passwordRepeat], ([password, passwordRepeat]) => {
  const result = passwordMatchValidator.validate([password, passwordRepeat]);
  matchError.value = result.isValid ? [] : result.getErrors();
});

watch([password, passwordRepeat], () => {
  // clear error since there are changes
  error.value = [];
});

const create = async () => {
  error.value = [];

  try {
    // await auth.resetPassword(password.value, passwordRepeat.value, reCaptchaResponse.value);
    showInfo.value = true;
  } catch (e: unknown) {
    error.value = Array.isArray(e) ? e : [(e as Error).toString()];
  }
};
</script>
