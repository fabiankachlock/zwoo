<template>
  <div class="w-full sm:max-w-xs mx-auto">
    <form class="bg-lightest shadow-md sm:rounded-sm px-6 py-4 mb-4 mt-8 relative">
      <router-link to="/" class="tc-main-secondary absolute left-3 top-3 text-xl transform transition-transform hover:-translate-x-1">
        <Icon icon="mdi:chevron-left" />
      </router-link>
      <h1 class="tc-main my-3 text-center text-3xl">{{ t('createAccount.title') }}</h1>
      <div class="mb-4">
        <TextInput
          id="username"
          v-model="username"
          labelKey="createAccount.username"
          :placeholder="t('createAccount.username')"
          :validator="usernameValidator"
        />
      </div>
      <div class="mb-4">
        <TextInput
          id="email"
          v-model="email"
          labelKey="createAccount.email"
          :placeholder="t('createAccount.emailExample', ['@'])"
          :validator="emailValidator"
        />
      </div>
      <div class="mb-4">
        <TextInput
          id="password"
          v-model="password"
          labelKey="createAccount.password"
          is-password
          placeholder="******"
          :validator="passwordValidator"
        />
      </div>
      <div class="mb-4">
        <TextInput id="passwordRepeat" v-model="passwordRepeat" labelKey="createAccount.passwordRepeat" is-password placeholder="******" />
      </div>
      <div class="m-2">
        <Error v-if="matchError.length > 0" :errors="matchError" />
      </div>
      <div class="m-2">
        <Error v-if="error.length > 0" :errors="error" />
      </div>
      <div class="flex items-center flex-col justify-center">
        <button
          class="bg-darkest tc-main-light font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline transform transition hover:scale-95"
          type="button"
          @click="create"
        >
          {{ t('createAccount.create') }}
        </button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { useConfig } from '@/core/adapter/config';
import { EmailValidator } from '@/core/services/validator/email';
import { PasswordValidator } from '@/core/services/validator/password';
import { PasswordMatchValidator } from '@/core/services/validator/passwordMatch';
import { ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import TextInput from '../components/misc/TextInput.vue';
import Error from '../components/misc/Error.vue';
import { UsernameValidator } from '@/core/services/validator/username';

const { t } = useI18n();
const config = useConfig();

const username = ref('');
const email = ref('');
const password = ref('');
const passwordRepeat = ref('');
const matchError = ref<string[]>([]);
const error = ref<string[]>([]);

const emailValidator = new EmailValidator();
const usernameValidator = new UsernameValidator();
const passwordValidator = new PasswordValidator();
const passwordMatchValidator = new PasswordMatchValidator();

watch([password, passwordRepeat], ([password, passwordRepeat]) => {
  const result = passwordMatchValidator.validate([password, passwordRepeat]);
  matchError.value = result.isValid ? [] : result.getErrors();
});

watch([username, email, password, passwordRepeat], () => {
  // clear error since there are changes
  error.value = [];
});

const create = async () => {
  error.value = [];

  try {
    await config.createAccount(username.value, email.value, password.value, passwordRepeat.value);
  } catch (e: any) {
    error.value = Array.isArray(e) ? e : [e.toString()];
  }
};
</script>
