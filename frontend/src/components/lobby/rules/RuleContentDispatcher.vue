<template>
  <template v-if="rule.ruleType === RuleType.Boolean">
    <RuleSwitch :readonly="rule.isReadonly || !isHost" :modelValue="rule.value === 1" @toggle="rulesStore.updateRule(rule.id, $event ? 1 : 0)" />
  </template>
  <template v-if="rule.ruleType === RuleType.Numeric">
    <NumericRule :modelValue="rule.value" :readonly="rule.isReadonly || !isHost" @update:modelValue="rulesStore.updateRule(rule.id, $event)" />
  </template>
</template>

<script setup lang="ts">
import { defineProps } from 'vue';

import { DisplayRule, useRules } from '@/core/adapter/game/rules';
import { useIsHost } from '@/core/adapter/game/util/userRoles';
import { RuleType } from '@/core/domain/game/GameRules';

import NumericRule from './contentTypes/NumericRule.vue';
import RuleSwitch from './contentTypes/RuleSwitch.vue';

const rulesStore = useRules();
const { isHost } = useIsHost();

defineProps<{
  rule: DisplayRule;
}>();
</script>
