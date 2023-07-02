<template>
  <div
    v-tooltip="t(isOpen ? 'wait.collapse' : 'wait.expand')"
    class="px-2 py-1 my-1 bg-dark border bc-darkest transition mouse:hover:bc-primary rounded-lg mouse:hover:bg-darkest cursor-pointer"
    @click="toggleOpenState"
  >
    <div class="flex flex-row flex-nowrap justify-between items-center">
      <p class="text-lg tc-main-dark">{{ t(translatedTitle) }}</p>
      <div>
        <slot></slot>
      </div>
    </div>
    <div class="rule-body overflow-hidden transition duration-300" :class="{ open: isOpen }">
      <div class="divider w-full my-2 bc-invert-darkest border-b"></div>
      <div class="content">
        <p class="text-sm italic tc-main-secondary">
          {{ t(translatedDescription) }}
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, toRefs } from 'vue';
import { useI18n } from 'vue-i18n';

import { defaultLanguage } from '@/i18n';

const props = defineProps<{
  title: Record<string, string>;
  description: Record<string, string>;
}>();

const { title, description } = toRefs(props);

const { t, locale } = useI18n();
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

<style scoped>
.rule-body {
  transition-property: max-height;
  max-height: 0;
}
.rule-body.open {
  transition-property: max-height;
  max-height: 200px;
}
</style>
