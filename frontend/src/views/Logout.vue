<template>
  <div class="w-full sm:max-w-xs mx-auto">
    <div class="m-2">
      <Error v-if="error !== undefined" :title="error" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useConfig } from '@/core/adapter/config';
import Error from '../components/misc/Error.vue';

const error = ref<string | undefined>(undefined);

const logout = async () => {
  try {
    await useConfig().logout();
  } catch (e) {
    if (Array.isArray(e)) {
      error.value = e.join('\n');
    }
  }
};
logout();
</script>
