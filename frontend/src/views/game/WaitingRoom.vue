<template>
  <div class="w-full h-full relative flex flex-col flex-nowrap overflow-y-auto py-2">
    <header class="mx-2 sticky top-0 left-0 right-0 z-10">
      <div class="w-full rounded-lg bg-darkest flex flex-nowrap flex-row px-2 py-1 justify-center items-center">
        <p class="text-3xl tc-main font-bold m-2 text-center" style="text-overflow: ellipsis; overflow: hidden">{{ gameConfig.name }}</p>
        <div class="space flex-1"></div>
        <div class="actions flex flex-row items-center justify-center m-2">
          <template v-if="isHost">
            <button class="tc-main-dark bg-primary hover:bg-primary-dark transition">
              <router-link to="/game/play">
                <!-- TODO: just temp -->
                {{ t('wait.start') }}
              </router-link>
            </button>
            <button class="tc-main-dark bg-secondary hover:bg-secondary-dark transition">{{ t('wait.stop') }}</button>
          </template>
          <template v-if="!isHost">
            <button @click="leave()" class="tc-main-dark bg-secondary hover:bg-secondary-dark transition">{{ t('wait.leave') }}</button>
          </template>
        </div>
      </div>
    </header>
    <main class="m-2 relative">
      <div class="main-content md:hidden grid grid-cols-1 gap-2 mx-auto max-w-5xl">
        <ChatWidget />
        <PlayersWidget />
        <SpectatorsWidget />
        <RulesWidget />
        <div v-if="isHost" class="bg-light" style="height: fit-content">Host section...</div>
      </div>
      <div class="main-content hidden md:grid grid-cols-2 gap-2 mx-auto max-w-5xl">
        <div class="grid grid-cols-1 gap-2" style="height: fit-content">
          <PlayersWidget />
          <SpectatorsWidget />
        </div>
        <div class="grid grid-cols-1 gap-2" style="height: fit-content">
          <ChatWidget />
          <RulesWidget />
          <div v-if="isHost" class="bg-light" style="height: fit-content">Host section...</div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import PlayersWidget from '@/components/waiting/widgets/PlayersWidget.vue';
import RulesWidget from '@/components/waiting/widgets/RulesWidget.vue';
import SpectatorsWidget from '@/components/waiting/widgets/SpectatorsWidget.vue';
import ChatWidget from '@/components/waiting/widgets/ChatWidget.vue';
import { useIsHost } from '@/composables/userRoles';
import { useGameConfig } from '@/core/adapter/game';
import { useLobbyStore } from '@/core/adapter/play/lobby';

const { t } = useI18n();
const gameConfig = useGameConfig();
const lobby = useLobbyStore();
const { isHost } = useIsHost();

const leave = () => {
  lobby.leave();
};
</script>

<style>
.actions button {
  @apply mx-1 px-2 py-1 rounded;
}

.qrcode-wrapper img {
  width: 100%;
}
</style>
