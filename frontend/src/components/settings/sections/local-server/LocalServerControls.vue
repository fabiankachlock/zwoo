<script setup lang="ts">
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
const isRunning = ref(false);

const toggleServer = () => {
  isRunning.value = !isRunning.value;
};
</script>

<template>
  <button
    class="flex justify-center items-center bg-light border-2 border-transparent px-2 rounded transition hover:bg-main cursor-pointer select-none"
    v-if="!isRunning"
    @click="toggleServer"
  >
    <p class="tc-main-light text-center">{{ t('settings.sections.server.start') }}</p>
  </button>
  <button
    class="flex justify-center items-center bg-light border-2 border-transparent px-2 rounded transition hover:bg-main cursor-pointer select-none"
    v-else
    @click="toggleServer"
  >
    <p class="tc-main-light text-center">{{ t('settings.sections.server.stop') }}</p>
  </button>
</template>
