<template>
  <div class="w-full sm:max-w-xs mx-auto">
    <div class="m-2">
      <Error v-if="error.length > 0" :errors="error" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
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
