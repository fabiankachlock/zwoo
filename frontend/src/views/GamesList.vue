<template>
  <MaxWidthLayout size="small">
    <div class="w-full flex flex-row justify-between items-center sticky -top-1 z-10 bg-main">
      <h2 class="tc-main text-4xl mb-2 py-3">{{ t('list.title') }}</h2>
      <div class="flex flex-nowrap">
        <button class="scan-code btn-wrapper bg-lightest hover:bg-light tc-main-dark" @click="scanCode">
          <div class="icon-wrapper">
            <Icon icon="iconoir:scan-qr-code" class="icon text-2xl"></Icon>
          </div>
        </button>
        <button class="refresh btn-wrapper bg-lightest hover:bg-light tc-main-dark" @click="refresh">
          <div class="icon-wrapper">
            <Icon icon="iconoir:refresh" class="icon text-2xl" :class="{ 'animate-spin': refreshing }"></Icon>
          </div>
        </button>
      </div>
    </div>
    <div v-if="scanDialogOpen">
      <FloatingDialog @click-outside="onScanClose">
        <QRCodeReader @close="onScanClose" />
      </FloatingDialog>
    </div>
    <div class="relative pt-6">
      <!-- Saved game -->
      <div v-if="savedGame" class="item my-1 rounded-xl border bc-lightest hover:bg-darkest hover:bc-primary bg-dark px-3 py-2 cursor-default mb-3">
        <div class="flex justify-between align-center mb-2">
          <h3 class="tc-main text-xl">{{ t('list.savedGame') }}</h3>
          <button @click="removeSavedGame()">
            <Icon class="text-lg tc-main transition-transform hover:scale-110" icon="akar-icons:cross" />
          </button>
        </div>
        <router-link :to="'/join/' + savedGame.id">
          <div class="flex flex-row justify-between flex-wrap items-center">
            <div class="text tc-main-light flex flex-row flex-nowrap justify-start items-center">
              <p class="text-md mr-2">
                {{ savedGame.name }}
              </p>
              <p
                v-if="!savedGame.isPublic"
                class="inline-flex align-baseline flex-row flex-nowrap items-center tc-main-secondary text-sm italic mx-1"
              >
                <Icon icon="iconoir:lock-key" class="text-sm tc-secondary mx-0.5" /><span>{{ t('list.private') }}</span>
              </p>
              <p class="tc-main-secondary text-xs italic mx-1 whitespace-nowrap">({{ t('list.players', savedGame.playerCount) }})</p>
            </div>
            <div class="flex flex-1 flex-row flex-nowrap justify-end items-stretch">
              <div class="tc-primary">
                <button @click="joinSavedGame()" class="flex flex-row flex-nowrap items-center h-full bg-light hover:bg-main rounded py-1 px-2">
                  <span>{{ t('list.rejoin') }}</span> <Icon icon="iconoir:play-outline" class="text-lg" />
                </button>
              </div>
            </div>
          </div>
        </router-link>
      </div>
      <!-- Games -->
      <div class="relative flex flex-col flex-nowrap">
        <div
          v-for="game of games"
          :key="game.id"
          class="item my-1 rounded-xl border bc-darkest mouse:hover:bg-darkest mouse:hover:bc-primary bg-dark px-3 py-2 cursor-pointer"
        >
          <router-link :to="'/join/' + game.id">
            <div class="flex flex-row justify-between flex-wrap items-center">
              <div class="text tc-main-light flex flex-row flex-nowrap justify-start items-center">
                <p class="text-md mr-2">
                  {{ game.name }}
                </p>
                <p v-if="!game.isPublic" class="inline-flex align-baseline flex-row flex-nowrap items-center tc-main-secondary text-sm italic mx-1">
                  <Icon icon="iconoir:lock-key" class="text-sm tc-secondary mx-0.5" /><span>{{ t('list.private') }}</span>
                </p>
                <p class="tc-main-secondary text-xs italic mx-1 whitespace-nowrap">({{ t('list.players', game.playerCount) }})</p>
              </div>
              <div class="flex flex-1 flex-row flex-nowrap justify-end items-stretch">
                <div class="mr-3" v-tooltip="t('list.spectate')">
                  <router-link
                    :to="'/join/' + game.id + '?spectate'"
                    class="flex flex-row flex-nowrap items-center h-full bg-light hover:bg-main rounded py-1 px-2"
                  >
                    <Icon icon="iconoir:eye-alt" class="text-xl tc-main" />
                  </router-link>
                </div>
                <div class="tc-primary">
                  <router-link
                    :to="'/join/' + game.id + '?play'"
                    class="flex flex-row flex-nowrap items-center h-full bg-light hover:bg-main rounded py-1 px-2"
                  >
                    <span>{{ t('list.play') }}</span> <Icon icon="iconoir:play-outline" class="text-lg" />
                  </router-link>
                </div>
              </div>
            </div>
          </router-link>
        </div>
        <div
          v-if="games.length === 0"
          class="item my-1 rounded-xl border bc-lightest hover:bg-darkest hover:bc-primary bg-dark px-3 py-2 cursor-default"
        >
          <div>
            <p class="text-center tc-main my-1">
              {{ t('list.nothingFound') }}
            </p>
            <div class="flex flex-row justify-center">
              <button class="nothing-found-btn bg-main hover:bg-light">
                <router-link to="/create-game" class="text-sm tc-main-secondary block">{{ t('home.create') }}</router-link>
              </button>
              <button
                @click="refresh()"
                class="nothing-found-btn bg-main hover:bg-light transition-transform"
                :class="{ 'scale-95 opacity-50 pointer-events-none cursor-default': refreshing }"
                :disabled="refreshing"
              >
                <p class="text-sm tc-main-secondary">
                  {{ t('list.reload') }}
                </p>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </MaxWidthLayout>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';

