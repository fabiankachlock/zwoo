<template>
  <div class="w-full sm:max-w-xs mx-auto">
    <form class="bg-lightest shadow-md sm:rounded-sm px-6 py-4 mb-4 mt-8">
      <h1 class="tc-main my-3 text-center text-3xl">{{ t('login.title') }}</h1>
      <div class="mb-4">
        <TextInput id="username" v-model="username" labelKey="login.username" :placeholder="t('login.username')" />
      </div>
      <div class="mb-4">
        <TextInput id="password" v-model="password" labelKey="login.password" is-password placeholder="******" />
      </div>
      <div class="m-2">
        <Error v-if="error !== undefined" :title="error" />
      </div>
      <div class="flex items-center flex-col justify-center">
        <button
          class="bg-darkest tc-main-light font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline transform transition hover:scale-95"
          type="button"
          @click="logIn"
        >
          {{ t('login.login') }}
        </button>
        <a class="inline-block align-baseline italic text-xs tc-main-secondary my-1 tc-main-dark hover:opacity-70 transition" href="#">{{
          t('login.resetPassword')
        }}</a>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { useConfig } from '@/core/adapter/config';
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';
import Error from '../components/misc/Error.vue';
import TextInput from '../components/misc/TextInput.vue';

const { t } = useI18n();
const config = useConfig();

const username = ref('');
const password = ref('');
const error = ref<string | undefined>(undefined);

const logIn = async () => {
  try {
    await config.login(username.value, password.value);
  } catch (e) {
    if (Array.isArray(e)) {
      error.value = e.join('\n');
    }
  }
};
</script>
