<template>
  <Widget v-model="isOpen" title="wait.spectators" widget-class="bg-light" button-class="bg-main hover:bg-dark">
    <div class="w-full flex flex-col">
      <div v-if="spectators.length === 0">
        <p class="tc-main-dark italic">{{ t('wait.noSpectators') }}</p>
      </div>
      <div
        v-for="player of spectators"
        :key="player.id"
        class="flex flex-nowrap justify-between items-center px-2 py-1 my-1 bg-main border bc-dark transition hover:bc-primary rounded-lg hover:bg-dark"
      >
        <p class="text-lg tc-main-secondary">
          {{ player.name }}
        </p>
        <div class="flex items-center h-full justify-end">
          <template v-if="username === player.name">
            <button
              v-tooltip="t('wait.play')"
              @click="startPlaying()"
              class="tc-primary h-full flex flex-row flex-nowrap items-center justify-center p-1 bg-light hover:bg-main rounded"
            >
              <Icon icon="iconoir:play-outline" />
            </button>
          </template>
          <template v-if="isHost && username !== player.name">
            <button v-tooltip="t('wait.kick')" @click="askKickPlayer()" class="tc-secondary h-full bg-light hover:bg-main rounded p-1">
              <Icon icon="iconoir:delete-circled-outline" />
            </button>
            <ReassureDialog
              @close="allowed => handleKickPlayer(player.id, allowed)"
              :title="t('dialogs.kickPlayer.title', [player.name])"
              :body="t('dialogs.kickPlayer.body', [player.name])"
              :is-open="playerKickDialogOpen"
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
import { Icon } from '@iconify/vue';
import { useLobbyStore } from '@/core/adapter/play/lobby';
import { useUserDefaults } from '@/composables/userDefaults';
import { useIsHost } from '@/composables/userRoles';
import { useAuth } from '@/core/adapter/auth';
import Widget from '../Widget.vue';
import ReassureDialog from '@/components/misc/ReassureDialog.vue';

const { t } = useI18n();
const isOpen = useUserDefaults('lobby:widgetSpectatorsOpen', true);
const lobby = useLobbyStore();
const spectators = computed(() => lobby.spectators);
const auth = useAuth();
const { isHost } = useIsHost();
const username = computed(() => auth.username);
const playerKickDialogOpen = ref(false);

const handleKickPlayer = (id: string, allowed: boolean) => {
  if (allowed) {
    lobby.kickPlayer(id);
  }
  playerKickDialogOpen.value = false;
};

const askKickPlayer = () => {
  playerKickDialogOpen.value = true;
};

const startPlaying = () => {
  lobby.changeToPlayer();
};
</script>
