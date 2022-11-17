<template>
  <div class="bg-darkest rounded-lg p-1 flex flex-row justify-between items-center">
    <p class="ml-2 tc-main text-xl" style="text-overflow: ellipsis; overflow: hidden">{{ gameName }}</p>
    <div class="flex-1 flex flex-row flex-nowrap justify-end items-center">
      <EndTurnButton />
      <div class="relative">
        <div @click="toggleMenu()" class="bg-main hover:bg-light p-2 rounded tc-main text-2xl text-right cursor-pointer">
          <Icon v-if="menuOpen" icon="akar-icons:cross"></Icon>
          <Icon v-else icon="akar-icons:chevron-down"></Icon>
        </div>
        <div
          :class="{ 'open-menu': menuOpen, 'h-0': !menuOpen }"
          class="fixed left-0 right-0 w-screen transition-all overflow-hidden px-6 flex flex-nowrap justify-end"
        >
          <div class="relative bg-darkest w-full max-w-sm h-full menu-rounded-edges overflow-hidden" style="max-height: 70vh">
            <div class="h-1.5"></div>
            <div class="menu-rounded-edges menu-content border bc-invert-main border-t-0 py-1">
              <div class="scroll-container overflow-y-auto h-full max-h-full">
                <div class="flex flex-col flex-nowrap">
                  <div class="menu-section">
                    <p class="tc-main text-lg">{{ t('ingame.options') }}</p>
                    <div class="menu-options-container flex flex-row justify-end">
                      <DarkModeSwitch />
                      <FullScreenSwitch />
                    </div>
                  </div>
                  <hr class="bc-invert-lightest opacity-40 my-1" />
                  <div class="menu-section">
                    <p class="tc-main text-lg">{{ t('ingame.actions') }}</p>
                    <div>
                      <button @click="handleLeave" class="tc-main-dark bg-secondary hover:bg-secondary-dark mx-1 px-2 py-1 rounded">
                        {{ t('ingame.leave') }}
                      </button>
                    </div>
                  </div>
                  <hr class="bc-invert-lightest opacity-40 my-1" />
                  <GameChat />
                  <hr class="bc-invert-lightest opacity-40 my-1" />
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
import DarkModeSwitch from '@/components/settings/DarkModeSwitch.vue';
import FullScreenSwitch from '@/components/settings/FullScreenSwitch.vue';
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
  height: 19rem;
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
  @apply flex justify-between items-center m-2;
}
</style>
