<template>
  <div class="w-screen h-screen absolute p-3 overflow-hidden">
    <div class="w-full h-full relative">
      <div class="layout-grid">
        <InGameMenu />
        <div class="h-full">
          Main
          <CardDetail v-if="showCardDetail" />
        </div>
        <CardDeck />
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

const config = useConfig();
const showCardDetail = computed(() => config.showCardDetail);
</script>

<style>
.layout-grid {
  @apply grid h-full w-full;
  grid-template-rows: min-content auto min-content;
}
</style>
