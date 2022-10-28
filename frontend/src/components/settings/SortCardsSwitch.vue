<template>
  <button @click="toggleSortCards" class="tc-main text-xl ease-linear transition-transform hover:scale-110">
    <Icon icon="icon-park-outline:sort-amount-down" v-show="!shouldSortCards" />
    <Icon icon="uis:bars" v-show="shouldSortCards" />
  </button>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { computed } from 'vue';

import { useConfig } from '@/core/adapter/config';

const config = useConfig();
const shouldSortCards = computed(() => config.sortCards);

const toggleSortCards = () => {
  config.setSortCards(!shouldSortCards.value);
};
</script>
