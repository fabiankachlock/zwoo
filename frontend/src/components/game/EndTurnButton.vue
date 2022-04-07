<template>
  <button
    class="end-turn-btn bg-main hover:bg-light border-2 rounded px-3 py-1 mr-5"
    :class="{
      hidden: state === 'hidden',
      'bc-primary': state === 'active',
      'bc-secondary x-animate-pulse': state === 'pulse'
    }"
  >
    <p class="text-lg tc-main whitespace-nowrap">{{ t('ingame.endTurn') }}</p>
  </button>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';
const { t } = useI18n();
const state = ref<'hidden' | 'active' | 'pulse'>('pulse');
</script>

<style scoped>
.x-animate-pulse {
  animation: PulseBorder 1.2s ease-in-out infinite;
}

.x-animate-pulse:hover {
  opacity: 1;
  animation: none;
}

@keyframes PulseBorder {
  0%,
  100% {
    opacity: 1;
    transform: scale(1.06, 1.06);
  }
  50% {
    opacity: 0.8;
    transform: scale(1, 1);
  }
}
</style>
