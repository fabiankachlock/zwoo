<template>
  <div
    :ref="r => container = (r as HTMLDivElement)"
    class="h-32 py-2 px-3 flex flex-col flex-nowrap items-center overflow-y-auto"
    style="max-height: 60vh"
  >
    <ChatMessage
      v-for="message in messages"
      :key="message.id"
      :message="message.message"
      :is-own="message.sender.id === lobbyId"
      :is-spectator="message.sender.role === ZRPRole.Spectator"
      :is-host="message.sender.role === ZRPRole.Host"
      :is-system="message.sender.role === ZRPRole._System"
      :name="message.sender.name"
    />
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';

import { useGameConfig } from '@/core/adapter/game';
import { useChatStore } from '@/core/adapter/game/chat';
import { ZRPRole } from '@/core/domain/zrp/zrpTypes';

import ChatMessage from './ChatMessage.vue';

const chat = useChatStore();
const messages = computed(() => chat.allMessages);
const gameConfig = useGameConfig();
const lobbyId = computed(() => gameConfig.lobbyId);
const container = ref<HTMLDivElement | undefined>(undefined);

watch(messages, () => {
  setTimeout(() => {
    // 'wait for' DOM update before scroll to bottom
    scrollToBottom();
  }, 0);
});

const scrollToBottom = () => {
  if (container.value) {
    container.value.scrollTo({
      top: container.value.scrollHeight,
      behavior: 'smooth'
    });
  }
};
</script>
