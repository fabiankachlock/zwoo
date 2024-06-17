<template>
  <div class="py-1 px-2 my-1 bg-bg border border-border rounded-lg">
    <div class="flex flex-row flex-nowrap justify-between items-center gap-2">
      <div class="flex items-center justify-start gap-2 flex-grow cursor-pointer" @click.stop="toggleOpenState">
        <button class="rounded-lg transition group active:scale-95">
          <Icon v-if="!isOpen" icon="akar-icons:circle-plus" class="text-text group-hover:text-primary-text text-xl" />
          <Icon v-else icon="akar-icons:circle-chevron-up" class="text-text group-hover:text-primary-text text-xl" />
        </button>
        <p class="text-lg text-text-dark flex-grow">{{ translatedTitle }}</p>
      </div>
      <div class="relative">
        <slot></slot>
      </div>
    </div>
    <div class="grid grid-rows-[0fr] transition-[grid-template-rows]" :class="{ 'grid-rows-[1fr]': isOpen }">
      <div class="relative overflow-hidden">
        <div class="flex flex-col gap-1 mt-2">
          <p class="text-sm text-text-secondary px-1">
            {{ translatedDescription }}
          </p>
          <slot name="subrules"></slot>
        </div>
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
