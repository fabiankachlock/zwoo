<template>
  <div class="w-full flex flex-col">
    <GameRule v-for="rule of displayedRules" :key="rule.id" :title="rule.title" :description="rule.description">
      <RuleContentDispatcher :rule="rule" />
      <template #subrules>
        <SubRule v-for="subrule of rule.children" :key="subrule.id" :title="subrule.title" :description="subrule.description">
          <RuleContentDispatcher :rule="subrule" />
        </SubRule>
      </template>
    </GameRule>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { useRules } from '@/core/adapter/game/rules';

import GameRule from './GameRule.vue';
import RuleContentDispatcher from './RuleContentDispatcher.vue';
import SubRule from './SubRule.vue';

const rules = useRules();
const displayedRules = computed(() => rules.rules);
</script>
