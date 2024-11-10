<template>
  <div class="flex flex-col flex-nowrap bg-surface rounded-lg w-full border-2 border-border py-2 relative mb-2 overflow-hidden">
    <div class="flex flex-row flex-wrap justify-between items-center mb-1">
      <h3 class="text-text text-xl mx-2">
        {{ props.theme.name.startsWith('__') ? t(`cardThemes.builtins.${props.theme.name}`) : props.theme.name }}
        <Icon v-if="props.isSelected" icon="fluent:checkmark-circle-32-regular" class="inline-block text-secondary-text ml-2" />
      </h3>
      <div class="mx-2">
        <ul class="tags flex flex-row justify-start items-center text-text text-sm">
          <li v-if="props.theme.isMultiLayer" class="inline-tag bg-bg border-border flex flex-row flex-nowrap items-center select-none">
            <Icon icon="fluent:layer-20-filled" class="mr-2 text-primary-text text-xl" />
            {{ t('cardThemes.multiLayer') }}
          </li>
        </ul>
      </div>
    </div>
    <div class="mx-2">
      <p v-if="props.theme.description" class="text-text italic">
        {{ t(`cardThemes.description.${props.theme.description}`) }}
      </p>
      <p>
        <span class="text-text-secondary text-xs italic mr-2">{{ t('cardThemes.by') }}</span>
        <span class="text-text">
          {{ props.theme.author || t('cardThemes.authorUnknown') }}
        </span>
      </p>
      <p>
        <span class="text-text-secondary text-xs italic mr-2">{{ t('cardThemes.variants') }}</span>
        <span
          v-for="variant in props.theme.variants"
          :key="variant"
          :class="{
            'border-primary hover:border-primary': previewVariant === variant,
            'border-secondary hover:border-primary': selectedVariant === variant
          }"
          class="text-text text-xs inline-tag bg-bg hover:bg-bg border-border hover:border-border cursor-pointer select-none"
          @click="previewVariant = variant"
        >
          {{ t(`cardThemes.variant.${variant}`) }}
        </span>
      </p>
    </div>
    <div class="divider border-border h-0 my-2 border-2 border-solid border-t-0"></div>
    <div class="relative px-2 flex flex-row w-full max-w-full overflow-x-auto">
      <div v-for="card in theme.previews" :key="card" class="card-preview max-h-full w-full mr-2">
        <Card :card="card" :override-theme="previewTheme as CardTheme" />
      </div>
    </div>
    <div class="divider border-border h-0 my-2 border-2 border-solid border-t-0"></div>
    <div class="mx-2">
      <div class="flex flex-row flex-nowrap justify-end items-center">
        <button class="py-1 px-2 rounded ml-2 bg-alt hover:bg-alt-hover text-primary-text" @click="selectAsTheme()">
          {{ t('cardThemes.useTheme') }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';

import { Icon } from '@/components/misc/Icon';
import { useCardTheme } from '@/core/adapter/game/cardTheme';
import { useColorTheme } from '@/core/adapter/helper/useColorTheme';
import { CardTheme } from '@/core/domain/cards/CardTheme';
import { CARD_THEME_VARIANT_AUTO, CardThemeInformation } from '@/core/domain/cards/CardThemeConfig';
import { CardThemeManager } from '@/core/domain/cards/ThemeManager';

import Card from '../game/Card.vue';
const { setTheme } = useCardTheme();

const props = defineProps<{
  theme: CardThemeInformation;
  isSelected: boolean;
  selectedVariant: string;
}>();

const { t } = useI18n();
const colorMode = useColorTheme();
const previewVariant = ref('');
const previewTheme = ref<CardTheme | undefined>(undefined);

onMounted(async () => {
  const variant = props.theme.variants.includes(CARD_THEME_VARIANT_AUTO) ? colorMode.value : (props.theme.variants[0] ?? '');
  if (props.isSelected) {
    previewVariant.value = props.selectedVariant !== '' ? props.selectedVariant : variant;
    await loadTheme(previewVariant.value);
  } else {
    previewVariant.value = variant;
    await loadTheme(variant);
  }
});

watch([previewVariant, colorMode], ([variant, colorMode]) => loadTheme(variant === CARD_THEME_VARIANT_AUTO ? colorMode : variant));

const loadTheme = async (variant: string) => {
  const theme = await CardThemeManager.global.loadPreview({
    name: props.theme.name,
    variant: variant
  });
  previewTheme.value = theme;
};

const selectAsTheme = () => {
  setTheme(props.theme.name, previewVariant.value);
};
</script>

<style scoped>
.inline-tag {
  @apply mr-2 py-1 px-2 rounded  border;
}

.card-preview {
  position: relative;
  width: calc(min(20vh, 500px) * (420 / 720));
}

.card-preview:last-of-type {
  margin-right: 0;
}
</style>
