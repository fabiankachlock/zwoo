<template>
  <div class="max-w-lg sm:w-full mx-auto h-full">
    <div class="mx-4 sm:mx-0 pb-2">
      <div class="w-full flex flex-row justify-between items-center sticky z-10 bg-main top-10">
        <h2 class="tc-main text-4xl mb-2 py-3">{{ t('join.join', [gameName]) }}</h2>
      </div>
      <div v-if="isLoading" class="flex flex-row justify-start flex-nowrap items-center tc-main">
        <Icon icon="iconoir:system-restart" class="text-xl tc-main-light animate-spin-slow mr-3" />
        <p class="text-xl tc-main">{{ t('util.loading') }}</p>
      </div>
      <div v-else class="flex flex-row flex-wrap items-center justify-center tc-main">
        <button
          @click="goBack()"
          class="action flex flex-row flex-nowrap items-center justify-center px-3 py-1 bg-dark hover:bg-darkest mx-2 my-1 rounded"
        >
          <Icon icon="iconoir:nav-arrow-left" class="icon text-xl mr-1 tc-secondary transform transition-transform" />{{ t('join.goBack') }}
        </button>
        <button
          @click="joinAsSpectator()"
          class="action flex flex-row flex-nowrap items-center justify-center px-3 py-1 bg-dark hover:bg-darkest mx-2 my-1 rounded"
        >
          <Icon icon="iconoir:eye-alt" class="icon text-xl mr-1 tc-secondary transform transition-transform" />
          <span class="whitespace-nowrap">{{ t('join.asSpectator') }}</span>
        </button>
        <button
          @click="joinAsPlayer()"
          class="action flex flex-row flex-nowrap items-center justify-center px-3 py-1 bg-dark hover:bg-darkest mx-2 my-1 rounded"
        >
          <Icon icon="iconoir:play-outline" class="icon text-xl mr-1 tc-secondary transform transition-transform" />
          <span class="whitespace-nowrap">{{ t('join.asPlayer') }}</span>
        </button>
      </div>
      <div>
        <Form>
          <FormError :error="error" />
        </Form>
      </div>
    </div>
  </div>
  <FloatingDialog v-if="showDialog" content-class="max-w-md">
    <Form show-close-button @close="showDialog = false">
      <FormTitle>
        {{ t('join.enterPassword') }}
      </FormTitle>
      <TextInput id="password" v-model="password" labelKey="join.password" is-password placeholder="******" />
      <FormError :error="error" />
      <FormActions>
        <FormSubmit @click="performJoinRequest">
          {{ t('join.enter') }}
        </FormSubmit>
      </FormActions>
    </Form>
  </FloatingDialog>
</template>

<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { Icon } from '@iconify/vue';
import FloatingDialog from '@/components/misc/FloatingDialog.vue';
import { Form, FormActions, FormError, FormSubmit, FormTitle, TextInput } from '@/components/forms/index';
import { onMounted, ref } from 'vue';
import { GameManagementService } from '@/core/services/api/GameManagement';
import { useGameConfig } from '@/core/adapter/game';
import { unwrapBackendError } from '@/core/services/api/errors';

const { t } = useI18n();
const route = useRoute();
const router = useRouter();
const isLoading = ref(true);
const gameConfig = useGameConfig();
const gameId = parseInt(route.params['id'] as string) ?? -1;
let asSpectator = route.query['spectate'] !== undefined;
let asPlayer = route.query['play'] !== undefined;

const gameName = ref('' + gameId);
const password = ref('');
const showDialog = ref(false);
let needsValidation = true;
const error = ref<string[]>([]);

const goBack = () => router.push('/available-games');

const performJoinRequest = async () => {
  error.value = [];
  isLoading.value = true;
  showDialog.value = false;

  try {
    await gameConfig.join(gameId, password.value, asPlayer, asSpectator);
    router.replace('/game/wait');
  } catch (e: unknown) {
    error.value = Array.isArray(e) ? e : [(e as Error).toString()];
    isLoading.value = false;
  }
};

const joinAsPlayer = () => {
  asPlayer = true;
  asSpectator = false;
  tryJoin();
};

const joinAsSpectator = () => {
  asPlayer = false;
  asSpectator = true;
  tryJoin();
};

const tryJoin = () => {
  console.log({ asPlayer, asSpectator, s: route.query['spectate'], p: route.query['play'] });
  if (!asSpectator && !asPlayer) {
    // no decision made
  } else if (!needsValidation) {
    // needs no password -> join direct
    performJoinRequest();
  } else {
    // ask for password
    showDialog.value = true;
  }
};

onMounted(async () => {
  const gameData = await GameManagementService.getJoinMeta(gameId);
  const [game] = unwrapBackendError(gameData);
  if (game) {
    isLoading.value = false;

    gameName.value = game.name;
    needsValidation = game.needsValidation;
    tryJoin();
  }
});
</script>

<style scoped>
.action:hover .icon {
  @apply scale-110;
}
</style>
