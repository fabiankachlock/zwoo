<template>
  <template v-if="rule.ruleType === RuleType.Boolean">
    <RuleSwitch :readonly="rule.isReadonly" :modelValue="rule.value === 1" @toggle="rulesStore.updateRule(rule.id, $event ? 1 : 0)" />
  </template>
  <template v-if="rule.ruleType === RuleType.Numeric">
    <NumericRule :modelValue="rule.value" :readonly="rule.isReadonly" @update:modelValue="rulesStore.updateRule(rule.id, $event)" />
  </template>
</template>

<script setup lang="ts">
import { defineProps } from 'vue';
import { RuleType } from '@/core/services/game/rules';
import { DisplayRule, useRules } from '@/core/adapter/play/rules';

import RuleSwitch from './contentTypes/RuleSwitch.vue';
import NumericRule from './contentTypes/NumericRule.vue';

const rulesStore = useRules();

defineProps<{
  rule: DisplayRule;
}>();
</script>
