<template>
  <div>
    <button
      class="flex justify-center items-center bg-alt border-2 border-transparent px-2 rounded transition hover:bg-alt-hover cursor-pointer select-none"
      @click="openDialog"
    >
      <p class="text-warning-text text-center">{{ t('settings.changePassword') }}</p>
    </button>
    <div v-if="showDialog">
      <FloatingDialog content-class="max-w-lg">
        <Form show-close-button @close="showDialog = false">
          <FormTitle>
            {{ t('changePassword.title') }}
          </FormTitle>
          <TextInput id="password" v-model="oldPassword" label-key="changePassword.oldPassword" is-password placeholder="******" />
          <TextInput
            id="password"
            v-model="password"
            label-key="changePassword.newPassword"
            is-password
            placeholder="******"
            :validator="passwordValidator"
          />
          <TextInput
            id="password"
            v-model="passwordRepeat"
            label-key="changePassword.newPasswordRepeat"
            is-password
            placeholder="******"
            :validator="passwordValidator"
          />
          <FormError :error="matchError" />
          <FormError :error="error" />
          <FormActions>
            <FormSubmit :disabled="!isSubmitEnabled" @click="changePassword">
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

import Form from '@/components/forms/Form.vue';
import FormActions from '@/components/forms/FormActions.vue';
import FormError from '@/components/forms/FormError.vue';
import FormSubmit from '@/components/forms/FormSubmit.vue';
import FormTitle from '@/components/forms/FormTitle.vue';
import TextInput from '@/components/forms/TextInput.vue';
import FloatingDialog from '@/components/misc/FloatingDialog.vue';
import { useAuth } from '@/core/adapter/auth';
import { PasswordValidator } from '@/core/services/validator/password';
import { PasswordMatchValidator } from '@/core/services/validator/passwordMatch';

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
