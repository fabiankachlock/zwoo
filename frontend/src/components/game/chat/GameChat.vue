<template>
  <div
    :ref="r => container = (r as HTMLDivElement)"
    class="h-32 py-2 px-3 flex flex-col flex-nowrap items-center overflow-y-auto"
    style="max-height: 60vh"
    v-auto-animate
  >
    <ChatMessage
      v-for="message in messages"
      :message="message.message"
      :key="message.id"
      :is-own="message.sender.id === auth.username"
      :is-spectator="message.sender.role === ZRPRole.Spectator"
      :is-host="message.sender.role === ZRPRole.Host"
      :name="message.sender.id"
    />
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';

import { useAuth } from '@/core/adapter/auth';
import { useChatStore } from '@/core/adapter/play/chat';
import { ZRPRole } from '@/core/services/zrp/zrpTypes';

import ChatMessage from './ChatMessage.vue';

const chat = useChatStore();
const messages = computed(() => chat.allMessages);
const auth = useAuth();
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
