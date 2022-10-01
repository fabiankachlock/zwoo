<template>
  <div class="mx-auto">
    <div v-if="messages.length === 0">no messages to display</div>
    <div>
      <div :ref="r => container = (r as HTMLDivElement)" class="h-full py-2 px-3 flex flex-col flex-nowrap items-center overflow-y-auto">
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
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuth } from '@/core/adapter/auth';
import { useChatBroadcast } from '@/core/adapter/play/features/chatBroadCast';
import { ZRPRole } from '@/core/services/zrp/zrpTypes';
import { computed, ref, watch } from 'vue';
import ChatMessage from '@/components/game/chat/ChatMessage.vue';

const chat = useChatBroadcast();
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
