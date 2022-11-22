<template>
  <button @click="toggleSortCards" class="tc-main text-xl ease-linear transition-transform hover:scale-110">
    <Icon icon="icon-park-outline:sort-amount-down" v-show="!shouldSortCards" />
    <Icon icon="uis:bars" v-show="shouldSortCards" />
  </button>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { Icon } from '@/components/misc/Icon';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

const config = useConfig();
const shouldSortCards = computed(() => config.get(ZwooConfigKey.SortCards));

const toggleSortCards = () => {
  config.set(ZwooConfigKey.SortCards, !shouldSortCards.value);
};
</script>
