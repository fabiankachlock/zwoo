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
        <p class="text-lg tc-main-secondary" :class="{ 'tc-primary': publicId === player.id }">
          {{ player.username }}
        </p>
        <div class="flex items-center h-full justify-end">
          <template v-if="publicId === player.id">
            <button
              v-tooltip="t('wait.play')"
              @click="startPlaying()"
              class="tc-primary h-full flex flex-row flex-nowrap items-center justify-center p-1 bg-light mouse:hover:bg-main rounded"
            >
              <Icon icon="iconoir:play-outline" />
            </button>
          </template>
          <template v-if="isHost && publicId !== player.id">
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
  </Widget>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { useI18n } from 'vue-i18n';

import { Icon } from '@/components/misc/Icon';
import ReassureDialog from '@/components/misc/ReassureDialog.vue';
import { useUserDefaults } from '@/composables/userDefaults';
import { useAuth } from '@/core/adapter/auth';
import { useLobbyStore } from '@/core/adapter/game/lobby';
import { useIsHost } from '@/core/adapter/game/util/userRoles';

import Widget from '../Widget.vue';

const { t } = useI18n();
const isOpen = useUserDefaults('lobby:widgetSpectatorsOpen', true);
const lobby = useLobbyStore();
const spectators = computed(() => lobby.spectators);
const auth = useAuth();
const { isHost } = useIsHost();
const publicId = computed(() => auth.publicId);
const playerToKick = ref<string | undefined>(undefined);

const handleKickPlayer = (id: string, allowed: boolean) => {
  if (allowed) {
    lobby.kickPlayer(id);
  }
  playerToKick.value = undefined;
};

const askKickPlayer = (id: string) => {
  playerToKick.value = id;
};

const startPlaying = () => {
  lobby.changeToPlayer();
};
</script>
