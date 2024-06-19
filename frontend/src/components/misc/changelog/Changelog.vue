<template>
  <div id="changelog" :ref="r => (changelogWrapper = r as HTMLDivElement)" class="py-1 px-5"></div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';

const props = defineProps<{
  changelog?: string;
}>();

const changelogWrapper = ref<HTMLDivElement | null>(null);

watch(
  () => props.changelog,
  changelog => {
    if (changelogWrapper.value && changelog) {
      changelogWrapper.value.innerHTML = changelog;
    }
  },
  { immediate: true }
);
</script>

<style>
#changelog {
  @apply text-text-secondary;
}

#changelog h1 {
  @apply text-4xl;
  @apply text-primary-text;
}

#changelog h2 {
  @apply text-3xl;
  @apply text-text;
}

#changelog h3 {
  @apply text-2xl;
  @apply text-text;
}

#changelog h4 {
  @apply text-xl;
  @apply text-text;
}

#changelog h5 {
  @apply text-lg;
}

#changelog h6 {
  @apply text-sm;
}

#changelog ol {
  @apply list-inside list-decimal;
}

#changelog ul {
  @apply list-inside list-disc ml-4;
}
</style>
