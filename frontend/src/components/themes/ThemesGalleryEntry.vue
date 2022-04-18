<template>
  <div class="flex flex-col flex-nowrap bg-dark rounded-lg w-full border-2 bc-darkest hover:bc-primary py-4 relative">
    <div class="flex flex-row flex-wrap justify-between items-center mb-2">
      <h3 class="tc-main-dark text-2xl mx-4">
        {{ props.theme.name }}
        <Icon v-if="props.isSelected" icon="fluent:checkmark-circle-32-regular" class="inline-block tc-secondary ml-2" />
      </h3>
      <div class="mx-4">
        <ul class="tags flex flex-row justify-start items-center tc-main text-sm">
          <li
            v-if="props.theme.isMultilayer || true"
            class="inline-tag bg-light hover:bg-main bc-light hover:bc-darkest flex flex-row flex-nowrap items-center"
          >
            <Icon icon="fluent:layer-20-filled" class="mr-2 tc-primary text-xl" />
            MultiLayer
          </li>
        </ul>
      </div>
    </div>
    <div class="mx-4">
      <p class="tc-main-light italic" v-if="props.theme.description">
        {{ props.theme.description }}
      </p>
      <p>
        <span class="tc-main-secondary italic mr-2"> By: </span>
        <span class="tc-main text-lg">
          {{ props.theme.author || 'unknown' }}
        </span>
      </p>
      <p>
        <span class="tc-main-secondary italic mr-2"> Variants: </span>
        <span
          v-for="variant in props.theme.variants"
          :key="variant"
          @click="previewVariant = variant"
          :class="{ 'bc-primary hover:bc-primary': previewVariant === variant }"
          class="tc-main inline-tag bg-light hover:bg-main bc-light hover:bc-darkest cursor-pointer"
        >
          {{ variant }}
        </span>
      </p>
    </div>
    <div class="divider bc-darkest h-0 my-2 border-2 border-solid border-t-0"></div>
    <div class="mx-4">previews..., showing {{ previewVariant }}</div>
    <div class="divider bc-darkest h-0 my-2 border-2 border-solid border-t-0"></div>
    <div class="mx-4">
      <div class="flex flex-row flex-nowrap justify-end items-center mt-2">
        <button class="py-2 px-3 rounded ml-2 bg-light hover:bg-main tc-primary" @click="selectAsTheme()">Select As Theme</button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { defineProps, ref } from 'vue';
import { Icon } from '@iconify/vue';
import { CardThemeInformation } from '@/core/services/cards/CardThemeConfig';

const props = defineProps<{
  theme: CardThemeInformation;
  isSelected: boolean;
}>();

const previewVariant = ref(props.theme.variants[0] ?? '');

const selectAsTheme = () => {
  console.log('selecting', props.theme.name, previewVariant.value, 'as theme');
};
</script>

<style>
.inline-tag {
  @apply text-base mr-2 py-1 px-2 rounded  border;
}
</style>
