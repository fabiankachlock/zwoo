<template>
  <div v-if="msg" class="snackbar-container fixed z-50" :class="SnackBarPositionClasses[msg.position]">
    <div class="snackbar-box bg-lightest rounded overflow-hidden max-w-xs relative">
      <div class="snackbar-content px-3 pt-2 pb-2 flex flex-row flex-nowrap items-center">
        <p class="text-sm tc-main">
          {{ msg.needsTranslation ? t(msg.message) : msg.message }}
        </p>
        <button
          v-if="msg.showClose !== undefined ? msg.showClose : true"
          @click="close()"
          class="ml-2 p-1 tc-main-dark bg-main hover:bg-dark rounded"
        >
          <Icon icon="akar-icons:cross" class="text-xs" />
        </button>
      </div>
      <div class="snackbar-progress absolute bottom-0 left-0 right-0 h-1 rounded-b overflow-hidden">
        <div
          class="h-full"
          :class="msg.color ? `bg-${msg.color}` : 'bg-_bg-dark-lightest dark:bg-_bg-light-darkest'"
          :style="constructAnimationString()"
          style="animation-timing-function: linear"
        ></div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useSnackbar, SnackBarPositionClasses } from '@/core/adapter/snackbar';
import { computed } from 'vue';
import { Icon } from '@iconify/vue';
import { useI18n } from 'vue-i18n';

const snackbar = useSnackbar();
const msg = computed(() => snackbar.activeMessage);

const { t } = useI18n();

const close = () => {
  snackbar.close();
};

const constructAnimationString = () => {
  // defined message: set animation
  // undefined message: reset animation
  return msg.value ? `animation: transition-progress; animation-duration: ${msg.value?.duration ?? 0}ms` : 'animation: none';
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
</style>
