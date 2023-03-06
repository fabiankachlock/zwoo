<template>
  <div id="main" class="w-screen absolute p-3 overflow-hidden">
    <div class="w-full h-full relative max-w-full max-h-full">
      <div class="layout-grid">
        <div class="z-10 min-w-0">
          <InGameMenu />
        </div>
        <div class="z-0 h-full min-w-0">
          <div class="relative layout-grid py-4">
            <Opponents />
            <div class="w-full h-full relative">
              <Pile />
              <MainCard />
            </div>
          </div>
        </div>
        <div class="relative z-0 min-w-0">
          <CardDeck v-if="!isSpectator" />
        </div>
        <CardDetail v-if="showCardDetail" />
        <template v-if="currentModal">
          <component :is="currentModal"></component>
        </template>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import CardDeck from '@/components/game/CardDeck.vue';
import CardDetail from '@/components/game/CardDetail.vue';
import InGameMenu from '@/components/game/InGameMenu.vue';
import MainCard from '@/components/game/MainCard.vue';
import Opponents from '@/components/game/OpponentsStrip.vue';
import Pile from '@/components/game/Pile.vue';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';
import { useGameModal } from '@/core/adapter/game/modal';
import { useIsSpectator } from '@/core/adapter/game/util/userRoles';

const config = useConfig();
const modalState = useGameModal();
const showCardDetail = computed(() => config.get(ZwooConfigKey.ShowCardsDetail));
const currentModal = computed(() => modalState.modalComponent);
const { isSpectator } = useIsSpectator();
</script>

<style scoped>
#main {
  height: calc(100vh - calc(100vh - 100%));
}

.layout-grid {
  @apply grid h-full w-full max-w-full;
  grid-template-rows: min-content auto min-content;
}
</style>
