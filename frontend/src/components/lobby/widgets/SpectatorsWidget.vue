<template>
  <Widget v-model="isOpen" title="wait.spectators">
    <div class="w-full flex flex-col">
      <div v-if="spectators.length === 0">
        <p class="text-text-secondary">{{ t('wait.noSpectators') }}</p>
      </div>
      <div
        v-for="player of spectators"
        :key="player.id"
        class="flex flex-nowrap justify-between items-center px-2 py-1 my-1 bg-bg border border-border transition mouse:hover:border-primary rounded-lg"
      >
        <p class="text-lg text-text-secondary" :class="{ 'text-primary-text': lobbyId === player.id }">
          {{ player.username }}
        </p>
        <div class="flex items-center h-full justify-end">
          <template v-if="lobbyId === player.id">
            <button
              v-tooltip="t('wait.play')"
              class="text-primary-text h-full flex flex-row flex-nowrap items-center justify-center p-1 bg-alt hover:bg-alt-hover border border-border rounded"
              @click="startPlaying()"
            >
              <Icon icon="iconoir:play-outline" />
            </button>
          </template>
          <template v-if="isHost && lobbyId !== player.id">
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
    </div>
  </Widget>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { useI18n } from 'vue-i18n';

import { Icon } from '@/components/misc/Icon';
import ReassureDialog from '@/components/misc/ReassureDialog.vue';
import { useUserDefaults } from '@/composables/userDefaults';
import { useGameConfig } from '@/core/adapter/game';
import { useLobbyStore } from '@/core/adapter/game/lobby';
import { useIsHost } from '@/core/adapter/game/util/userRoles';

import Widget from '../Widget.vue';

const { t } = useI18n();
const isOpen = useUserDefaults('lobby:widgetSpectatorsOpen', true);
const lobby = useLobbyStore();
const gameConfig = useGameConfig();
const spectators = computed(() => lobby.spectators);
const { isHost } = useIsHost();
const lobbyId = computed(() => gameConfig.lobbyId);
const playerToKick = ref<number | undefined>(undefined);

const handleKickPlayer = (id: number, allowed: boolean) => {
  if (allowed) {
    lobby.kickPlayer(id);
  }
  playerToKick.value = undefined;
};

const askKickPlayer = (id: number) => {
  playerToKick.value = id;
};

const startPlaying = () => {
  lobby.changeToPlayer();
};
</script>
