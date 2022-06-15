<template>
  <div class="w-full flex flex-col">
    <GameRule v-for="rule of displayedRules" :key="rule.id" :title="rule.title" :description="rule.description">
      <!-- <RuleSwitch :modelValue="rule.isActivated" @toggle="toggleRule(rule.id, $event)" /> -->
    </GameRule>
  </div>
</template>

<script setup lang="ts">
import { useRules } from '@/core/adapter/play/rules';
import { computed } from 'vue';
import GameRule from './GameRule.vue';
// import RuleSwitch from './RuleSwitch.vue';

const rules = useRules();
const displayedRules = computed(() => rules.rules);

// const toggleRule = (id: string, isActive: boolean): void => {
//   rules.updateRule(id, isActive);
// };
</script>
