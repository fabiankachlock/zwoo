<template>
  <div :class="{ 'justify-end': isOwn, 'justify-center': isSystem }" class="flex justify-start w-full my-0.5 max-w-full relative">
    <p
      :class="{
        'bg-surface w-full rounded-none  text-center mb-0 border-transparent bg-transparent italic text-xs text-text-secondary': isSystem,
        'bg-surface rounded-bl-none border-transparent mb-3': !isOwn && !isSystem,
        'bg-surface rounded-br-none border-primary': isOwn,
        'bg-surface border-secondary': isHost,
        'bg-bg text-text-secondary': isSpectator,
        'bg-bg text-text': !isSpectator && !isSystem
      }"
      class="px-2 py-0.5 rounded-2xl border break-all"
    >
      {{ message }}
    </p>
    <p v-if="!isOwn && !isSystem" class="text-xs absolute bottom-[-0.2rem] italic text-text-secondary">{{ name }}</p>
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
