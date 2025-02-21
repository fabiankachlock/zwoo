<script setup lang="ts">
import { computed, ref } from 'vue';
import { useI18n } from 'vue-i18n';

import ShareSheet from '@/components/lobby/ShareSheet.vue';
import Environment from '@/components/misc/Environment.vue';
import FloatingDialog from '@/components/misc/FloatingDialog.vue';
import { Icon } from '@/components/misc/Icon';
import QRCode from '@/components/misc/QRCode.vue';
import ReassureDialog from '@/components/misc/ReassureDialog.vue';
import { useIsOffline } from '@/composables/useEnvironment';
import { useUserDefaults } from '@/composables/userDefaults';
import { useGameConfig } from '@/core/adapter/game';
import { useLobbyStore } from '@/core/adapter/game/lobby';
import { useIsHost } from '@/core/adapter/game/util/userRoles';
import { useServerUrl } from '@/core/adapter/helper/useServerUrl';
import { ZRPRole } from '@/core/domain/zrp/zrpTypes';

import Widget from '../Widget.vue';

const { t } = useI18n();
const isOpen = useUserDefaults('lobby:widgetPlayersOpen', true);
const lobby = useLobbyStore();
const gameConfig = useGameConfig();
const joinUrl = computed(() => useServerUrl(gameConfig.gameId?.toString() ?? ''));
const { isHost } = useIsHost();
const lobbyId = computed(() => gameConfig.lobbyId);
const gameHost = computed(() => lobby.host);
const playerToPromote = ref<number | undefined>(undefined);
const playerToKick = ref<number | undefined>(undefined);
const shareSheetOpen = ref(false);
const winsInfoOpen = ref(false);
const qrCodeOpen = ref(false);
const players = computed(() => lobby.players);
const isOffline = useIsOffline();
const showWinsWarning = computed(() => players.value.filter(player => player.role !== ZRPRole.Bot).length <= 1 || isOffline.value);

const handlePromotePlayer = (id: number, allowed: boolean) => {
  if (allowed) {
    lobby.promotePlayer(id);
  }
  playerToPromote.value = undefined;
};

const askPromotePlayer = (id: number) => {
  playerToPromote.value = id;
};

const handleKickPlayer = (id: number, allowed: boolean) => {
  if (allowed) {
    lobby.kickPlayer(id);
  }
  playerToKick.value = undefined;
};

const askKickPlayer = (id: number) => {
  playerToKick.value = id;
};

const handleChangeToSpectator = () => {
  lobbyId.value && lobby.changeToSpectator(lobbyId.value);
};

const handlePlayerToSpectator = (id: number) => {
  lobby.changeToSpectator(id);
};
</script>

