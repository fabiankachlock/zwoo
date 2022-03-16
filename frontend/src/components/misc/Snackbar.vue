<template>
  <div class="snackbar-container fixed z-50" :class="SnackBarPositionClasses[position]">
    <div class="snackbar-box px-3 py-2 bg-lightest rounded">
      {{ msg }}
    </div>
  </div>
</template>

<script setup lang="ts">
import { useSnackbar, SnackBarPositionClasses, SnackBarPosition } from '@/core/adapter/snackbar';
import { computed, watch } from 'vue';

const snackbar = useSnackbar();
const msg = computed(() => snackbar.activeMessage?.message);
const position = computed(() => snackbar.activeMessage?.position ?? SnackBarPosition.TopRight);

watch(
  () => snackbar.activeMessage,
  msg => {
    console.log(msg);
  }
);
</script>
