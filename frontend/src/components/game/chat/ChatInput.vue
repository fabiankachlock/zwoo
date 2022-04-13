<template>
  <div class="py-1 px-3 flex flex-row flex-nowrap justify-between items-stretch">
    <div class="w-full mr-2">
      <input
        class="appearance-none outline-none w-full bg-light tc-main-light px-2 py-0.5 rounded transition hover:bg-main focus:bg-main border border-transparent focus:bc-primary ring-0"
        type="text"
        v-model="message"
        @keyup.enter="sendMessage"
      />
    </div>
    <div class="flex justify-center items-center bg-light px-3 py-0.5 rounded transition hover:bg-main">
      <button @click="sendMessage" class="tc-primary">
        <Icon icon="teenyicons:send-outline" />
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { Icon } from '@iconify/vue';
import { useChatStore } from '@/core/adapter/play/chat';

const message = ref('');
const gameChat = useChatStore();

const sendMessage = () => {
  if (message.value.length === 0) return;
  gameChat.sendChatMessage(message.value);
  message.value = '';
};
</script>
