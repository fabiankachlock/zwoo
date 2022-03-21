<template>
  <div class="h-32 py-2 px-3 flex flex-col flex-nowrap items-center overflow-y-auto" style="max-height: 60vh">
    <ChatMessage
      v-for="message in messages"
      :message="message.message"
      :key="message.id"
      :is-own="message.sender.id === auth.username"
      :is-spectator="message.sender.role === ZRPRole.Spectator"
      :name="message.sender.id"
    />
  </div>
</template>

<script setup lang="ts">
import { useAuth } from '@/core/adapter/auth';
import { useChatStore } from '@/core/adapter/play/chat';
import { ZRPRole } from '@/core/services/zrp/zrpTypes';
import { computed } from 'vue';
import ChatMessage from './ChatMessage.vue';

const chat = useChatStore();
const messages = computed(() => chat.allMessages);
const auth = useAuth();
</script>
