<template>
  <Widget v-model="isOpen" title="wait.spectators" widget-class="bg-light" button-class="bg-main hover:bg-dark">
    <div class="w-full flex flex-col">
      <div v-if="spectators.length === 0">
        <p class="tc-main-dark italic">{{ t('wait.noSpectators') }}</p>
      </div>
      <div
        v-for="player of spectators"
        :key="player.id"
        class="flex flex-nowrap justify-between items-center px-2 py-1 my-1 bg-main border bc-dark transition mouse:hover:bc-primary rounded-lg mouse:hover:bg-dark"
      >
        <p class="text-lg tc-main-secondary" :class="{ 'tc-primary': lobbyId === player.id }">
          {{ player.username }}
        </p>
        <div class="flex items-center h-full justify-end">
          <template v-if="lobbyId === player.id">
            <button
              v-tooltip="t('wait.play')"
              class="tc-primary h-full flex flex-row flex-nowrap items-center justify-center p-1 bg-light mouse:hover:bg-main rounded"
              @click="startPlaying()"
            >
              <Icon icon="iconoir:play-outline" />
            </button>
          </template>
          <template v-if="isHost && lobbyId !== player.id">
            <button v-tooltip="t('wait.kick')" class="tc-secondary h-full bg-light hover:bg-main rounded p-1" @click="askKickPlayer(player.id)">
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
