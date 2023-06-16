<template>
  <button
    v-if="isAllowed"
    class="end-turn-btn bg-main hover:bg-light border-2 rounded px-3 py-1 mr-5"
    :class="{
      hidden: !isEnabled,
      'bc-primary': state === 'active',
      'bc-secondary x-animate-pulse': isEnabled
    }"
    @click="handlePress"
  >
    <p class="text-lg tc-main whitespace-nowrap">{{ t('ingame.endTurn') }}</p>
  </button>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { useI18n } from 'vue-i18n';

import { useGameCardDeck } from '@/core/adapter/game/deck';
import { useGameEventDispatch } from '@/core/adapter/game/util/useGameEventDispatch';
import { useIsRuleActive } from '@/core/adapter/game/util/useIsRuleActive';
import { ZRPOPCode } from '@/core/domain/zrp/zrpTypes';
const { t } = useI18n();

const isAllowed = useIsRuleActive('explicitLastCard');
const deck = useGameCardDeck();
const isEnabled = computed(() => deck.cards.length === 1);
const sendEvent = useGameEventDispatch();

const handlePress = () => {
  sendEvent(ZRPOPCode.RequestEndTurn, {});
};

const state = ref<'hidden' | 'active' | 'pulse'>('hidden');
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
