<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';

import { useSearch } from '@/core/adapter/controller/search';

import SearchBox from './SearchBox.vue';

const search = useSearch();
const { locale } = useI18n();
const isOpen = computed(() => search.isOpen);
const languagePrefix = computed(() => {
  if (locale.value === 'en') {
    return '';
  }
  return `/${locale.value}`;
});
</script>

<template>
  <SearchBox
    v-if="isOpen"
    default-mode="docs"
    :urlPrefix="{
      docs: '/docs',
      api: '/docs/api',
      dev: '/docs/dev'
    }"
    :indexUris="{
      docs: `/docs${languagePrefix}/assets/searchIndex.js`,
      api: `/docs${languagePrefix}/api/assets/searchIndex.js`,
      dev: `/docs${languagePrefix}/dev/assets/searchIndex.js`
    }"
    @close="search.closeSearch()"
  />
</template>
