<template>
  <div
    :class="{ 'border-secondary': isActive }"
    class="opponent-wrapper px-2 py-1 h-full rounded-sm mx-2 border border-transparent bg-darkest hover:bg-dark"
    :ref="r => (elmRef = r as HTMLDivElement)"
  >
    <div class="flex flex-row flex-nowrap w-full h-full items-center tc-main cursor-default overflow-hidden whitespace-nowrap">
      <span class="mr-3 opponent-name">{{ name }}</span>
      <span class="whitespace-nowrap">{{ cardAmount }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { toRefs, defineProps, watch, ref } from 'vue';

const props = defineProps<{
  isActive: boolean;
  name: string;
  cardAmount: number;
}>();

const { name, cardAmount, isActive } = toRefs(props);
const elmRef = ref<HTMLDivElement | null>(null);

watch(isActive, newValue => {
  if (newValue) {
    elmRef.value?.scrollIntoView();
  }
});
</script>

<style>
.opponent-wrapper {
  max-width: 20vw;
}

.opponent-name {
  text-overflow: ellipsis;
  overflow: hidden;
}
</style>