<template>
  <Widget v-model="isOpen" title="wait.players">
    <template #actions>
      <div class="flex flex-row">
        <Environment :exclude="['offline']">
          <button class="share rounded m-1 bg-alt hover:bg-alt-hover border border-border text-text" @click="shareSheetOpen = true">
            <div class="transform transition-transform hover:scale-110 p-1">
              <Icon icon="iconoir:share-android" class="icon text-2xl"></Icon>
            </div>
          </button>
          <button class="scan-code rounded m-1 mr-2 bg-alt hover:bg-alt-hover border border-border text-text" @click="qrCodeOpen = true">
            <div class="transform transition-transform hover:scale-110 p-1">
              <Icon icon="iconoir:scan-qr-code" class="icon text-2xl"></Icon>
            </div>
          </button>
        </Environment>
      </div>
      <div v-if="shareSheetOpen">
        <FloatingDialog>
          <div class="absolute top-2 right-2 z-10">
            <button class="bg-alt hover:bg-alt-hover border border-border p-1.5 text-text rounded" @click="shareSheetOpen = false">
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
            <button class="bg-alt hover:bg-alt-hover border border-border p-1.5 text-text rounded" @click="qrCodeOpen = false">
              <Icon icon="akar-icons:cross" class="text-xl" />
            </button>
          </div>
          <h3 class="text-xl text-text my-2">{{ t('wait.qrcode') }}</h3>
          <p class="mb-2 text-sm text-text-secondary">
            {{ t('wait.qrcodeInfo') }}
          </p>
          <div class="qrcode-wrapper">
            <QRCode :data="joinUrl" :width="256" :height="256" class="block mx-auto" image-class="h-[30vh] max-w-full" />
          </div>
        </FloatingDialog>
      </div>
    </template>
    <template #default>
      <div class="w-full flex flex-col">
        <div v-if="players.length === 0">
          <p class="text-text-secondary">{{ t('wait.noPlayers') }}</p>
        </div>
        <div
          v-for="player of players"
          :key="player.id"
          class="flex flex-nowrap justify-between items-center px-2 py-1 my-1 bg-bg border border-border transition mouse:hover:border-primary rounded-lg"
        >
          <div class="flex justify-start items-center">
            <p class="text-lg text-text">
              <span :class="{ 'text-primary-text': lobbyId === player.id }">
                {{ player.username }}
              </span>
            </p>
            <!-- display bot badge -->
            <template v-if="player.role === ZRPRole.Bot">
              <span class="text-primary-text text-lg ml-2">
                <Icon icon="fluent:bot-24-regular" />
              </span>
            </template>
            <!-- display host badge -->
            <template v-else-if="gameHost === player.id">
              <span class="text-primary-text text-lg ml-2">
                <Icon icon="akar-icons:crown" />
              </span>
            </template>
          </div>
          <div class="flex items-center h-full justify-end">
            <!-- display player actions for player -->
            <template v-if="!isHost && lobbyId === player.id && player.role !== ZRPRole.Bot">
              <button
                v-tooltip="t('wait.spectate')"
                class="text-primary-text h-full bg-alt hover:bg-alt-hover border border-border rounded p-1"
                @click="handleChangeToSpectator()"
              >
                <Icon icon="iconoir:eye-alt" />
              </button>
            </template>
            <!-- display player actions for host -->
            <template v-else-if="isHost && lobbyId !== player.id && player.role !== ZRPRole.Bot">
              <button
                v-tooltip="t('wait.spectate')"
                class="text-primary-text h-full bg-alt hover:bg-alt-hover border border-border rounded p-1 mr-2"
                @click="handlePlayerToSpectator(player.id)"
              >
                <Icon icon="iconoir:eye-alt" />
              </button>
              <button
                v-tooltip="t('wait.promote')"
                class="text-primary-text h-full bg-alt hover:bg-alt-hover border border-border rounded p-1 mr-2"
                @click="askPromotePlayer(player.id)"
              >
                <Icon icon="akar-icons:crown" />
              </button>
              <ReassureDialog
                :title="t('dialogs.promotePlayer.title', [player.username])"
                :body="t('dialogs.promotePlayer.body', [player.username])"
                :is-open="playerToPromote === player.id"
                @close="allowed => handlePromotePlayer(player.id, allowed)"
              />
              <button
                v-tooltip="t('wait.kick')"
                class="text-warning-text h-full bg-alt hover:bg-alt-hover border border-border rounded p-1"
                @click="askKickPlayer(player.id)"
              >
                <Icon icon="iconoir:delete-circled-outline" />
              </button>
              <ReassureDialog
                :title="t('dialogs.kickPlayer.title', [player.username])"
                :body="t('dialogs.kickPlayer.body', [player.username])"
                :is-open="playerToKick === player.id"
                @close="allowed => handleKickPlayer(player.id, allowed)"
              />
            </template>
          </div>
        </div>
        <!-- wins info section -->
        <div v-if="showWinsWarning" class="rounded-lg px-2 py-1 mt-4 bg-bg">
          <div class="flex justify-between items-center cursor-pointer" @click="winsInfoOpen = !winsInfoOpen">
            <p class="text-text">
              {{ t('wait.noWinInfo') }}
            </p>
            <Icon v-if="!winsInfoOpen" icon="akar-icons:info" class="text-text hover:text-primary-text cursor-pointer text-xl" />
            <Icon v-else icon="akar-icons:circle-chevron-up" class="text-text hover:text-primary-text cursor-pointer text-xl" />
          </div>
          <div class="grid overflow-hidden grid-rows-[0fr] transition-[grid-template-rows]" :class="{ 'grid-rows-[1fr]': winsInfoOpen }">
            <Environment show="online">
              <p class="min-h-0 text-text-secondary">
                {{ t('wait.noWinsOnlineInfo') }}
              </p>
            </Environment>
            <Environment show="offline">
              <p class="min-h-0 text-text-secondary">
                {{ t('wait.noWinsOfflineInfo') }}
              </p>
            </Environment>
          </div>
        </div>
      </div>
    </template>
  </Widget>
</template>
