<template>
  <MaxWidthLayout size="small">
    <div class="w-full flex flex-row justify-between items-center sticky top-10 z-10 bg-bg">
      <h2 class="text-text text-4xl pt-3 pb-1">{{ t('list.title') }}</h2>
      <div class="flex flex-nowrap">
        <button class="scan-code btn-wrapper" @click="scanCode">
          <div class="icon-wrapper">
            <Icon icon="iconoir:scan-qr-code" class="icon text-2xl"></Icon>
          </div>
        </button>
        <button class="refresh btn-wrapper" @click="refresh">
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
      <div v-if="savedGame" class="item my-1 rounded-xl border border-border bg-surface px-3 py-2 cursor-default mb-3">
        <div class="flex justify-between align-center mb-2">
          <h3 class="text-text text-xl">{{ t('list.savedGame') }}</h3>
          <button @click="removeSavedGame()">
            <Icon class="text-lg text-text transition-transform hover:scale-110" icon="akar-icons:cross" />
          </button>
        </div>
        <div class="flex flex-row justify-between flex-wrap items-center">
          <div class="text text-text flex flex-row flex-nowrap justify-start items-center">
            <p class="text-md mr-2">
              {{ savedGame.name }}
            </p>
            <p
              v-if="!savedGame.isPublic"
              class="inline-flex align-baseline flex-row flex-nowrap items-center text-text-secondary text-sm italic mx-1"
            >
              <Icon icon="iconoir:lock-key" class="text-sm text-secondary-text mx-0.5" /><span>{{ t('list.private') }}</span>
            </p>
            <p class="text-text-secondary text-xs italic mx-1 whitespace-nowrap">({{ t('list.players', savedGame.playerCount) }})</p>
          </div>
          <div class="flex flex-1 flex-row flex-nowrap justify-end items-stretch">
            <div class="text-primary-text">
              <button
                class="flex flex-row flex-nowrap items-center h-full bg-alt hover:bg-alt-hover rounded py-1 px-2 border border-border"
                @click.stop="joinSavedGame()"
              >
                <span>{{ t('list.rejoin') }}</span> <Icon icon="iconoir:play-outline" class="text-lg" />
              </button>
            </div>
          </div>
        </div>
      </div>
      <!-- Games -->
      <div class="relative flex flex-col flex-nowrap">
        <div
          v-for="game of games"
          :key="game.id"
          class="item my-1 rounded-xl border border-border mouse:hover:bg-surface-hover mouse:hover:border-primary bg-surface px-3 py-2 cursor-pointer"
        >
          <router-link :to="'/join/' + game.id">
            <div class="flex flex-row justify-between flex-wrap items-center">
              <div class="text text-text flex flex-row flex-nowrap justify-start items-center">
                <p class="text-md mr-2">
                  {{ game.name }}
                </p>
                <p v-if="!game.isPublic" class="inline-flex align-baseline flex-row flex-nowrap items-center text-text-secondary text-sm italic mx-1">
                  <Icon icon="iconoir:lock-key" class="text-sm text-secondary-text mx-0.5" /><span>{{ t('list.private') }}</span>
                </p>
                <p class="text-text-secondary text-xs italic mx-1 whitespace-nowrap">({{ t('list.players', game.playerCount) }})</p>
              </div>
              <div class="flex flex-1 flex-row flex-nowrap justify-end items-stretch">
                <div v-tooltip="t('list.spectate')" class="mr-3">
                  <router-link
                    :to="'/join/' + game.id + '?spectate'"
                    class="flex flex-row flex-nowrap items-center h-full bg-alt hover:bg-alt-hover rounded py-1 px-2 border border-border"
                  >
                    <Icon icon="iconoir:eye-alt" class="text-xl text-text" />
                  </router-link>
                </div>
                <div class="text-primary-text">
                  <router-link
                    :to="'/join/' + game.id + '?play'"
                    class="flex flex-row flex-nowrap items-center h-full bg-alt hover:bg-alt-hover rounded py-1 px-2 border border-border"
                  >
                    <span>{{ t('list.play') }}</span> <Icon icon="iconoir:play-outline" class="text-lg" />
                  </router-link>
                </div>
              </div>
            </div>
          </router-link>
        </div>
        <div class="item my-1 rounded-xl bg-surface px-3 py-2 cursor-default">
          <div>
            <p class="text-center text-text my-1">
              {{ t(games.length === 0 ? 'list.nothingFound' : 'list.noFit') }}
            </p>
            <div class="flex flex-row justify-center">
              <button class="nothing-found-btn">
                <router-link to="/create-game" class="text-sm text-text block">{{ t('home.create') }}</router-link>
              </button>
              <button
                class="nothing-found-btn"
                :class="{ 'scale-95 opacity-50 pointer-events-none cursor-default': refreshing }"
                :disabled="refreshing"
                @click="refresh()"
              >
                <p class="text-sm text-text">
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
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';

import FloatingDialog from '@/components/misc/FloatingDialog.vue';
import { Icon } from '@/components/misc/Icon';
import QRCodeReader from '@/components/misc/QRCodeReader.vue';
import { SavedGame, useGameConfig } from '@/core/adapter/game';
import { GameMeta } from '@/core/api/entities/Game';
import { ZRPRole } from '@/core/domain/zrp/zrpTypes';
import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';

const { t } = useI18n();
const gameConfig = useGameConfig();
const router = useRouter();

const games = ref<GameMeta[]>([]);
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
  @apply transform transition-transform hover:scale-125 p-2 text-text;
}

.btn-wrapper {
  @apply rounded m-2 bg-alt hover:bg-alt-hover border border-border;
}

.nothing-found-btn {
  flex-basis: 100%;
  @apply rounded p-1 mx-2 my-1 bg-alt hover:bg-alt-hover border border-border;
}
</style>
