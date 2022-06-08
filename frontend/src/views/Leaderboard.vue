<template>
  <div class="max-w-lg sm:w-full mx-auto">
    <div class="mx-4 sm:mx-0 pb-2 relative">
      <div class="w-full flex flex-row justify-between items-center sticky z-10 bg-main -top-1">
        <h2 class="tc-main text-4xl mb-2 py-3">{{ t('leaderboard.leaderboard') }}</h2>
      </div>
      <div class="relative flex flex-col flex-nowrap">
        <div v-if="playerEntries">
          <div
            v-for="(player, index) in playerEntries"
            :key="index"
            class="item my-1 rounded-xl border bc-darkest hover:bg-darkest hover:bc-primary bg-dark px-3 py-2 cursor-pointer"
          >
            <div class="flex flex-row justify-between flex-wrap items-center">
              <div class="text tc-main-light flex flex-row flex-nowrap justify-start items-center">
                <p class="mr-2">{{ index + 1 }}. {{ player.username }}</p>
              </div>
              <div class="flex flex-1 flex-row flex-nowrap justify-end items-stretch">
                <p class="text-md tc-main-secondary italic">
                  {{ t('leaderboard.wins', player.wins) }}
                </p>
              </div>
            </div>
          </div>
        </div>
        <div v-else>
          <p class="tc-main">
            {{ t('leaderboard.failure') }}
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { LeaderBoardResponse, LeaderBoardService } from '@/core/services/api/LeaderBoard';
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
const playerEntries = ref<LeaderBoardResponse['leaderboard'][number][] | undefined>(undefined);

onMounted(async () => {
  const response = await LeaderBoardService.fetchLeaderBoards();
  if (response && 'leaderboard' in response) {
    playerEntries.value = response.leaderboard;
  } else {
    playerEntries.value = undefined;
  }
});
</script>
