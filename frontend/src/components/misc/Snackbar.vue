<template>
  <div v-if="msg" class="snackbar-container fixed z-50 pointer-events-none" :class="SnackBarPositionClasses[msg.position]">
    <div class="snackbar-box bg-bg rounded overflow-hidden max-w-xs relative">
      <div class="snackbar-content px-3 pt-2 pb-2 flex flex-row flex-nowrap items-center pointer-events-auto">
        <p class="text-sm text-text">
          {{ msg.needsTranslation ? t(msg.message) : msg.message }}
        </p>
        <button
          v-if="msg.showClose !== undefined ? msg.showClose : true"
          class="ml-2 p-1 text-text-dark bg-bg hover:bg-surface rounded"
          @click="close()"
        >
          <Icon icon="akar-icons:cross" class="text-xs" />
        </button>
      </div>
      <div v-if="!msg.hideProgress" class="snackbar-progress absolute bottom-0 left-0 right-0 h-1 rounded-b overflow-hidden">
        <div
          class="h-full relative"
          :class="msg.color ? `bg-${msg.color}` : 'bg-_bg-dark-lightest dark:bg-_bg-light-darkest'"
          :style="constructAnimationString()"
          style="animation-timing-function: linear"
        ></div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';

import { Icon } from '@/components/misc/Icon';
import { SnackBarPositionClasses, useSnackbar } from '@/core/adapter/snackbar';

const snackbar = useSnackbar();
const msg = computed(() => snackbar.activeMessage);

const { t } = useI18n();

const close = () => {
  snackbar.close();
};

const constructAnimationString = () => {
  // defined message: set animation
  // undefined message: reset animation
  if (!msg.value) {
    return 'animation: none';
  } else if (msg.value.mode === 'static') {
    return `animation: transition-progress; animation-duration: ${msg.value?.duration ?? 0}ms`;
  } else if (msg.value.mode === 'loading') {
    return `animation: 800ms linear alternate animation-loading infinite;`;
  }
};
</script>

<style scopes>
@keyframes transition-progress {
  0% {
    width: 0%;
  }
  100% {
    width: 100%;
  }
}

@keyframes animation-loading {
  0% {
    left: 0%;
    width: 5%;
  }
  10% {
    left: 0%;
    width: 20%;
  }
  50% {
    left: 30%;
    width: 40%;
  }
  90% {
    left: 80%;
    width: 20%;
  }
  100% {
    left: 95%;
    width: 5%;
  }
}
</style>
