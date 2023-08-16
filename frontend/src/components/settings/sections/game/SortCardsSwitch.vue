<template>
  <SettingsSwitch :model-value="shouldSortCards" @update:model-value="toggleSortCards">
    <div class="tc-main text-xl">
      <Icon v-show="shouldSortCards" icon="icon-park-outline:sort-amount-down" />
      <Icon v-show="!shouldSortCards" icon="uis:bars" />
    </div>
  </SettingsSwitch>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { Icon } from '@/components/misc/Icon';
import SettingsSwitch from '@/components/settings/SettingsSwitch.vue';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

const config = useConfig();
const shouldSortCards = computed(() => config.get(ZwooConfigKey.SortCards));

const toggleSortCards = () => {
  config.set(ZwooConfigKey.SortCards, !shouldSortCards.value);
};
</script>
