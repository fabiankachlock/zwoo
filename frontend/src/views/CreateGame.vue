<template>
  <div class="w-full sm:max-w-xs mx-auto">
    <form class="bg-lightest shadow-md sm:rounded-sm px-6 py-4 mb-4 mt-8 relative">
      <router-link to="/" class="tc-main-secondary absolute left-3 top-3 text-xl transform transition-transform hover:-translate-x-1">
        <Icon icon="mdi:chevron-left" />
      </router-link>
      <h1 class="tc-main my-3 text-center text-3xl">{{ t('createGame.title') }}</h1>
      <div class="mb-4">
        <TextInput id="name" v-model="name" labelKey="createGame.name" :placeholder="t('createGame.name')" :validator="nameValidator" />
      </div>
      <div class="m-2 mb-4 flex no-wrap items-center">
        <label class="tc-main-secondary text-sm font-bold my-2">{{ t('createGame.isPublic') }}</label>
        <Checkbox styles="tc-primary mx-3" v-model="isPublic" />
      </div>
      <div class="mb-4" v-if="!isPublic">
        <TextInput id="password" v-model="password" labelKey="createGame.password" is-password placeholder="******" />
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
import { ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import TextInput from '../components/misc/TextInput.vue';
import Error from '../components/misc/Error.vue';
import { GameNameValidator } from '@/core/services/validator/gameName';
import Checkbox from '@/components/misc/Checkbox.vue';
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

    router.push('/game/room');
  } catch (e: any) {
    error.value = Array.isArray(e) ? e : [e.toString()];
  }
};
</script>
