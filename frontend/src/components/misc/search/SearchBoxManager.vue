<script setup lang="ts">
import { computed } from 'vue';

import { useSearch } from '@/core/adapter/controller/search';

import SearchBox from './SearchBox.vue';

const search = useSearch();
const isOpen = computed(() => search.isOpen);
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
      docs: '/docs/assets/searchIndex.js',
      api: '/docs/api/assets/searchIndex.js',
      dev: '/docs/dev/assets/searchIndex.js'
    }"
    @close="search.closeSearch()"
  />
</template>
