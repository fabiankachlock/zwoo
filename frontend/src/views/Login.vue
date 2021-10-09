<template>
  <FlatDialog>
    <Form>
      <FormTitle> {{ t('login.title') }} </FormTitle>
      <TextInput id="username" v-model="username" labelKey="login.username" :placeholder="t('login.username')" />
      <TextInput id="password" v-model="password" labelKey="login.password" is-password placeholder="******" />
      <FormError :error="error" />
      <FormActions>
        <FormSubmit @click="logIn">
          {{ t('login.login') }}
        </FormSubmit>
        <FormSecondaryAction>
          {{ t('login.resetPassword') }}
        </FormSecondaryAction>
        <FormAlternativeAction>
          <router-link :to="'/create-account?' + joinQuery(route.query)">{{ t('nav.createAccount') }}</router-link>
        </FormAlternativeAction>
      </FormActions>
    </Form>
  </FlatDialog>
</template>

<script setup lang="ts">
import { Form, FormTitle, FormError, TextInput, FormAlternativeAction, FormSecondaryAction, FormSubmit, FormActions } from '@/components/forms/index';
import FlatDialog from '@/components/misc/FlatDialog.vue';
import { useConfig } from '@/core/adapter/config';
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';
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
