<template>
  <div class="max-w-lg sm:w-full mx-auto h-full">
    <div class="mx-4 sm:mx-0 pb-2">
      <div class="w-full flex flex-row justify-between items-center sticky z-10 bg-main top-10">
        <h2 class="tc-main text-4xl mb-2 py-3">{{ t('list.title') }}</h2>
        <div class="flex flex-nowrap">
          <div class="scan-code btn-wrapper bg-lightest hover:bg-light tc-main-dark" @click="scanCode">
            <div class="icon-wrapper">
              <Icon icon="iconoir:scan-qr-code" class="icon text-2xl"></Icon>
            </div>
          </div>
          <div class="refresh btn-wrapper bg-lightest hover:bg-light tc-main-dark" @click="refresh">
            <div class="icon-wrapper">
              <Icon icon="iconoir:refresh" class="icon text-2xl" :class="{ 'animate-spin': refreshing }"></Icon>
            </div>
          </div>
        </div>
      </div>
      <div v-if="scanDialogOpen">
        <FloatingDialog @click-outside="onScanClose">
          <QRCodeReader @close="onScanClose" />
        </FloatingDialog>
      </div>
      <div class="relative overflow-y-scroll h-1/4 max-h-full">
        <div class="relative flex flex-col flex-nowrap">
          <div
            v-for="game of games"
            :key="game.id"
            class="item my-1 rounded-xl border bc-lightest hover:bg-darkest hover:bc-primary bg-dark px-3 py-2 cursor-pointer"
          >
            <router-link :to="'/join/' + game.id">
              <p class="text tc-main-light">
                {{ game.name }}
                <span v-if="!game.isPublic" class="tc-main-secondary text-sm italic mx-3">{{ t('list.private') }}</span>
                <span class="tc-main-secondary text-sm italic mx-3 whitespace-nowrap">({{ t('list.players', game.playerCount) }})</span>
              </p>
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
                  class="nothing-found-btn bg-main hover:bg-light transform transition-transform"
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

