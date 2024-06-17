<template>
  <div class="bg-darkest rounded-lg p-1 flex flex-row justify-between items-center">
    <p class="ml-2 text-text text-xl" style="text-overflow: ellipsis; overflow: hidden">{{ gameName }}</p>
    <div class="flex-1 flex flex-row flex-nowrap justify-end items-center">
      <EndTurnButton />
      <div class="relative">
        <div class="bg-bg hover:bg-bg p-2 rounded text-text text-2xl text-right cursor-pointer" @click="toggleMenu()">
          <Icon v-if="menuOpen" icon="akar-icons:cross"></Icon>
          <Icon v-else icon="akar-icons:chevron-down"></Icon>
        </div>
        <div
          :class="{ 'open-menu': menuOpen, 'h-0': !menuOpen }"
          class="fixed left-0 right-0 w-screen transition-all overflow-hidden px-6 flex flex-nowrap justify-end"
        >
          <div class="relative bg-darkest w-full max-w-sm h-full menu-rounded-edges overflow-hidden" style="max-height: 70vh">
            <div class="h-1.5"></div>
            <div class="menu-rounded-edges menu-content border border-border-divider border-t-0 py-1">
              <div class="scroll-container overflow-y-auto h-full max-h-full">
                <div class="flex flex-col flex-nowrap">
                  <div class="menu-section items-start">
                    <p class="text-text text-lg">{{ t('ingame.options') }}</p>
                    <div class="menu-options-container flex flex-row justify-end">
                      <div>
                        <div class="mb-1">
                          <DarkModeSwitch class="mr-1" />
                          <FullScreenSwitch />
                        </div>
                        <div>
                          <SortCardsSwitch class="mr-1" />
                          <ShowCardDetailSwitch />
                        </div>
                      </div>
                    </div>
                  </div>
                  <hr class="border-border-divider opacity-40 my-1" />
                  <div class="menu-section items-center">
                    <p class="text-text text-lg">{{ t('ingame.actions') }}</p>
                    <div>
                      <button class="text-text-dark bg-secondary hover:bg-secondary-dark mx-1 px-2 py-1 rounded" @click="handleLeave">
                        {{ t('ingame.leave') }}
                      </button>
                    </div>
                  </div>
                  <hr class="border-border-divider opacity-40 my-1" />
                  <GameChat />
                  <hr class="border-border-divider opacity-40 my-1" />
                  <ChatInput />
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { useI18n } from 'vue-i18n';

import { Icon } from '@/components/misc/Icon';
import ShowCardDetailSwitch from '@/components/settings/sections/game/ShowCardDetailSwitch.vue';
import SortCardsSwitch from '@/components/settings/sections/game/SortCardsSwitch.vue';
import DarkModeSwitch from '@/components/settings/sections/general/DarkModeSwitch.vue';
import FullScreenSwitch from '@/components/settings/sections/general/FullScreenSwitch.vue';
import { useGameConfig } from '@/core/adapter/game';

import ChatInput from './chat/ChatInput.vue';
import GameChat from './chat/GameChat.vue';
import EndTurnButton from './EndTurnButton.vue';

const game = useGameConfig();
const { t } = useI18n();
const menuOpen = ref(false);
const gameName = computed(() => game.name);

const toggleMenu = () => {
  menuOpen.value = !menuOpen.value;
};

const handleLeave = () => {
  game.leave();
};
</script>

<style scoped>
.open-menu {
  height: 21rem;
}

.menu-rounded-edges {
  border-radius: 0 0 1rem 1rem;
}

.menu-content {
  height: calc(100% - 0.375rem - 1px); /* - h-1.5 - border-bottom*/
}

.menu-options-container > * {
  @apply mx-1;
}

.menu-section {
  @apply flex justify-between m-2;
}
</style>
