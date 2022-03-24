<template>
  <div class="w-full h-full relative flex flex-col flex-nowrap overflow-y-scroll py-2">
    <header class="mx-2 sticky top-0 left-0 right-0 z-10">
      <div class="w-full rounded-lg bg-darkest flex flex-row px-2 py-1 justify-center flex-wrap items-center">
        <p class="text-3xl tc-main font-bold m-2 text-center">{{ gameConfig.name }}</p>
        <div class="space flex-1"></div>
        <div class="actions flex flex-row items-center justify-center m-2">
          <template v-if="isHost">
            <button class="tc-main-dark bg-primary hover:bg-primary-dark transition">
              <router-link to="/game/play">
                <!-- TODO: just temp -->
                {{ t('wait.start') }}
              </router-link>
            </button>
            <button class="tc-main-dark bg-secondary hover:bg-secondary-dark transition">{{ t('wait.stop') }}</button>
          </template>
          <template v-if="!isHost">
            <router-link to="/game/play">
              <button class="tc-main-dark bg-secondary hover:bg-secondary-dark transition">{{ t('wait.leave') }}</button>
            </router-link>
          </template>
        </div>
      </div>
    </header>
    <main class="m-2 relative">
      <div class="main-content grid gap-2 grid-cols-1 md:grid-cols-2 mx-auto max-w-5xl">
        <PlayersWidget />
        <RulesWidget />
        <SpectatorsWidget />
        <div v-if="isHost" class="bg-light md:col-start-2 md:row-start-2" style="height: fit-content">Host section...</div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { useGameConfig } from '@/core/adapter/game';
import { computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useLobbyStore } from '@/core/adapter/play/lobby';
import PlayersWidget from '@/components/waiting/widgets/PlayersWidget.vue';
import RulesWidget from '@/components/waiting/widgets/RulesWidget.vue';
import SpectatorsWidget from '@/components/waiting/widgets/SpectatorsWidget.vue';

const { t } = useI18n();
const lobby = useLobbyStore();
const gameConfig = useGameConfig();
const isHost = computed(() => gameConfig.host || true);

onMounted(() => {
  lobby.setup();
});
</script>

<style>
.actions button {
  @apply mx-1 px-2 py-1 rounded;
}

.qrcode-wrapper img {
  width: 100%;
}
</style>
