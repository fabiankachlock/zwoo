<template>
  <template v-if="rule.ruleType === SettingsType.Boolean">
    <RuleSwitch :readonly="rule.isReadonly || !isHost" :model-value="rule.value === 1" @toggle="rulesStore.updateRule(rule.id, $event ? 1 : 0)" />
  </template>
  <template v-if="rule.ruleType === SettingsType.Numeric">
    <NumericRule :model-value="rule.value" :readonly="rule.isReadonly || !isHost" @update:model-value="rulesStore.updateRule(rule.id, $event)" />
  </template>
</template>

<script setup lang="ts">
import { DisplayRule, useRules } from '@/core/adapter/game/rules';
import { useIsHost } from '@/core/adapter/game/util/userRoles';
import { SettingsType } from '@/core/domain/zrp/zrpTypes';

import NumericRule from './contentTypes/NumericRule.vue';
import RuleSwitch from './contentTypes/RuleSwitch.vue';

const rulesStore = useRules();
const { isHost } = useIsHost();

defineProps<{
  rule: DisplayRule;
}>();
</script>
