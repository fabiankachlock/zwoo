<template>
  <div :class="{ 'justify-end': isOwn, 'justify-center': isSystem }" class="flex justify-start w-full my-0.5 max-w-full relative">
    <p
      :class="{
        'w-full rounded-none  text-center mb-0 border-transparent bg-transparent italic text-xs tc-main-secondary': isSystem,
        'rounded-bl-none border-transparent mb-3': !isOwn && !isSystem,
        'rounded-br-none bc-primary': isOwn,
        'bc-secondary': isHost,
        'bg-dark tc-main-secondary': isSpectator,
        'bg-main tc-main-light': !isSpectator && !isSystem
      }"
      class="px-2 py-0.5 rounded-2xl border break-all"
    >
      {{ message }}
    </p>
    <p v-if="!isOwn && !isSystem" class="text-xs absolute bottom-[-0.2rem] italic tc-main-secondary">{{ name }}</p>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  message: string;
  isSystem: boolean;
  isOwn?: boolean;
  isSpectator?: boolean;
  isHost?: boolean;
  name: string;
}>();
</script>
