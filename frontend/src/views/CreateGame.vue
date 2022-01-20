<template>
  <FlatDialog>
    <Form>
      <FormTitle>
        {{ t('createGame.title') }}
      </FormTitle>
      <TextInput id="name" v-model="name" labelKey="createGame.name" :placeholder="t('createGame.name')" :validator="nameValidator" />
      <Checkbox styles="tc-primary mx-3" v-model="isPublic">
        {{ t('createGame.isPublic') }}
      </Checkbox>
      <TextInput id="password" v-model="password" labelKey="createGame.password" is-password placeholder="******" v-show="!isPublic" />
      <FormError :error="error" />
      <FormActions>
        <FormSubmit @click="create">
          {{ t('createAccount.create') }}
        </FormSubmit>
      </FormActions>
    </Form>
  </FlatDialog>
</template>

<script setup lang="ts">
import { Checkbox, Form, FormActions, FormError, FormSubmit, FormTitle, TextInput } from '@/components/forms/index';
import FlatDialog from '@/components/misc/FlatDialog.vue';
import { ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { GameNameValidator } from '@/core/services/validator/gameName';
import { useGameConfig } from '@/core/adapter/game';
import { useRouter } from 'vue-router';

const gameConfig = useGameConfig();
const nameValidator = new GameNameValidator();
const router = useRouter();

const { t } = useI18n();

const name = ref('');
const isPublic = ref(true);
const password = ref('');
const error = ref<string[]>([]);

watch([name, isPublic, password], () => {
  // clear error since there are changes
  error.value = [];
});

const create = async () => {
  error.value = [];

  try {
    await gameConfig.create(name.value, isPublic.value, password.value);

    router.push('/game/wait');
  } catch (e: unknown) {
    error.value = Array.isArray(e) ? e : [(e as Error).toString()];
  }
};
</script>
