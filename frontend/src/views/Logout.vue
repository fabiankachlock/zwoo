<template>
  <div class="w-full sm:max-w-xs mx-auto">
    <div class="bg-lightest shadow-md sm:rounded-sm px-6 py-4 mb-4 mt-8 relative">
      <router-link to="/" class="tc-main-secondary absolute left-3 top-3 text-xl transform transition-transform hover:-translate-x-1">
        <Icon icon="mdi:chevron-left" />
      </router-link>
      <div class="m-2">
        <Error v-if="error.length > 0" :errors="error" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { Icon } from '@iconify/vue';
import { useConfig } from '@/core/adapter/config';
import Error from '../components/misc/Error.vue';

const error = ref<string[]>([]);

const logout = async () => {
  error.value = [];

  try {
    await useConfig().logout();
  } catch (e: any) {
    error.value = Array.isArray(e) ? e : [e.toString()];
  }
};
logout();
</script>
