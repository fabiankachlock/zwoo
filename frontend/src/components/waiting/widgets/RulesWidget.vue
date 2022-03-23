<template>
  <Widget
    title="wait.rules"
    widget-class="bg-light md:col-start-2 md:row-start-1"
    :class="{ 'md:row-span-2': !isHost }"
    button-class="bg-main hover:bg-dark"
  >
    <Rules />
  </Widget>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useGameConfig } from '@/core/adapter/game';
import Widget from '../Widget.vue';
import Rules from '@/components/waiting/Rules.vue';

const gameConfig = useGameConfig();
const isHost = computed(() => gameConfig.host || true);
</script>
