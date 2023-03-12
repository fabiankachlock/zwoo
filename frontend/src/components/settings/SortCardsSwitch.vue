<template>
  <SettingsSwitch :model-value="shouldSortCards" @update:model-value="toggleSortCards">
    <div class="tc-main text-xl">
      <Icon icon="icon-park-outline:sort-amount-down" v-show="shouldSortCards" />
      <Icon icon="uis:bars" v-show="!shouldSortCards" />
    </div>
  </SettingsSwitch>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { Icon } from '@/components/misc/Icon';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

import SettingsSwitch from './common/SettingsSwitch.vue';

const config = useConfig();
const shouldSortCards = computed(() => config.get(ZwooConfigKey.SortCards));

const toggleSortCards = () => {
  config.set(ZwooConfigKey.SortCards, !shouldSortCards.value);
};
</script>
