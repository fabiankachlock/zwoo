<template>
  <div :class="{ 'justify-end': isOwn }" class="flex justify-start w-full my-0.5 max-w-full relative">
    <p
      :class="{
        'rounded-bl-none border-transparent mb-3': !isOwn,
        'rounded-br-none bc-primary': isOwn,
        'bg-main tc-main-light': isSpectator,
        'bg-dark tc-main-secondary': !isSpectator
      }"
      class="px-2 py-0.5 rounded-2xl border break-all"
    >
      {{ message }}
    </p>
    <p v-if="!isOwn" class="text-xs absolute -bottom-0.5 italic">{{ name }}</p>
  </div>
</template>

<script setup lang="ts">
import { toRefs, defineProps } from 'vue';

const props = defineProps<{
  message: string;
  isOwn?: boolean;
  isSpectator?: boolean;
  name: string;
}>();

const { message, isOwn, name } = toRefs(props);
</script>
