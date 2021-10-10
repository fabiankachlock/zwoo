<template>
  <div class="max-w-lg sm:w-full mx-auto h-full">
    <div class="mx-4 sm:mx-0 pb-2">
      <h2 class="tc-main text-4xl mb-2 py-3 sticky top-0 z-10 bg-main">{{ t('list.title') }}</h2>
      <div class="relative overflow-y-scroll h-1/4 max-h-full">
        <div class="relative flex flex-col flex-nowrap">
          <div
            v-for="game of games"
            :key="game.id"
            class="item my-1 rounded-xl border bc-lightest transition hover:bc-primary bg-dark px-3 py-2 cursor-pointer"
          >
            <p class="text tc-main-light">
              {{ game.name }}
              <span v-if="!game.isPublic" class="tc-main-secondary text-sm italic mx-3">{{ t('list.private') }}</span>
              <span class="tc-main-secondary text-sm italic mx-3">({{ t('list.players', game.playerCount) }})</span>
            </p>
          </div>
          <p class="my-32">A</p>
          <p class="my-32">A</p>
          <p class="my-32">A</p>
          <p class="my-32">A</p>
          <p class="my-32">A</p>
          <p class="my-32">A</p>
          <p class="my-32">A</p>
          <p class="my-32">A</p>
          <p class="my-32">A</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { GameManagementService, GamesList } from '@/core/services/api/GameManagement';
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

const games = ref<GamesList>([]);

onMounted(async () => {
  games.value = await GameManagementService.listAll();
});
</script>
