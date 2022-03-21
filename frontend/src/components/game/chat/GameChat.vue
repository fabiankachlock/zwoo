<template>
  <div class="h-32 py-2 px-3 flex flex-col flex-nowrap items-center overflow-y-auto" style="max-height: 60vh">
    <ChatMessage v-for="message in messages" :message="message.message" :key="message.id" :is-own="message.sender.id === auth.username" />
  </div>
</template>

<script setup lang="ts">
import { useGameChat } from '@/composables/gameChat';
import { useAuth } from '@/core/adapter/auth';
import ChatMessage from './ChatMessage.vue';

const { messages } = useGameChat();
const auth = useAuth();
</script>
