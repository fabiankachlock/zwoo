<script setup lang="ts">
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
</script>

<template>
  <div class="w-full flex flex-col bg-bg-surface p-3 my-3 rounded-lg border border-transparent mouse:hover:border-primary mouse:hover:bg-darkest">
    <div class="h-full text-text-light mb-2">
      {{ t('localServer.stats') }}
    </div>
    <div class="flex flex-row">Stats</div>
  </div>
</template>
