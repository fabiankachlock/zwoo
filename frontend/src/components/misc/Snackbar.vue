<template>
  <div class="snackbar-container fixed z-50" :class="SnackBarPositionClasses[position]">
    <div class="snackbar-box bg-lightest rounded overflow-hidden max-w-xs relative">
      <div class="snackbar-content pl-3 pr-2 pt-2 pb-3 flex flex-row flex-nowrap items-center">
        <p class="text-sm tc-main">
          {{ msg ?? 'My Text 123' }}
        </p>
        <button class="ml-2 p-1 tc-main-dark bg-main hover:bg-dark rounded">
          <Icon icon="akar-icons:cross" class="text-xs" />
        </button>
      </div>
      <div class="snackbar-progress absolute bottom-0 left-0 right-0 h-2 rounded-b overflow-hidden">
        <div class="h-full bg-primary" style="width: 50%"></div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useSnackbar, SnackBarPositionClasses, SnackBarPosition } from '@/core/adapter/snackbar';
import { computed, watch } from 'vue';
import { Icon } from '@iconify/vue';

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
