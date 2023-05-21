<template>
  <div class="game-summary overflow-y-auto">
    <template v-if="winner">
      <div
        class="sticky flex h-14 justify-between items-center flex-nowrap top-0 m-1 mt-0 bg-dark mouse:hover:bg-darkest rounded-lg px-3 py-2 border bc-primary"
      >
        <div class="h-full flex-1 flex flex-nowrap items-center justify-start">
          <img src="/img/logo/zwoo_logo_simple_none.png" alt="" class="max-h-full mr-3" />
          <p class="tc-main text-xl">{{ winner.username }}</p>
          <template v-if="winner.id.startsWith('b_')">
            <span class="tc-primary text-2xl ml-2">
              <Icon icon="fluent:bot-24-regular" />
            </span>
          </template>
          <p class="tc-main text-xl ml-1">- {{ t('summary.winner') }}</p>
        </div>
        <p class="tc-main text-xl italic">{{ winner.score }}</p>
      </div>
      <div
        v-for="player in notWinners"
        :key="player.username"
        class="player flex justify-start items-center flex-nowrap m-1 bg-lightest mouse:hover:bg-light rounded px-3 py-2 border bc-dark"
      >
        <p class="tc-main-dark">
          <span class="mr-2">{{ player.position }}.</span>
          {{ player.username }}
        </p>
        <template v-if="player.id.startsWith('b_')">
          <span class="tc-primary text-lg ml-2">
            <Icon icon="fluent:bot-24-regular" />
          </span>
        </template>
        <div class="flex-1"></div>
        <p class="tc-main-dark italic">{{ player.score }}</p>
      </div>
      <div class="bottom-spacer h-32"></div>
    </template>
    <div v-else class="flex flex-row justify-center flex-nowrap items-center tc-main">
      <Icon icon="iconoir:system-restart" class="text-xl tc-main-light animate-spin-slow mr-3" />
      <p class="text-xl tc-main">{{ t('util.loading') }}</p>
    </div>
  </div>
  <div class="actions">
    <div class="actions-grid w-full grid gap-2">
      <button class="action bg-dark hover:bg-darkest" @click="handleLeaveClick()">
        <Icon class="icon tc-secondary" icon="mdi:logout-variant" />
        <p class="text tc-main text-md">
          {{ t('summary.leave') }}
        </p>
      </button>
      <button class="action bg-dark hover:bg-darkest" @click="handlePlayClick()">
        <Icon class="icon tc-secondary" icon="mdi:logout-variant" />
        <p class="text tc-main text-md">
          {{ t('summary.playAgain') }}
        </p>
      </button>

      <!-- TODO tmp(beta): might enabled later again <button :click="handleSpectatorClick()" class="action bg-dark hover:bg-darkest">
        <Icon class="icon tc-secondary" icon="iconoir:eye-alt" />
        <p class="text tc-main text-md">
          {{ t(isSpectator ? 'summary.spectateAgain' : 'summary.startSpectating') }}
        </p>
      </button>
      <button :click="handlePlayClick()" class="action bg-dark hover:bg-darkest">
        <Icon class="icon tc-secondary" icon="iconoir:play-outline" />
        <p class="text tc-main text-md">
          {{ t(isSpectator ? 'summary.startPlaying' : 'summary.playAgain') }}
        </p>
      </button> -->
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';

import { Icon } from '@/components/misc/Icon';
import { useGameSummary } from '@/core/adapter/game/summary';

const { t } = useI18n();
const summary = useGameSummary();
const players = computed(() => summary.summary);
const winner = computed(() => (players.value.length > 0 ? players.value[0] : undefined));
const notWinners = computed(() => [...players.value].slice(1) ?? []);

const handlePlayClick = () => {
  summary.playAgain();
};

const handleLeaveClick = () => {
  summary.leave();
};
</script>

<style scoped>
.game-summary {
  position: absolute;
  top: 10%;
  left: 3rem;
  right: 3rem;
  bottom: 0;
}

.actions {
  position: absolute;
  bottom: 5%;
  left: 10%;
  right: 10%;
}

.actions-grid {
  @apply grid-cols-1 grid-rows-2;
}

@media only screen and (min-width: 640px) {
  .actions-grid {
    @apply grid-cols-2 grid-rows-1;
  }
}

.action {
  @apply block rounded py-1 flex flex-nowrap justify-center items-center w-full max-w-full;
}

.icon {
  @apply text-xl transform ml-2 shrink-0 transition;
}

.text {
  @apply shrink mx-2 flex-1 min-w-0 overflow-hidden;
  text-overflow: ellipsis;
}

.action:hover .icon {
  @apply scale-110;
}

.player {
  @apply mx-3;
}

@media only screen and (min-width: 672px) {
  .player {
    @apply mx-8;
  }
}
</style>
