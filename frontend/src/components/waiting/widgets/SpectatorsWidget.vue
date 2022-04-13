<template>
  <Widget v-model="isOpen" title="wait.spectators" widget-class="bg-light" button-class="bg-main hover:bg-dark">
    <div class="w-full flex flex-col">
      <div v-if="spectators.length === 0">
        <p class="tc-main-dark italic">{{ t('wait.noSpectators') }}</p>
      </div>
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
import { useI18n } from 'vue-i18n';
import { useUserDefaults } from '@/composables/userDefaults';

const { t } = useI18n();
const isOpen = useUserDefaults('lobby:widgetSpectatorsOpen', true);
const lobby = useLobbyStore();
const spectators = computed(() => lobby.spectators);
</script>
