<template>
  <MaxWidthLayout size="normal">
    <div class="sticky z-10 bg-bg top-10">
      <h2 class="text-text text-4xl py-2 text-center">{{ t('cardThemes.galleryTitle') }}</h2>
    </div>
    <div class="flex flex-col my-2">
      <ThemesGalleryEntry
        v-for="theme in themes"
        :key="theme.name"
        :theme="theme"
        :is-selected="selectedCardTheme.name === theme.name"
        :selected-variant="selectedCardTheme.name === theme.name ? selectedCardTheme.variant : ''"
      />
    </div>
  </MaxWidthLayout>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

import ThemesGalleryEntry from '@/components/themes/ThemesGalleryEntry.vue';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';
import { CardThemeInformation } from '@/core/domain/cards/CardThemeConfig';
import { CardThemeManager } from '@/core/domain/cards/ThemeManager';
import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';

const { t } = useI18n();
const themes = ref<CardThemeInformation[]>([]);
const config = useConfig();
const selectedCardTheme = computed(() => config.get(ZwooConfigKey.CardsTheme));

onMounted(async () => {
  const loadedThemes = await CardThemeManager.global.getAllThemesInformation();
  themes.value = loadedThemes;
});
</script>
