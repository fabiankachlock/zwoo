<script setup lang="ts">
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
</script>

<template>
  <div class="w-full flex flex-col py-3 my-3">
    <div class="h-full text-text mb-2">
      {{ t('localServer.stats') }}
    </div>
    <div class="flex flex-row text-text-secondary">TODO: Stats</div>
  </div>
</template>
