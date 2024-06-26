<template>
  <MaxWidthLayout size="small">
    <div class="mx-4 sm:mx-0 pb-2 relative">
      <div class="sticky z-10 bg-bg top-10">
        <h2 class="text-text text-4xl py-2">{{ t('leaderboard.leaderboard') }}</h2>
      </div>
      <div class="relative flex flex-col flex-nowrap">
        <div v-if="playerEntries && playerEntries.length > 0">
          <div
            v-for="(player, index) in playerEntries"
            :key="index"
            class="item my-1 rounded-xl border border-border mouse:hover:border-primary bg-surface px-3 py-2"
          >
            <div class="flex flex-row justify-between flex-wrap items-center">
              <div class="text text-text flex flex-row flex-nowrap justify-start items-center">
                <p class="mr-2">{{ player.position }}. {{ player.name }}</p>
              </div>
              <div class="flex flex-1 flex-row flex-nowrap justify-end items-stretch">
                <p class="text-md text-text-secondary italic">
                  {{ t('leaderboard.wins', player.wins) }}
                </p>
              </div>
            </div>
          </div>
          <div v-if="isLoggedIn && ownPosition > 100 && wins >= 0">
            <div class="mb-2 -mt-1">
              <p class="ml-4 text-lg text-text">...</p>
            </div>
            <div class="item my-1 rounded-xl border border-border hover:border-primary bg-surface px-3 py-2">
              <div class="flex flex-row justify-between flex-wrap items-center">
                <div class="text text-text flex flex-row flex-nowrap justify-start items-center">
                  <p class="mr-2">{{ ownPosition }}. {{ username }}</p>
                </div>
                <div class="flex flex-1 flex-row flex-nowrap justify-end items-stretch">
                  <p class="text-md text-text-secondary italic">
                    {{ t('leaderboard.wins', wins) }}
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div v-else>
          <p class="text-text">
            {{ t('leaderboard.failure') }}
          </p>
        </div>
      </div>
    </div>
  </MaxWidthLayout>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

import { useAuth } from '@/core/adapter/auth';
import { LeaderBoardEntry, useLeaderBoard } from '@/core/adapter/controller/leaderboard';
import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';

const { t } = useI18n();
const leaderBoardStore = useLeaderBoard();
const authStore = useAuth();
const playerEntries = ref<LeaderBoardEntry[] | undefined>(undefined);
const ownPosition = ref<number>(-1);
const username = computed(() => authStore.username);
const wins = computed(() => authStore.wins);
const isLoggedIn = computed(() => authStore.isLoggedIn);

onMounted(async () => {
  playerEntries.value = (await leaderBoardStore.getLeaderBoard()) ?? [];

  if ((playerEntries.value ?? []).findIndex(player => player.name === authStore.username) < 0) {
    ownPosition.value = await leaderBoardStore.getOwnPosition();
  }
});
</script>
