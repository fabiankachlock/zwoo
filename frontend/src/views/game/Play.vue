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
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import InGameMenu from '@/components/game/InGameMenu.vue';
import CardDeck from '@/components/game/CardDeck.vue';
import CardDetail from '@/components/game/CardDetail.vue';
import { useConfig } from '@/core/adapter/config';
import { computed } from 'vue';
import Opponents from '@/components/game/OpponentsStrip.vue';
import { useIsSpectator } from '@/composables/userRoles';
import Pile from '@/components/game/Pile.vue';
import MainCard from '@/components/game/chat/MainCard.vue';

const config = useConfig();
const showCardDetail = computed(() => config.showCardDetail);
const { isSpectator } = useIsSpectator();
</script>

<style>
#main {
  height: calc(100vh - calc(100vh - 100%));
}

.layout-grid {
  @apply grid h-full w-full max-w-full;
  grid-template-rows: min-content auto min-content;
}
</style>
