<template>
  <MaxWidthLayout size="small">
    <div class="w-full flex flex-row justify-between items-center sticky z-10 bg-main top-10">
      <h2 class="tc-main text-4xl mb-2 py-3">{{ t('join.join', [gameName]) }}</h2>
    </div>
    <div v-if="isLoading" class="flex flex-row justify-start flex-nowrap items-center tc-main">
      <Icon icon="iconoir:system-restart" class="text-xl tc-main-light animate-spin-slow mr-3" />
      <p class="text-xl tc-main">{{ t('util.loading') }}</p>
    </div>
    <div v-else class="flex flex-row flex-wrap items-center justify-center tc-main">
      <button
        class="action flex flex-row flex-nowrap items-center justify-center px-3 py-1 bg-dark hover:bg-darkest mx-2 my-1 rounded"
        @click="goBack()"
      >
        <Icon icon="iconoir:nav-arrow-left" class="icon text-xl mr-1 tc-secondary transition-transform" />{{ t('join.goBack') }}
      </button>
      <button
        class="action flex flex-row flex-nowrap items-center justify-center px-3 py-1 bg-dark hover:bg-darkest mx-2 my-1 rounded"
        @click="joinAsSpectator()"
      >
        <Icon icon="iconoir:eye-alt" class="icon text-xl mr-1 tc-secondary transition-transform" />
        <span class="whitespace-nowrap">{{ t('join.asSpectator') }}</span>
      </button>
      <button
        class="action flex flex-row flex-nowrap items-center justify-center px-3 py-1 bg-dark hover:bg-darkest mx-2 my-1 rounded"
        @click="joinAsPlayer()"
      >
        <Icon icon="iconoir:play-outline" class="icon text-xl mr-1 tc-secondary transition-transform" />
        <span class="whitespace-nowrap">{{ t('join.asPlayer') }}</span>
      </button>
    </div>
    <div>
      <Form>
        <FormError :error="error" />
      </Form>
    </div>
  </MaxWidthLayout>
  <FloatingDialog v-if="showDialog" content-class="max-w-md">
    <Form show-close-button @close="showDialog = false">
      <FormTitle>
        {{ t('join.enterPassword') }}
      </FormTitle>
      <TextInput id="password" v-model="password" label-key="join.password" is-password placeholder="******" @keyup.enter="performJoinRequest" />
      <FormError :error="error" />
      <FormActions>
        <FormSubmit :disabled="isLoading" @click="performJoinRequest">
          {{ t('join.enter') }}
        </FormSubmit>
      </FormActions>
    </Form>
  </FloatingDialog>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute, useRouter } from 'vue-router';

import { Form, FormActions, FormError, FormSubmit, FormTitle, TextInput } from '@/components/forms/index';
import FloatingDialog from '@/components/misc/FloatingDialog.vue';
import { Icon } from '@/components/misc/Icon';
import { useGameConfig } from '@/core/adapter/game';
import { SnackBarPosition, useSnackbar } from '@/core/adapter/snackbar';
import { BackendError } from '@/core/api/ApiError';
import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';

const { t } = useI18n();
const route = useRoute();
const router = useRouter();
const snackbar = useSnackbar();
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
    snackbar.pushMessage({
      message: 'errors.zrp.loading',
      needsTranslation: true,
      showClose: false,
      position: SnackBarPosition.Top,
      mode: 'loading',
      onClosed() {
        router.replace('/game/wait');
      }
    });
    await gameConfig.join(gameId, password.value, asPlayer, asSpectator);
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
  const game = await gameConfig.getGameMeta(gameId);
  if (game) {
    isLoading.value = false;

    gameName.value = game.name;
    needsValidation = !game.isPublic;
    tryJoin();
  } else {
    isLoading.value = false;
    error.value = [t(`errors.backend.${BackendError.GameNotFound}`)];
  }
});
</script>

<style scoped>
.action:hover .icon {
  @apply scale-110;
}
</style>
