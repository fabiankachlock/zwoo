<template>
  <Widget v-model="isOpen" title="wait.rules" widget-class="bg-light" button-class="bg-main hover:bg-dark">
    <Rules />
  </Widget>
</template>

<script setup lang="ts">
import Widget from '../Widget.vue';
import Rules from '@/components/waiting/rules/Rules.vue';
import { useUserDefaults } from '@/composables/userDefaults';

const isOpen = useUserDefaults('lobby:widgetRulesOpen', false);
</script>
