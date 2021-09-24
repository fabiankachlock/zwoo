<template>
  <div>
    <select
      :ref="
        r => {
          selection = r as HTMLSelectElement;
        }
      "
      class="bg-light p-1 rounded tc-main-dark"
    >
      <option v-for="lng in supportedLanguages" :key="lng" :value="lng">{{ t('lng.' + lng) }}</option>
    </select>
  </div>
</template>

<script setup lang="ts">
import { supportedLanguages, defaultLanguage } from '@/i18n';
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';
const { t, locale } = useI18n();

const languageKey = 'zwoo:lng';

const selection = ref<HTMLSelectElement>();

onMounted(() => {
  const stored = localStorage.getItem(languageKey);
  if (stored && selection.value) {
    selection.value.value = stored;
    locale.value = stored;
  } else if (selection.value) {
    selection.value.value = defaultLanguage;
    localStorage.setItem(languageKey, defaultLanguage);
  }

  selection.value?.addEventListener('change', () => {
    const lng = selection.value?.value || '';
    localStorage.setItem(languageKey, lng);
    locale.value = lng;
  });
});
</script>
