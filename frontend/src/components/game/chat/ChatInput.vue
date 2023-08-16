<template>
  <div class="py-1 px-3 flex flex-row flex-nowrap justify-between items-stretch">
    <div class="w-full mr-2">
      <input
        v-model="message"
        class="appearance-none outline-none w-full bg-light tc-main-light px-2 py-0.5 rounded transition hover:bg-main focus:bg-main border border-transparent focus:bc-primary ring-0"
        type="text"
        @keyup.stop
        @keyup.enter="sendMessage"
      />
    </div>
    <div class="flex justify-center items-center bg-light rounded transition hover:bg-main">
      <button class="block tc-primary px-3 py-0.5 h-full" @click="sendMessage">
        <Icon icon="teenyicons:send-outline" />
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';

import { Icon } from '@/components/misc/Icon';
import { useChatStore } from '@/core/adapter/game/chat';

const message = ref('');
const gameChat = useChatStore();

const sendMessage = () => {
  const chatMessage = (message.value || '').trim();
  if (chatMessage.length === 0) return;
  gameChat.sendChatMessage(message.value);
  message.value = '';
};
</script>
