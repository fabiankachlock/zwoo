<template>
  <h2 class="text-4xl tc-main text-center my-6">zwoo - Theme&nbsp;Gallery</h2>
  <div class="flex flex-col mx-2 sm:mx-8 my-2">
    <ThemesGalleryEntry
      v-for="theme in themes"
      :key="theme.name"
      :theme="theme"
      :is-selected="selectedCardTheme === theme.name"
      :selected-variant="selectedCardTheme === theme.name ? selectedCardThemeVariant : ''"
    />
  </div>
</template>

<script setup lang="ts">
import { CardThemeInformation } from '@/core/services/cards/CardThemeConfig';
import { CardThemeManager } from '@/core/services/cards/ThemeManager';
import { onMounted, ref } from 'vue';
import ThemesGalleryEntry from '@/components/themes/ThemesGalleryEntry.vue';
import { useConfig } from '@/core/adapter/config';
import { computed } from '@vue/reactivity';

const themes = ref<CardThemeInformation[]>([]);
const config = useConfig();
const selectedCardTheme = computed(() => config.cardTheme);
const selectedCardThemeVariant = computed(() => config.cardThemeVariant);

onMounted(async () => {
  const loadedThemes = await CardThemeManager.global.getAllThemesInformation();
  themes.value = loadedThemes;
});
</script>
