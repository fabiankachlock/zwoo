<template>
  <Widget v-model="isOpen" title="wait.rules" widget-class="bg-light" button-class="bg-main hover:bg-dark">
    <Rules />
  </Widget>
</template>

<script setup lang="ts">
import Rules from '@/components/lobby/rules/Rules.vue';
import { useUserDefaults } from '@/composables/userDefaults';

import Widget from '../Widget.vue';

const isOpen = useUserDefaults('lobby:widgetRulesOpen', false);
</script>
