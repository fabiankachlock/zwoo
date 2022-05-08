<template>
  <div class="flex flex-col flex-nowrap bg-dark rounded-lg w-full border-2 bc-darkest hover:bc-primary py-2 relative mb-2 overflow-hidden">
    <div class="flex flex-row flex-wrap justify-between items-center mb-1">
      <h3 class="tc-main-dark text-xl mx-2">
        {{ props.theme.name }}
        <Icon v-if="props.isSelected" icon="fluent:checkmark-circle-32-regular" class="inline-block tc-secondary ml-2" />
      </h3>
      <div class="mx-2">
        <ul class="tags flex flex-row justify-start items-center tc-main text-sm">
          <li
            v-if="props.theme.isMultiLayer"
            class="inline-tag bg-light hover:bg-main bc-light hover:bc-darkest flex flex-row flex-nowrap items-center"
          >
            <Icon icon="fluent:layer-20-filled" class="mr-2 tc-primary text-xl" />
            MultiLayer
          </li>
        </ul>
      </div>
    </div>
    <div class="mx-2">
      <p class="tc-main-light italic" v-if="props.theme.description">
        {{ props.theme.description }}
      </p>
      <p>
        <span class="tc-main-secondary text-xs italic mr-2"> By: </span>
        <span class="tc-main">
          {{ props.theme.author || 'unknown' }}
        </span>
      </p>
      <p>
        <span class="tc-main-secondary text-xs italic mr-2"> Variants: </span>
        <span
          v-for="variant in props.theme.variants"
          :key="variant"
          @click="previewVariant = variant"
          :class="{
            'bc-primary hover:bc-primary': previewVariant === variant,
            'bc-secondary hover:bc-primary': selectedVariant === variant
          }"
          class="tc-main text-xs inline-tag bg-light hover:bg-main bc-light hover:bc-darkest cursor-pointer"
        >
          {{ variant }}
        </span>
      </p>
    </div>
    <div class="divider bc-darkest h-0 my-2 border-2 border-solid border-t-0"></div>
    <div class="relative px-2 card-preview-wrapper flex flex-row w-full max-w-full overflow-x-auto">
      <div v-for="card in theme.previews" :key="card" class="card-preview max-h-full w-full mr-2">
        <Card :layers="getCardLayers(card)" :alt="card" />
      </div>
    </div>
    <div class="divider bc-darkest h-0 my-2 border-2 border-solid border-t-0"></div>
    <div class="mx-2">
      <div class="flex flex-row flex-nowrap justify-end items-center mt-2">
        <button class="py-1 px-2 rounded ml-2 bg-light hover:bg-main tc-primary" @click="selectAsTheme()">Select As Theme</button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { defineProps, onMounted, ref, watch } from 'vue';
import { Icon } from '@iconify/vue';
import { CardThemeInformation, CARD_THEME_VARIANT_AUTO } from '@/core/services/cards/CardThemeConfig';
import { useCardTheme } from '@/core/adapter/play/cardTheme';
import { CardTheme } from '@/core/services/cards/CardTheme';
import { CardThemeManager } from '@/core/services/cards/ThemeManager';
import { useColorTheme } from '@/composables/colorTheme';
import Card from '../game/Card.vue';
const { setTheme } = useCardTheme();

const props = defineProps<{
  theme: CardThemeInformation;
  isSelected: boolean;
  selectedVariant: string;
}>();

const colorMode = useColorTheme();
const previewVariant = ref('');
const previewTheme = ref<CardTheme | undefined>(undefined);

onMounted(async () => {
  const variant = props.theme.variants.includes(CARD_THEME_VARIANT_AUTO) ? colorMode.value : props.theme.variants[0] ?? '';
  previewVariant.value = variant;
  await loadTheme(variant === CARD_THEME_VARIANT_AUTO ? colorMode.value : variant);
});

watch([previewVariant, colorMode], ([variant, colorMode]) => loadTheme(variant === CARD_THEME_VARIANT_AUTO ? colorMode : variant));

const loadTheme = async (variant: string) => {
  const theme = await CardThemeManager.global.loadPreview({
    name: props.theme.name,
    variant: variant
  });
  previewTheme.value = theme;
};

const getCardLayers = (descriptor: string) => previewTheme.value?.getCard(descriptor).layers ?? [];

const selectAsTheme = () => {
  console.log('selecting', props.theme.name, previewVariant.value, 'as theme');
  setTheme(props.theme.name, previewVariant.value);
};
</script>

<style>
.inline-tag {
  @apply mr-2 py-1 px-2 rounded  border;
}

.card-preview {
  position: relative;
  height: min(20vh, 500px);
  min-height: min(20vh, 500px);
  max-height: min(20vh, 500px);
  width: calc(min(20vh, 500px) * (420 / 720));
  min-width: calc(min(20vh, 500px) * (420 / 720));
}

.card-preview:last-of-type {
  margin-right: 0;
}

.card-preview-wrapper {
  height: min(20vh, 500px);
  max-height: min(20vh, 500px);
}
</style>
