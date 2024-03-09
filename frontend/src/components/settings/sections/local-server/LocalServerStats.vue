<script setup lang="ts">
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
</script>

<template>
  <div class="w-full flex flex-col bg-dark p-3 my-3 rounded-lg border border-transparent mouse:hover:bc-primary mouse:hover:bg-darkest">
    <div class="h-full tc-main-light mb-2">
      {{ t('settings.sections.server.stats') }}
    </div>
    <div class="flex flex-row">Stats</div>
  </div>
</template>
