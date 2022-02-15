<template>
  <div class="bg-darkest rounded-lg py-1 px-3 flex flex-row justify-between items-center">
    <p class="tc-main text-xl">#GameName#</p>
    <div class="relative">
      <div @click="menuOpen = !menuOpen" class="tc-main text-2xl text-right cursor-pointer">
        <Icon v-if="menuOpen" icon="akar-icons:cross"></Icon>
        <Icon v-else icon="akar-icons:chevron-down"></Icon>
      </div>
      <div
        :class="{ 'h-64': menuOpen, 'h-0': !menuOpen }"
        class="fixed left-0 right-0 w-screen transition-all overflow-hidden px-6 flex flex-nowrap justify-end"
      >
        <div class="relative bg-darkest w-full max-w-sm h-full menu-rounded-edges overflow-hidden" style="max-height: 70vh">
          <div class="h-1.5"></div>
          <div class="menu-rounded-edges menu-content border bc-invert-main border-t-0 py-1">
            <div class="scroll-container overflow-y-scroll h-full max-h-full">
              <div class="flex flex-col flex-nowrap">
                <div class="menu-section">
                  <p class="tc-main text-lg">#Options#</p>
                  <div class="menu-options-container flex flex-row justify-end">
                    <DarkModeSwitch />
                    <FullScreenSwitch />
                  </div>
                </div>
                <hr class="bc-invert-lightest opacity-40 my-1" />
                <div class="menu-section">
                  <p class="tc-main text-lg">#Actions#</p>
                  <div>
                    <button @click="handleLeave" class="tc-main-dark bg-secondary hover:bg-secondary-dark mx-1 px-2 py-1 rounded">#Leave#</button>
                  </div>
                </div>
                <hr class="bc-invert-lightest opacity-40 my-1" />
                <GameChat />
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { ref } from 'vue';
import DarkModeSwitch from '@/components/settings/DarkModeSwitch.vue';
import FullScreenSwitch from '@/components/settings/FullScreenSwitch.vue';
import { useGameState } from '@/core/adapter/play/gameState';
import { useGameCardDeck } from '@/core/adapter/play/deck';
import GameChat from './chat/GameChat.vue';

const stateStore = useGameState();
const deckState = useGameCardDeck();
const menuOpen = ref(false);

const handleLeave = () => {
  // TODO: Just Temp
  stateStore.setIsActive(!stateStore.isActivePlayer);
  stateStore.update();
  deckState.addCard({
    id: deckState.cards.length.toString()
  });
};
</script>

<style>
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
