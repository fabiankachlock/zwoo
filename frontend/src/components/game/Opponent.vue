<template>
  <div
    :class="{
      'border-secondary': isActive,
      'border-transparent': !isActive,
      'bg-darkest z-0': isConnected,
      'bg-main bc-primary sticky left-0 z-10': !isConnected
    }"
    class="opponent-wrapper px-2 py-1 rounded-sm mx-2 my-1 border"
    :ref="r => (elmRef = r as HTMLDivElement)"
  >
    <div
      :class="{ 'tc-main': isConnected, 'tc-main-secondary line-through': !isConnected }"
      class="flex flex-row flex-nowrap w-full h-full items-center cursor-default overflow-hidden whitespace-nowrap"
    >
      <span class="opponent-name" :class="{ 'tc-primary': isSelf }">
        {{ name }}
      </span>
      <template v-if="id.startsWith('b_')">
        <span class="tc-primary text-lg ml-1">
          <Icon icon="fluent:bot-24-regular" />
        </span>
      </template>
      <span class="whitespace-nowrap ml-3">{{ cardAmount }}</span>
      <span class="ml-2 flex items-center">
        <button v-if="isConnected" @click="toggleMute" class="transition-transform hover:scale-125">
          <Icon v-if="isMuted" icon="bi:mic-mute-fill" />
          <Icon v-else icon="bi:mic-fill" />
        </button>
        <button v-if="isHost && !isConnected" @click="kickPlayer" class="transition-transform hover:scale-110 tc-secondary">
          <Icon icon="akar-icons:cross" />
        </button>
      </span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { defineProps, ref, toRefs, watch } from 'vue';

import { Icon } from '@/components/misc/Icon';
import { useChatStore } from '@/core/adapter/game/chat';
import { useLobbyStore } from '@/core/adapter/game/lobby';
import { useIsHost } from '@/core/adapter/game/util/userRoles';

const chat = useChatStore();
const lobby = useLobbyStore();
const { isHost } = useIsHost();
const props = defineProps<{
  isActive: boolean;
  isSelf: boolean;
  isConnected: boolean;
  id: string;
  name: string;
  cardAmount: number;
  isMuted?: boolean;
}>();

const { name, id, cardAmount, isActive, isMuted } = toRefs(props);
const elmRef = ref<HTMLDivElement | null>(null);

const toggleMute = () => {
  chat.mutePlayer(name.value, !isMuted?.value);
};

const kickPlayer = () => {
  lobby.kickPlayer(id.value);
};

watch(isActive, newValue => {
  if (newValue) {
    elmRef.value?.scrollIntoView();
  }
});
</script>

<style scoped>
.opponent-wrapper {
  max-width: max(10rem, 20vw);
}

.opponent-name {
  text-overflow: ellipsis;
  overflow: hidden;
}
</style>
