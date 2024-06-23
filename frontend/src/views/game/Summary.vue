<template>
  <div class="game-summary overflow-y-auto">
    <template v-if="winner">
      <div class="sticky flex h-14 justify-between items-center flex-nowrap top-0 m-1 mt-0 bg-surface rounded-lg px-3 py-2 border border-primary">
        <div class="h-full flex-1 flex flex-nowrap items-center justify-start">
          <div class="logo w-10 h-10 mr-3" v-html="Logo"></div>
          <p class="text-text text-xl">{{ winner.username }}</p>
          <template v-if="winner.isBot">
            <span class="text-primary-text text-2xl ml-2">
              <Icon icon="fluent:bot-24-regular" />
            </span>
          </template>
          <p class="text-text text-xl ml-1">- {{ t('summary.winner') }}</p>
        </div>
        <p class="text-text text-xl italic">{{ winner.score }}</p>
      </div>
      <div
        v-for="player in notWinners"
        :key="player.username"
        class="player flex justify-start items-center flex-nowrap m-1 bg-surface rounded px-3 py-2 border border-border"
      >
        <p class="text-text">
          <span class="mr-2">{{ player.position }}.</span>
          {{ player.username }}
        </p>
        <template v-if="player.isBot">
          <span class="text-primary-text text-lg ml-2">
            <Icon icon="fluent:bot-24-regular" />
          </span>
        </template>
        <div class="flex-1"></div>
        <p class="text-text italic">{{ player.score }}</p>
      </div>
      <div class="bottom-spacer h-32"></div>
    </template>
    <div v-else class="flex flex-row justify-center flex-nowrap items-center text-text">
      <Icon icon="iconoir:system-restart" class="text-xl text-text animate-spin-slow mr-3" />
      <p class="text-xl text-text">{{ t('util.loading') }}</p>
    </div>
  </div>
  <div class="actions">
    <div class="actions-grid w-full grid gap-2">
      <button class="action bg-alt hover:bg-alt-hover border border-border" @click="handleLeaveClick()">
        <Icon class="icon text-primary-text" icon="mdi:logout-variant" />
        <p class="text text-text text-md">
          {{ t('summary.leave') }}
        </p>
      </button>
      <button class="action bg-alt hover:bg-alt-hover border border-border" @click="handlePlayClick()">
        <Icon class="icon text-warning-text" icon="mdi:logout-variant" />
        <p class="text text-text text-md">
          {{ t('summary.playAgain') }}
        </p>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';

import Logo from '@/assets/zwoo_logo_simple_none_auto.svg?raw';
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

.logo :deep(svg) {
  @apply w-full h-full;
}
</style>
