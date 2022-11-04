<template>
  <div>
    <button
      class="flex justify-center items-center bg-light border-2 border-transparent px-2 rounded transition hover:bg-main cursor-pointer select-none"
      @click="openDialog"
    >
      <p class="tc-primary text-center">{{ t('settings.changePassword') }}</p>
    </button>
    <div v-if="showDialog">
      <FloatingDialog content-class="max-w-lg">
        <Form show-close-button @close="showDialog = false">
          <FormTitle>
            {{ t('changePassword.title') }}
          </FormTitle>
          <TextInput id="password" v-model="oldPassword" labelKey="changePassword.oldPassword" is-password placeholder="******" />
          <TextInput
            id="password"
            v-model="password"
            labelKey="changePassword.newPassword"
            is-password
            placeholder="******"
            :validator="passwordValidator"
          />
          <TextInput
            id="password"
            v-model="passwordRepeat"
            labelKey="changePassword.newPasswordRepeat"
            is-password
            placeholder="******"
            :validator="passwordValidator"
          />
          <FormError :error="matchError" />
          <FormError :error="error" />
          <FormActions>
            <FormSubmit @click="changePassword" :disabled="!isSubmitEnabled">
              {{ t('changePassword.change') }}
            </FormSubmit>
          </FormActions>
        </Form>
      </FloatingDialog>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';

import { useAuth } from '@/core/adapter/auth';
import { PasswordValidator } from '@/core/services/validator/password';
import { PasswordMatchValidator } from '@/core/services/validator/passwordMatch';

import Form from '../forms/Form.vue';
import FormActions from '../forms/FormActions.vue';
import FormError from '../forms/FormError.vue';
import FormSubmit from '../forms/FormSubmit.vue';
import FormTitle from '../forms/FormTitle.vue';
import TextInput from '../forms/TextInput.vue';
import FloatingDialog from '../misc/FloatingDialog.vue';

const auth = useAuth();
const { t } = useI18n();

const showDialog = ref(false);
const oldPassword = ref('');
const password = ref('');
const passwordRepeat = ref('');
const matchError = ref<string[]>([]);
const error = ref<string[]>([]);
const isLoading = ref<boolean>(false);
const isSubmitEnabled = computed(
  () =>
    !isLoading.value &&
    passwordValidator.validate(password.value).isValid &&
    passwordMatchValidator.validate([password.value, passwordRepeat.value]).isValid
);

const passwordValidator = new PasswordValidator();
const passwordMatchValidator = new PasswordMatchValidator();

watch([password, passwordRepeat], ([password, passwordRepeat]) => {
  const result = passwordMatchValidator.validate([password, passwordRepeat]);
  matchError.value = result.isValid ? [] : result.getErrors();
});

const openDialog = () => {
  showDialog.value = true;
};

const changePassword = async () => {
  error.value = [];
  isLoading.value = true;
  try {
    await auth.changePassword(oldPassword.value, password.value, passwordRepeat.value);
    showDialog.value = false;
  } catch (e: unknown) {
    error.value = Array.isArray(e) ? e : [(e as Error).toString()];
  }
  isLoading.value = false;
};
</script>
