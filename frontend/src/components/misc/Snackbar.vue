<template>
  <div class="fixed z-50 top-5 right-5 px-3 py-2 bg-lightest">
    {{ msg }}
  </div>
</template>

<script setup lang="ts">
import { useSnackbar } from '@/core/adapter/snackbar';
import { computed, watch } from 'vue';

const snackbar = useSnackbar();
const msg = computed(() => snackbar.activeMessage?.message);

watch(
  () => snackbar.activeMessage,
  msg => {
    console.log(msg);
  }
);
</script>
