<template>
  <div v-if="msg" class="snackbar-container fixed z-50" :class="SnackBarPositionClasses[msg.position]">
    <div class="snackbar-box bg-lightest rounded overflow-hidden max-w-xs relative">
      <div class="snackbar-content pl-3 pr-2 pt-2 pb-3 flex flex-row flex-nowrap items-center">
        <p class="text-sm tc-main">
          {{ msg.needsTranslation ? t(msg.message) : msg.message }}
        </p>
        <button v-if="msg.showClose" @click="close()" class="ml-2 p-1 tc-main-dark bg-main hover:bg-dark rounded">
          <Icon icon="akar-icons:cross" class="text-xs" />
        </button>
      </div>
      <div class="snackbar-progress absolute bottom-0 left-0 right-0 h-2 rounded-b overflow-hidden">
        <div class="h-full" :class="msg.color ? `bg-${msg.color}` : 'bg-_bg-dark-lightest dark:bg-_bg-light-darkest'" style="width: 50%"></div>
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
</script>
