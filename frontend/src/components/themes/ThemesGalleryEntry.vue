<template>
  <div class="flex flex-col flex-nowrap bg-dark rounded-lg w-full border-2 bc-darkest hover:bc-primary py-4 relative">
    <h3 class="tc-main-dark text-2xl mx-4 mb-2">
      {{ props.theme.name }}
    </h3>
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
          class="tc-main text-base mx-1 py-1 px-2 rounded bg-light hover:bg-main border bc-light hover:bc-darkest cursor-pointer"
        >
          {{ variant }}
        </span>
      </p>
    </div>
    <div class="divider bc-darkest h-0 my-2 border-2 border-solid border-t-0"></div>
    <div class="mx-4">previews...</div>
    <div class="divider bc-darkest h-0 my-2 border-2 border-solid border-t-0"></div>
    <div class="mx-4">actions...</div>
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue';
import { CardThemeInformation } from '@/core/services/cards/CardThemeConfig';

const props = defineProps<{
  theme: CardThemeInformation;
}>();
</script>
