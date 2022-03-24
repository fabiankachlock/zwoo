<template>
  <Widget title="wait.chat" widget-class="bg-light" button-class="bg-main hover:bg-dark">
    <div class="bg-darkest rounded-md pb-2">
      <GameChat />
      <ChatInput />
    </div>
  </Widget>
</template>

<script setup lang="ts">
import Widget from '../Widget.vue';
import GameChat from '@/components/game/chat/GameChat.vue';
import ChatInput from '@/components/game/chat/ChatInput.vue';
</script>
