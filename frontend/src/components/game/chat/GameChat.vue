<template>
  <div class="h-32 py-2 px-3 flex flex-col flex-nowrap items-center overflow-y-auto" style="max-height: 60vh">
    <ChatMessage v-for="message in messages" :message="message.message" :key="message.id" :is-own="message.sender === 'me'" />
  </div>
</template>

<script setup lang="ts">
import { useChat } from '@/core/adapter/play/chat';
import { computed } from 'vue';
import ChatMessage from './ChatMessage.vue';

const chat = useChat();
const messages = computed(() => chat.allMessages);
</script>
