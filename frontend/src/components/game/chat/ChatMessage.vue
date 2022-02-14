<template>
  <div :class="{ 'justify-end': isOwn }" class="flex justify-start w-full my-0.5 max-w-full">
    <p
      :class="{ 'rounded-br-none border-transparent': !isOwn, 'rounded-bl-none bc-primary': isOwn }"
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
}>();

const { message, isOwn } = toRefs(props);
</script>
