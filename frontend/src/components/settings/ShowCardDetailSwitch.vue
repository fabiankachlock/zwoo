<template>
  <button @click="toggleShouldShowDetail" class="tc-main text-xl ease-linear transition-transform hover:scale-110">
    <Icon icon="carbon:magnify" v-show="!shouldShowDetail" />
    <Icon icon="teenyicons:file-x-outline" v-show="shouldShowDetail" />
  </button>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { Icon } from '@/components/misc/Icon';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

const config = useConfig();
const shouldShowDetail = computed(() => config.get(ZwooConfigKey.ShowCardsDetail));

const toggleShouldShowDetail = () => {
  config.set(ZwooConfigKey.ShowCardsDetail, !shouldShowDetail.value);
};
</script>
