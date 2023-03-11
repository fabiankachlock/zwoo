<template>
  <SettingsSwitch :model-value="shouldShowDetail" @update:model-value="toggleShouldShowDetail">
    <div class="tc-main text-xl">
      <Icon icon="carbon:magnify" v-show="shouldShowDetail" />
      <Icon icon="teenyicons:file-x-outline" v-show="!shouldShowDetail" />
    </div>
  </SettingsSwitch>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { Icon } from '@/components/misc/Icon';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

import SettingsSwitch from './common/SettingsSwitch.vue';

const config = useConfig();
const shouldShowDetail = computed(() => config.get(ZwooConfigKey.ShowCardsDetail));

const toggleShouldShowDetail = () => {
  config.set(ZwooConfigKey.ShowCardsDetail, !shouldShowDetail.value);
};
</script>
