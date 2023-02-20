<template>
  <div class="w-full h-full relative flex flex-col flex-nowrap overflow-y-auto py-2">
    <header class="mx-2 sticky top-0 left-0 right-0 z-10">
      <div class="w-full rounded-lg bg-darkest flex flex-nowrap flex-row px-2 py-1 justify-center items-center">
        <p class="text-3xl tc-main font-bold m-2 text-center" style="text-overflow: ellipsis; overflow: hidden">{{ gameConfig.name }}</p>
        <div class="space flex-1"></div>
        <div class="actions flex flex-row items-center justify-center m-2">
          <template v-if="isHost && playerCount > 1">
            <button @click="startGame()" class="tc-main-dark bg-primary hover:bg-primary-dark transition">
              {{ t('wait.start') }}
            </button>
          </template>
          <button @click="leave()" class="tc-main-dark bg-secondary hover:bg-secondary-dark transition">{{ t('wait.leave') }}</button>
        </div>
      </div>
    </header>
    <main class="m-2 relative">
      <div class="main-content md:hidden grid grid-cols-1 gap-2 mx-auto max-w-5xl">
        <ChatWidget />
        <PlayersWidget />
        <SpectatorsWidget />
        <RulesWidget />
        <BotsWidget />
      </div>
      <div class="main-content hidden md:grid grid-cols-2 gap-2 mx-auto max-w-5xl">
        <div class="grid grid-cols-1 gap-2" style="height: fit-content">
          <PlayersWidget />
          <SpectatorsWidget />
        </div>
        <div class="grid grid-cols-1 gap-2" style="height: fit-content">
          <ChatWidget />
          <RulesWidget />
          <BotsWidget />
        </div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';

import BotsWidget from '@/components/lobby/widgets/BotsWidget.vue';
import ChatWidget from '@/components/lobby/widgets/ChatWidget.vue';
import PlayersWidget from '@/components/lobby/widgets/PlayersWidget.vue';
import RulesWidget from '@/components/lobby/widgets/RulesWidget.vue';
import SpectatorsWidget from '@/components/lobby/widgets/SpectatorsWidget.vue';
import { useGameConfig } from '@/core/adapter/game';
import { useLobbyStore } from '@/core/adapter/play/lobby';
import { useIsHost } from '@/core/adapter/play/util/userRoles';

const { t } = useI18n();
const gameConfig = useGameConfig();
const lobby = useLobbyStore();
const { isHost } = useIsHost();
const playerCount = computed(() => lobby.players.length);

const leave = () => {
  lobby.leave();
};

const startGame = () => {
  lobby.startGame();
};
</script>

<style scoped>
.actions button {
  @apply mx-1 px-2 py-1 rounded;
}

.qrcode-wrapper img {
  width: 100%;
}
</style>
