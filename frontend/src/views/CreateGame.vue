<template>
  <FormLayout>
    <Form show-back-button>
      <FormTitle>
        {{ t('createGame.title') }}
      </FormTitle>
      <TextInput id="game-name" v-model="name" label-key="createGame.name" :placeholder="t('createGame.name')" :validator="nameValidator" />
      <Checkbox v-model="isPublic" styles="tc-primary mx-3">
        {{ t('createGame.isPublic') }}
      </Checkbox>
      <TextInput v-if="!isPublic" id="game-password" v-model="password" label-key="createGame.password" is-password placeholder="******" />
      <FormError :error="error" />
      <FormActions>
        <FormSubmit :disabled="!nameValidator.validate(name).isValid || isLoading" @click="create">
          {{ t('createAccount.create') }}
        </FormSubmit>
      </FormActions>
    </Form>
  </FormLayout>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';

import { Checkbox, Form, FormActions, FormError, FormSubmit, FormTitle, TextInput } from '@/components/forms/index';
import { useGameConfig } from '@/core/adapter/game';
import { GameNameValidator } from '@/core/services/validator/gameName';
import FormLayout from '@/layouts/FormLayout.vue';

const gameConfig = useGameConfig();
const nameValidator = new GameNameValidator();
const router = useRouter();

const { t } = useI18n();

const name = ref('');
const isPublic = ref(true);
const password = ref('');
const error = ref<string[]>([]);
const isLoading = ref<boolean>(false);

watch([name, isPublic, password], () => {
  // clear error since there are changes
  error.value = [];
});

const create = async () => {
  error.value = [];
  isLoading.value = true;

  try {
    await gameConfig.create(name.value, isPublic.value, password.value);
    router.push('/game/wait');
  } catch (e: unknown) {
    error.value = Array.isArray(e) ? e : [(e as Error).toString()];
  }
  isLoading.value = false;
};
</script>
