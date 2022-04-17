<template>
  <div class="max-w-lg sm:w-full mx-auto">
    <div class="mx-4 sm:mx-0 pb-2 relative">
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
        <div class="relative flex flex-col flex-nowrap">
          <div
            v-for="game of games"
            :key="game.id"
            class="item my-1 rounded-xl border bc-darkest hover:bg-darkest hover:bc-primary bg-dark px-3 py-2 cursor-pointer"
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
    </div>
  </div>
</template>

<script setup lang="ts">
import { GameManagementService, GamesList } from '@/core/services/api/GameManagement';
import { onMounted, ref } from 'vue';
import { Icon } from '@iconify/vue';
import { useI18n } from 'vue-i18n';
import QRCodeReader from '@/components/misc/QRCodeReader.vue';
import FloatingDialog from '@/components/misc/FloatingDialog.vue';

const { t } = useI18n();

const games = ref<GamesList>([]);
const refreshing = ref(false);
const scanDialogOpen = ref(false);

onMounted(async () => {
  games.value = await GameManagementService.listAll();
});

const refresh = async () => {
  games.value = await GameManagementService.listAll();
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
