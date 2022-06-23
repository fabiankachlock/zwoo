<template>
  <div
    :class="{ 'border-secondary': isActive }"
    class="opponent-wrapper px-2 py-1 rounded-sm mx-2 my-1 border border-transparent bg-darkest mouse:hover:bg-dark"
    :ref="r => (elmRef = r as HTMLDivElement)"
  >
    <div class="flex flex-row flex-nowrap w-full h-full items-center tc-main cursor-default overflow-hidden whitespace-nowrap">
      <span class="mr-3 opponent-name">{{ name }}</span>
      <span class="whitespace-nowrap">{{ cardAmount }}</span>
      <span class="ml-2 flex items-center">
        <button @click="toggleMute" class="transition-transform hover:scale-125">
          <Icon v-if="isMuted" icon="bi:mic-mute-fill" />
          <Icon v-else icon="bi:mic-fill" />
        </button>
      </span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { toRefs, defineProps, watch, ref } from 'vue';
import { Icon } from '@iconify/vue';
import { useChatStore } from '@/core/adapter/play/chat';

const chat = useChatStore();
const props = defineProps<{
  isActive: boolean;
  name: string;
  cardAmount: number;
  isMuted?: boolean;
}>();

const { name, cardAmount, isActive, isMuted } = toRefs(props);
const elmRef = ref<HTMLDivElement | null>(null);

const toggleMute = () => {
  chat.mutePlayer(name.value, !isMuted?.value);
};

watch(isActive, newValue => {
  if (newValue) {
    elmRef.value?.scrollIntoView();
  }
});
</script>

<style>
.opponent-wrapper {
  max-width: max(10rem, 20vw);
}

.opponent-name {
  text-overflow: ellipsis;
  overflow: hidden;
}
</style>
