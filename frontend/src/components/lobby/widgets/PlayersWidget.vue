<template>
  <Widget v-model="isOpen" title="wait.players" widget-class="bg-light" button-class="bg-main hover:bg-dark">
    <template #actions>
      <div class="flex flex-row">
        <button @click="shareSheetOpen = true" class="share rounded m-1 bg-main hover:bg-dark tc-main-light">
          <div class="transform transition-transform hover:scale-110 p-1">
            <Icon icon="iconoir:share-android" class="icon text-2xl"></Icon>
          </div>
        </button>
        <button @click="qrCodeOpen = true" class="scan-code rounded m-1 mr-2 bg-main hover:bg-dark tc-main-light">
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
            <QRCode :data="`${Frontend.url}/join/${gameId}`" :width="256" :height="256" />
          </div>
        </FloatingDialog>
      </div>
    </template>
    <template #default>
      <div class="w-full flex flex-col">
        <div v-if="players.length === 0">
          <p class="tc-main-dark italic">{{ t('wait.noPlayers') }}</p>
        </div>
        <div
          v-for="player of players"
          :key="player.id"
          class="flex flex-nowrap justify-between items-center px-2 py-1 my-1 bg-dark border bc-darkest transition mouse:hover:bc-primary rounded-lg mouse:hover:bg-darkest"
        >
          <p class="text-lg tc-main-dark">
            <span :class="{ 'tc-primary': username === player.username }">
              {{ player.username }}
            </span>
            <span v-if="gameHost === player.username" class="tc-primary text-lg">
              <Icon icon="akar-icons:crown" class="inline ml-1" />
            </span>
          </p>
          <div class="flex items-center h-full justify-end">
            <template v-if="!isHost && username === player.username">
              <button v-tooltip="t('wait.spectate')" @click="handleChangeToSpectator()" class="tc-primary h-full bg-light hover:bg-main rounded p-1">
                <Icon icon="iconoir:eye-alt" />
              </button>
            </template>
            <template v-if="isHost && username !== player.username">
              <button
                v-tooltip="t('wait.spectate')"
                @click="handlePlayerToSpectator(player.id)"
                class="tc-primary h-full bg-light hover:bg-main rounded p-1 mr-2"
              >
                <Icon icon="iconoir:eye-alt" />
              </button>
              <button
                v-tooltip="t('wait.promote')"
                @click="askPromotePlayer(player.id)"
                class="tc-primary h-full bg-light hover:bg-main rounded p-1 mr-2"
              >
                <Icon icon="akar-icons:crown" />
              </button>
              <ReassureDialog
                @close="allowed => handlePromotePlayer(player.id, allowed)"
                :title="t('dialogs.promotePlayer.title', [player.username])"
                :body="t('dialogs.promotePlayer.body', [player.username])"
                :is-open="playerToPromote === player.id"
              />
              <button v-tooltip="t('wait.kick')" @click="askKickPlayer(player.id)" class="tc-secondary h-full bg-light hover:bg-main rounded p-1">
                <Icon icon="iconoir:delete-circled-outline" />
              </button>
              <ReassureDialog
                @close="allowed => handleKickPlayer(player.id, allowed)"
                :title="t('dialogs.kickPlayer.title', [player.username])"
                :body="t('dialogs.kickPlayer.body', [player.username])"
                :is-open="playerToKick === player.id"
              />
            </template>
          </div>
        </div>
      </div>
    </template>
  </Widget>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { computed, ref } from 'vue';
import { useI18n } from 'vue-i18n';

import ShareSheet from '@/components/lobby/ShareSheet.vue';
import FloatingDialog from '@/components/misc/FloatingDialog.vue';
import QRCode from '@/components/misc/QRCode.vue';
import ReassureDialog from '@/components/misc/ReassureDialog.vue';
import { useUserDefaults } from '@/composables/userDefaults';
import { useAuth } from '@/core/adapter/auth';
import { useGameConfig } from '@/core/adapter/game';
import { useLobbyStore } from '@/core/adapter/play/lobby';
import { useIsHost } from '@/core/adapter/play/util/userRoles';
import { Frontend } from '@/core/services/api/ApiConfig';

import Widget from '../Widget.vue';

const { t } = useI18n();
const isOpen = useUserDefaults('lobby:widgetPlayersOpen', true);
const lobby = useLobbyStore();
const gameConfig = useGameConfig();
const auth = useAuth();
const gameId = computed(() => gameConfig.gameId);
const { isHost } = useIsHost();
const username = computed(() => auth.username);
const gameHost = computed(() => lobby.host);
const playerToPromote = ref<string | undefined>(undefined);
const playerToKick = ref<string | undefined>(undefined);
const shareSheetOpen = ref(false);
const qrCodeOpen = ref(false);
const players = computed(() => lobby.players);

const handlePromotePlayer = (id: string, allowed: boolean) => {
  if (allowed) {
    lobby.promotePlayer(id);
  }
  playerToPromote.value = undefined;
};

const askPromotePlayer = (id: string) => {
  playerToPromote.value = id;
};

const handleKickPlayer = (id: string, allowed: boolean) => {
  if (allowed) {
    lobby.kickPlayer(id);
  }
  playerToKick.value = undefined;
};

const askKickPlayer = (id: string) => {
  playerToKick.value = id;
};

const handleChangeToSpectator = () => {
  lobby.changeToSpectator(username.value);
};

const handlePlayerToSpectator = (id: string) => {
  lobby.changeToSpectator(id);
};
</script>