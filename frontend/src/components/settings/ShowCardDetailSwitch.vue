<template>
  <button @click="toggleShouldShowDetail" class="tc-main text-xl ease-linear transition-transform hover:scale-110">
    <Icon icon="carbon:magnify" v-show="!shouldShowDetail" />
    <Icon icon="teenyicons:file-x-outline" v-show="shouldShowDetail" />
  </button>
</template>

<script setup lang="ts">
import { useConfig } from '@/core/adapter/config';
import { Icon } from '@iconify/vue';
import { computed } from 'vue';

const config = useConfig();
const shouldShowDetail = computed(() => config.showCardDetail);

const toggleShouldShowDetail = () => {
  config.setShowCardDetail(!shouldShowDetail.value);
};
</script>
