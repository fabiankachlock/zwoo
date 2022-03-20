<template>
  <div :class="{ 'justify-end': isOwn }" class="flex justify-start w-full my-0.5 max-w-full">
    <p
      :class="{
        'rounded-bl-none border-transparent': !isOwn,
        'rounded-br-none bc-primary': isOwn,
        'bg-dark tc-main-secondary': isSpectator
      }"
      class="bg-main tc-main-secondary px-2 py-0.5 rounded-2xl border break-all"
    >
      {{ message }}
    </p>
  </div>
</template>

<script setup lang="ts">
import { toRefs, defineProps } from 'vue';

const props = defineProps<{
  message: string;
  isOwn?: boolean;
  isSpectator?: boolean;
}>();

const { message, isOwn } = toRefs(props);
</script>
