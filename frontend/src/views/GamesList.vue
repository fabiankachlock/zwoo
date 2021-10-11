<template>
  <div class="max-w-lg sm:w-full mx-auto h-full">
    <div class="mx-4 sm:mx-0 pb-2">
      <div class="w-full flex flex-row justify-between items-center sticky z-10 bg-main top-10">
        <h2 class="tc-main text-4xl mb-2 py-3">{{ t('list.title') }}</h2>
        <div class="refresh rounded bg-lightest hover:bg-light tc-main-dark" @click="refresh">
          <div class="wrapper p-2">
            <Icon icon="iconoir:refresh" class="icon" :class="{ 'animate-spin': refreshing }"></Icon>
          </div>
        </div>
      </div>
      <div class="relative overflow-y-scroll h-1/4 max-h-full">
        <div class="relative flex flex-col flex-nowrap">
          <div
            v-for="game of games"
            :key="game.id"
            class="item my-1 rounded-xl border bc-lightest hover:bg-darkest hover:bc-primary bg-dark px-3 py-2 cursor-pointer"
          >
            <router-link :to="'/join/' + game.id">
              <p class="text tc-main-light">
                {{ game.name }}
                <span v-if="!game.isPublic" class="tc-main-secondary text-sm italic mx-3">{{ t('list.private') }}</span>
                <span class="tc-main-secondary text-sm italic mx-3 whitespace-nowrap">({{ t('list.players', game.playerCount) }})</span>
              </p>
            </router-link>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { GameManagementService, GamesList } from '@/core/services/api/GameManagement';
import { onMounted, ref } from 'vue';
import { Icon } from '@iconify/vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

const games = ref<GamesList>([]);
const refreshing = ref(false);

onMounted(async () => {
  games.value = await GameManagementService.listAll();
});

const refresh = async () => {
  games.value = await GameManagementService.listAll();
  refreshing.value = true;
  setTimeout(() => {
    refreshing.value = false;
  }, 1000);
};
</script>

<style scoped>
.refresh .wrapper {
  @apply transform transition-transform hover:scale-125;
}
</style>
