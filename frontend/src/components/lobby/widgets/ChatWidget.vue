<template>
  <Widget v-model="isOpen" title="wait.chat">
    <div class="bg-bg rounded-md pb-2">
      <GameChat />
      <ChatInput />
    </div>
  </Widget>
</template>

<script setup lang="ts">
import ChatInput from '@/components/game/chat/ChatInput.vue';
import GameChat from '@/components/game/chat/GameChat.vue';
import { useUserDefaults } from '@/composables/userDefaults';

import Widget from '../Widget.vue';

const isOpen = useUserDefaults('lobby:widgetChatOpen', false);
</script>