import FloatingDialog from '@/components/misc/FloatingDialog.vue';
import QRCodeReader from '@/components/misc/QRCodeReader.vue';
import { SavedGame, useGameConfig } from '@/core/adapter/game';
import { GameMeta, GamesList } from '@/core/services/api/GameManagement';
import { ZRPRole } from '@/core/services/zrp/zrpTypes';
import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';

const { t } = useI18n();
const gameConfig = useGameConfig();
const router = useRouter();

const games = ref<GamesList>([]);
const refreshing = ref(false);
const scanDialogOpen = ref(false);
const savedGame = ref<(GameMeta & SavedGame) | undefined>(undefined);

onMounted(async () => {
  games.value = await gameConfig.listGames();
  savedGame.value = await gameConfig.tryRestoreStoredConfig();
});

const refresh = async () => {
  games.value = await gameConfig.listGames();
  savedGame.value = await gameConfig.tryRestoreStoredConfig();
  refreshing.value = true;
  setTimeout(() => {
    refreshing.value = false;
  }, 1000);
};

const scanCode = () => {
  scanDialogOpen.value = true;
};

const onScanClose = () => {
  scanDialogOpen.value = false;
};

const joinSavedGame = () => {
  if (!savedGame.value) return;
  if (savedGame.value.role !== ZRPRole.Spectator) {
    // play
    router.push(`/join/${savedGame.value.id}?play`);
  } else {
    // spectate
    router.push(`/join/${savedGame.value.id}?spectate`);
  }
};

const removeSavedGame = () => {
  gameConfig.clearStoredConfig();
  savedGame.value = undefined;
};
</script>

<style scoped>
.icon-wrapper {
  @apply transform transition-transform hover:scale-125 p-2;
}

.btn-wrapper {
  @apply rounded m-2;
}

.nothing-found-btn {
  flex-basis: 100%;
  @apply rounded p-1 mx-2 my-1;
}
</style>
