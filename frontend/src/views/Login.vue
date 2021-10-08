<template>
  <div class="w-full sm:max-w-xs mx-auto">
    <form class="bg-lightest shadow-md sm:rounded-sm px-6 py-4 mb-4 mt-8 relative">
      <router-link to="/" class="tc-main-secondary absolute left-3 top-3 text-xl transform transition-transform hover:-translate-x-1">
        <Icon icon="mdi:chevron-left" />
      </router-link>
      <h1 class="tc-main my-3 text-center text-3xl">{{ t('login.title') }}</h1>
      <div class="mb-4">
        <TextInput id="username" v-model="username" labelKey="login.username" :placeholder="t('login.username')" />
      </div>
      <div class="mb-4">
        <TextInput id="password" v-model="password" labelKey="login.password" is-password placeholder="******" />
      </div>
      <div class="m-2">
        <Error v-if="error.length > 0" :errors="error" />
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
        <router-link
          class="inline-block align-baseline italic text-sm tc-main-secondary my-1 tc-main-dark hover:opacity-70 transition"
          :to="'/create-account?' + joinQuery(route.query)"
          >{{ t('nav.createAccount') }}</router-link
        >
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { useConfig } from '@/core/adapter/config';
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';
import Error from '../components/misc/Error.vue';
import TextInput from '../components/misc/TextInput.vue';
import { useRoute, useRouter } from 'vue-router';
import { joinQuery } from '@/core/services/utils';

const { t } = useI18n();
const config = useConfig();
const route = useRoute();
const router = useRouter();

const username = ref('');
const password = ref('');
const error = ref<string[]>([]);

const logIn = async () => {
  error.value = [];

  try {
    await config.login(username.value, password.value);
    const redirect = route.query['redirect'] as string;

    if (redirect) {
      router.push(redirect);
      return;
    }

    router.push('/home');
  } catch (e: any) {
    error.value = Array.isArray(e) ? e : [e.toString()];
  }
};
</script>
