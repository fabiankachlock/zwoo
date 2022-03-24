<template>
  <Widget title="wait.spectators" widget-class="bg-light" button-class="bg-main hover:bg-dark" default-open>
    <div class="w-full flex flex-col">
      <div
        v-for="player of spectators"
        :key="player.id"
        class="flex flex-nowrap justify-between items-center px-2 py-1 my-1 bg-main border bc-dark transition hover:bc-primary rounded-lg hover:bg-dark"
      >
        <p class="text-lg tc-main-secondary">
          {{ player.name }}
        </p>
      </div>
    </div>
  </Widget>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useLobbyStore } from '@/core/adapter/play/lobby';
import Widget from '../Widget.vue';

const lobby = useLobbyStore();
const spectators = computed(() => lobby.spectators);
</script>
