<template>
  <div class="mx-auto px-2 max-w-xl">
    <div v-if="!isActive">no active game</div>
    <div v-if="isActive && messages.length > 0">
      <div :ref="r => container = (r as HTMLDivElement)" class="h-full py-2 px-3 flex flex-col flex-nowrap items-center overflow-y-auto">
        <ChatMessage
          v-for="message in messages"
          :message="message.message"
          :key="message.id"
          :is-own="message.sender.id === username"
          :is-spectator="message.sender.role === ZRPRole.Spectator"
          :is-host="message.sender.role === ZRPRole.Host"
          :name="message.sender.id"
        />
      </div>
    </div>
    <div v-if="isActive && messages.length === 0">No messages to dispalay</div>
  </div>
</template>

<script setup lang="ts">
import { useChatBroadcast } from '@/core/adapter/play/features/chatBroadCast';
import { ZRPRole } from '@/core/services/zrp/zrpTypes';
import { computed, onMounted, ref, watch } from 'vue';
import ChatMessage from '@/components/game/chat/ChatMessage.vue';

const chat = useChatBroadcast();
const messages = computed(() => chat.allMessages);
const isActive = computed(() => chat.isActive);
const username = computed(() => chat.ownName);
const container = ref<HTMLDivElement | undefined>(undefined);

watch(messages, () => {
  setTimeout(() => {
    // 'wait for' DOM update before scroll to bottom
    scrollToBottom();
  }, 0);
});

onMounted(() => {
  chat.requireSetup();
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
