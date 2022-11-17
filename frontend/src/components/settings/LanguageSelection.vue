<template>
  <div>
    <select
      :ref="
        r => {
          selection = r as HTMLSelectElement;
        }
      "
      :value="selectedLng"
      class="bg-light p-1 rounded tc-main-dark"
    >
      <option v-for="lng in supportedLanguages" :key="lng" :value="lng">{{ t('lng.' + lng) }}</option>
    </select>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

import { useConfig } from '@/core/adapter/config';
import { supportedLanguages } from '@/i18n';

const { t } = useI18n();
const config = useConfig();
const selection = ref<HTMLSelectElement>();
const selectedLng = computed(() => config.language);

onMounted(() => {
  selection.value?.addEventListener('change', () => {
    const lng = selection.value?.value || 'en';
    config.setLanguage(lng);
  });
});
</script>
