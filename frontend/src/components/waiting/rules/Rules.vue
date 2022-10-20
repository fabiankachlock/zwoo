<template>
  <div class="w-full flex flex-col">
    <GameRule v-for="rule of displayedRules" :key="rule.id" :title="rule.title" :description="rule.description">
      <RuleContentDispatcher :rule="rule" />
    </GameRule>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { useRules } from '@/core/adapter/play/rules';

import GameRule from './GameRule.vue';
import RuleContentDispatcher from './RuleContentDispatcher.vue';

const rules = useRules();
const displayedRules = computed(() => rules.rules);
</script>
