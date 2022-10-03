<template>
  <div v-if="!isActive" class="mx-auto px-2 max-w-xl">
    <p class="text-xl tc-main text-center m-2">no active game</p>
  </div>
  <div v-else>
    <div class="pop-out-grid">
      <div class="bg-darkest m-2 mb-0 rounded-lg p-1 flex flex-row justify-between items-center">
        <p class="ml-2 tc-main text-xl" style="text-overflow: ellipsis; overflow: hidden">{{ gameName }}</p>
      </div>
      <div v-if="messages.length > 0" class="mx-auto px-2 max-w-xl overflow-y-auto w-full">
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
      <div>
        <div class="py-1 px-3 flex flex-row flex-nowrap justify-between items-stretch">
          <div class="w-full mr-2">
            <input
              class="appearance-none outline-none w-full bg-dark tc-main-light px-2 py-0.5 rounded transition hover:bg-darkest focus:bg-darkest border border-transparent focus:bc-primary ring-0"
              type="text"
              v-model="message"
              @keyup.stop
              @keyup.enter="sendMessage"
            />
          </div>
          <div @click="sendMessage" class="flex justify-center items-center bg-dark px-3 py-0.5 rounded transition hover:bg-darkest cursor-pointer">
            <button class="tc-primary">
              <Icon icon="teenyicons:send-outline" />
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useChatBroadcast } from '@/core/adapter/play/features/chatBroadcast';
import { ZRPRole } from '@/core/services/zrp/zrpTypes';
import { computed, onMounted, ref, watch } from 'vue';
import ChatMessage from '@/components/game/chat/ChatMessage.vue';
import { Icon } from '@iconify/vue';

const chat = useChatBroadcast();
const message = ref('');
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

const sendMessage = () => {
  const chatMessage = (message.value || '').trim();
  if (chatMessage.length === 0) return;
  chat.sendMessage(message.value);
  message.value = '';
};
</script>

<style>
.pop-out-grid {
  @apply grid h-full w-full max-w-full;
  grid-template-rows: min-content auto min-content;
}
</style>
