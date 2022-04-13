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
          class="flex flex-nowrap justify-between items-center px-2 py-1 my-1 bg-dark border bc-darkest transition hover:bc-primary rounded-lg hover:bg-darkest"
        >
          <p class="text-lg tc-main-dark">
            {{ player.name }}
            <span v-if="gameHost === player.name" class="tc-primary text-lg">
              <Icon icon="akar-icons:crown" class="inline ml-1" />
            </span>
          </p>
          <div v-if="isHost && username !== player.name" class="flex items-center h-full justify-end">
            <button v-tooltip="t('wait.promote')" @click="askPromotePlayer()" class="tc-primary h-full bg-light hover:bg-main rounded p-1 mr-2">
              <Icon icon="akar-icons:crown" />
            </button>
            <button v-tooltip="t('wait.kick')" @click="askKickPlayer()" class="tc-secondary h-full bg-light hover:bg-main rounded p-1">
              <Icon icon="iconoir:delete-circled-outline" />
            </button>
            <ReassureDialog
              @close="allowed => handlePromotePlayer(player.id, allowed)"
              :title="t('dialogs.promotePlayer.title', [player.name])"
              :body="t('dialogs.promotePlayer.body', [player.name])"
              :is-open="playerPromoteDialogOpen"
            />
            <ReassureDialog
              @close="allowed => handleKickPlayer(player.id, allowed)"
              :title="t('dialogs.kickPlayer.title', [player.name])"
              :body="t('dialogs.kickPlayer.body', [player.name])"
              :is-open="playerKickDialogOpen"
            />
          </div>
        </div>
      </div>
    </template>
  </Widget>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { Icon } from '@iconify/vue';
import { useLobbyStore } from '@/core/adapter/play/lobby';
import { useGameConfig } from '@/core/adapter/game';
import { useI18n } from 'vue-i18n';
import { Frontend } from '@/core/services/api/apiConfig';
import Widget from '../Widget.vue';
import FloatingDialog from '@/components/misc/FloatingDialog.vue';
import ShareSheet from '@/components/waiting/ShareSheet.vue';
import ReassureDialog from '@/components/misc/ReassureDialog.vue';
import QRCode from '@/components/misc/QRCode.vue';
import { useUserDefaults } from '@/composables/userDefaults';
import { useIsHost } from '@/composables/userRoles';
import { useAuth } from '@/core/adapter/auth';

const { t } = useI18n();
const isOpen = useUserDefaults('lobby:widgetPlayersOpen', true);
const lobby = useLobbyStore();
const gameConfig = useGameConfig();
const auth = useAuth();
const gameId = computed(() => gameConfig.gameId);
const { isHost } = useIsHost();
const username = computed(() => auth.username);
const gameHost = computed(() => lobby.host);
const playerPromoteDialogOpen = ref(false);
const playerKickDialogOpen = ref(false);
const shareSheetOpen = ref(false);
const qrCodeOpen = ref(false);
const players = computed(() => lobby.players);

const handlePromotePlayer = (id: string, allowed: boolean) => {
  if (allowed) {
    lobby.promotePlayer(id);
  }
  playerPromoteDialogOpen.value = false;
};

const askPromotePlayer = () => {
  playerPromoteDialogOpen.value = true;
};

const handleKickPlayer = (id: string, allowed: boolean) => {
  if (allowed) {
    lobby.kickPlayer(id);
  }
  playerKickDialogOpen.value = false;
};

const askKickPlayer = () => {
  playerKickDialogOpen.value = true;
};
</script>
