<template>
  <div class="w-full h-full relative flex flex-col flex-nowrap overflow-y-scroll py-2">
    <header class="mx-2 sticky top-0 left-0 right-0 z-10">
      <div class="w-full rounded-lg bg-darkest flex flex-row px-2 py-1 justify-center flex-wrap items-center">
        <p class="text-3xl tc-main font-bold m-2 text-center">{{ gameConfig.name }}</p>
        <div class="space flex-1"></div>
        <div class="actions flex flex-row items-center justify-center m-2">
          <template v-if="isHost">
            <button class="tc-main-dark bg-primary hover:bg-primary-dark transition">{{ t('wait.start') }}</button>
            <button class="tc-main-dark bg-secondary hover:bg-secondary-dark transition">{{ t('wait.stop') }}</button>
          </template>
          <template v-if="!isHost">
            <button class="tc-main-dark bg-secondary hover:bg-secondary-dark transition">{{ t('wait.leave') }}</button>
          </template>
        </div>
      </div>
    </header>
    <main class="m-2 relative">
      <div class="main-content grid gap-2 grid-cols-1 md:grid-cols-2 mx-auto max-w-5xl">
        <div class="bg-lightest md:row-span-2">
          <div class="flex flex-nowrap flex-row justify-between items-center">
            <p class="text-xl tc-main my-2">{{ t('wait.players') }}</p>
            <div class="flex flex-row">
              <button @click="shareSheetOpen = true" class="scan-code rounded m-1 bg-main hover:bg-dark tc-main-light">
                <div class="transform transition-transform hover:scale-110 p-1">
                  <Icon icon="iconoir:share-android" class="icon text-2xl"></Icon>
                </div>
              </button>
              <button @click="qrCodeOpen = true" class="refresh rounded m-1 bg-main hover:bg-dark tc-main-light">
                <div class="transform transition-transform hover:scale-110 p-1">
                  <Icon icon="iconoir:scan-qr-code" class="icon text-2xl"></Icon>
                </div>
              </button>
            </div>
            <div v-if="shareSheetOpen">
              <FloatingDialog>
                <div class="absolute top-2 right-2 z-10">
                  <button @click="shareSheetOpen = false" class="bg-lightest hover:bg-light p-1.5 tc-main-dark rounded">
                    <Icon icon="akar-icons:cross" class="text-xl" />
                  </button>
                </div>
                <div class="relative">
                  <ShareSheet @should-close="shareSheetOpen = false" />
                </div>
              </FloatingDialog>
            </div>
            <div v-if="qrCodeOpen" class="share-qr-dialog">
              <FloatingDialog>
                <div class="absolute top-2 right-2 z-10">
                  <button @click="qrCodeOpen = false" class="bg-lightest hover:bg-light p-1.5 tc-main-dark rounded">
                    <Icon icon="akar-icons:cross" class="text-xl" />
                  </button>
                </div>
                <h3 class="text-xl tc-main my-2">{{ t('wait.qrcode') }}</h3>
                <p class="my-1 text-sm italic tc-main-secondary">
                  {{ t('wait.qrcodeInfo') }}
                </p>
                <div class="qrcode-wrapper mx-auto max-w-xs">
                  <QRCode :data="'https://zwoo-ui.web.app/join/' + gameId" :width="256" :height="256" />
                </div>
              </FloatingDialog>
            </div>
          </div>
          <div class="w-full flex flex-col">
            <div
              v-for="player of players"
              :key="player.id"
              class="flex flex-nowrap justify-between items-center px-2 py-1 my-1 bg-dark border bc-darkest transition hover:bc-primary rounded-lg hover:bg-darkest"
            >
              <p class="text-lg tc-main-dark">
                {{ player.name }}
              </p>
              <div class="flex items-center h-full justify-end">
                <button v-if="isHost" @click="askPromotePlayer()" class="tc-primary h-full bg-light hover:bg-main rounded p-1 mr-2">
                  <Icon icon="akar-icons:crown" />
                </button>
                <button v-if="isHost" @click="askKickPlayer()" class="tc-secondary h-full bg-light hover:bg-main rounded p-1">
                  <Icon icon="iconoir:delete-circled-outline" />
                </button>
                <ReassureDialog
                  @close="allowed => promotePlayer(player.id, allowed)"
                  :title="t('dialogs.promotePlayer.title', [player.name])"
                  :body="t('dialogs.promotePlayer.body', [player.name])"
                  :is-open="playerPromoteDialogOpen"
                />
                <ReassureDialog
                  @close="allowed => kickPlayer(player.id, allowed)"
                  :title="t('dialogs.kickPlayer.title', [player.name])"
                  :body="t('dialogs.kickPlayer.body', [player.name])"
                  :is-open="playerKickDialogOpen"
                />
              </div>
            </div>
          </div>
        </div>
        <div class="bg-lightest" :class="{ 'md:row-span-2': !isHost }">
          <Rules />
        </div>
        <div v-if="isHost" class="bg-lightest">Host section...</div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { useGameConfig } from '@/core/adapter/game';
import { computed, ref } from 'vue';
import QRCode from '@/components/misc/QRCode.vue';
import { useI18n } from 'vue-i18n';
import { ZRPOPCode } from '@/core/services/zrp/zrpTypes';
import Rules from '@/components/waiting/Rules.vue';
import { Icon } from '@iconify/vue';
import FloatingDialog from '@/components/misc/FloatingDialog.vue';
import ShareSheet from '@/components/waiting/ShareSheet.vue';
import ReassureDialog from '@/components/misc/ReassureDialog.vue';
import { useWatchGameEvents } from '@/core/adapter/play/util/gameEventWatcher';
import { createZRPOPCodeMatcher } from '@/core/adapter/play/util/zrpMatcher';

const { t } = useI18n();
const gameConfig = useGameConfig();
const isHost = computed(() => gameConfig.host || true);
const gameId = computed(() => gameConfig.gameId);
const qrCodeOpen = ref(false);
const playerPromoteDialogOpen = ref(false);
const playerKickDialogOpen = ref(false);
const shareSheetOpen = ref(false);

const promotePlayer = (id: string, allowed: boolean) => {
  if (allowed) {
    console.log('promoting', id);
  }
  playerPromoteDialogOpen.value = false;
};

const askPromotePlayer = () => {
  playerPromoteDialogOpen.value = true;
};

const kickPlayer = (id: string, allowed: boolean) => {
  if (allowed) {
    console.log('kick', id);
  }
  playerKickDialogOpen.value = false;
};

const askKickPlayer = () => {
  playerKickDialogOpen.value = true;
};

const players = ref([
  {
    name: 'player 1',
    id: '123'
  },
  {
    name: 'player 2',
    id: '234'
  }
]);

useWatchGameEvents<ZRPOPCode.PlayerJoined | ZRPOPCode.PlayerLeft>(createZRPOPCodeMatcher(ZRPOPCode.PlayerJoined, ZRPOPCode.PlayerLeft), msg => {
  if (msg.code === ZRPOPCode.PlayerJoined) {
    players.value.push({
      name: msg.data.name,
      id: msg.data.name
    });
  } else if (msg.code === ZRPOPCode.PlayerLeft) {
    players.value = players.value.filter(p => p.id !== msg.data.name);
  }
});
</script>

<style>
.actions button {
  @apply mx-1 px-2 py-1 rounded;
}

.main-content > div {
  @apply rounded-md p-3;
}

.qrcode-wrapper img {
  width: 100%;
}
</style>
