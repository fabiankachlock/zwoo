import { defineStore } from 'pinia';
import { ref, watch } from 'vue';
import { ChatMessage, useChatStore } from '../chat';

const BroadcastChannelID = 'zwoo:$gameChat';

export const useChatBroadcast = defineStore('chat-broadcast', () => {
  const chatStore = useChatStore();
  const channel = new BroadcastChannel(BroadcastChannelID);
  const messages = ref<ChatMessage[]>([]);

  watch(
    () => chatStore.lastMessage,
    newMessage => {
      console.log('posting message');
      channel.postMessage(JSON.stringify(newMessage) ?? '');
    }
  );

  channel.addEventListener('message', message => {
    console.log('receiving message', message.data);
    if (message.data) {
      messages.value.push(JSON.parse(message.data) as ChatMessage);
    } else {
      // undefined message === clear
      messages.value = [];
    }
  });

  return {
    allMessages: messages,
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
