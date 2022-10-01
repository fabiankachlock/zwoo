<template>
  <div v-if="!isActive" class="mx-auto px-2 max-w-xl">
    <p class="text-xl tc-main text-center m-2">no active game</p>
  </div>
  <div v-else>
    <div class="bg-darkest m-2 rounded-lg p-1 flex flex-row justify-between items-center">
      <p class="ml-2 tc-main text-xl" style="text-overflow: ellipsis; overflow: hidden">{{ gameName }}</p>
    </div>
    <div v-if="messages.length > 0" class="mx-auto px-2 max-w-xl">
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
    <div v-else class="mx-auto px-2 max-w-xl">
      <p class="text-xl tc-main text-center m-2">No messages to dispalay</p>
    </div>
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
const gameName = computed(() => chat.gameName);
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
