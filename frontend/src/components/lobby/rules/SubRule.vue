<template>
  <div class="px-2 py-1 bg-light border bc-dark rounded-lg">
    <div class="flex flex-row flex-nowrap justify-between items-center gap-2">
      <div class="flex items-center justify-start gap-2 flex-grow cursor-pointer" @click.stop="toggleOpenState">
        <button class="rounded-lg transition group active:scale-95">
          <Icon v-if="!isOpen" icon="akar-icons:circle-plus" class="tc-main group-hover:tc-primary text-xl" />
          <Icon v-else icon="akar-icons:circle-chevron-up" class="tc-main group-hover:tc-primary text-xl" />
        </button>
        <p class="text-lg tc-main-dark flex-grow">{{ translatedTitle }}</p>
      </div>
      <div class="relative">
        <slot></slot>
      </div>
    </div>
    <div class="grid grid-rows-[0fr] transition-[grid-template-rows]" :class="{ 'grid-rows-[1fr]': isOpen }">
      <div class="relative overflow-hidden px-1">
        <p class="text-sm tc-main-secondary">
          {{ translatedDescription }}
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, toRefs } from 'vue';
import { useI18n } from 'vue-i18n';

import { Icon } from '@/components/misc/Icon';
import { defaultLanguage } from '@/i18n';

const props = defineProps<{
  title: Record<string, string>;
  description: Record<string, string>;
}>();

const { title, description } = toRefs(props);

const { locale } = useI18n();
const isOpen = ref(false);
const translatedTitle = computed(() => getTranslationOrFallback(title.value, locale.value));
const translatedDescription = computed(() => getTranslationOrFallback(description.value, locale.value));

const toggleOpenState = () => {
  isOpen.value = !isOpen.value;
};

const getTranslationOrFallback = (translateAble: Record<string, string>, locale: string): string => {
  if (translateAble[locale]) return translateAble[locale];
  return translateAble[defaultLanguage];
};
</script>
