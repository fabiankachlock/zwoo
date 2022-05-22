import { useGameEventDispatch } from '@/composables/eventDispatch';
import { ZRPMessage, ZRPOPCode, ZRPRole } from '@/core/services/zrp/zrpTypes';
import { defineStore } from 'pinia';
import { MonolithicEventWatcher } from './util/MonolithicEventWatcher';
import { computed, ref } from 'vue';

export type ChatMessage = {
  id: number;
  message: string;
  sender: {
    id: string;
    role: ZRPRole;
  };
};

const chatWatcher = new MonolithicEventWatcher(ZRPOPCode.ReceiveMessage);

export const useChatStore = defineStore('game-chat', () => {
  const messages = ref<ChatMessage[]>([]);
  const muted = ref<Record<string, boolean>>({});
  const sendEvent = useGameEventDispatch();

  const sendChatMessage = (msg: string) => {
    sendEvent(ZRPOPCode.SendMessage, { message: msg });
  };

  const pushMessage = (msg: string, from: ChatMessage['sender']) => {
    messages.value.push({
      id: performance.now(),
      message: msg,
      sender: from
    });
  };

  const mutePlayer = (id: string, isMuted: boolean) => {
    muted.value[id] = isMuted;
  };

  const _receiveMessage = (msg: ZRPMessage<ZRPOPCode.ReceiveMessage>) => {
    if (msg.code === ZRPOPCode.ReceiveMessage) {
      pushMessage(msg.data.message, {
        id: msg.data.username,
        role: msg.data.role
      });
    }
  };

  chatWatcher.onMessage(_receiveMessage);

  chatWatcher.onClose(() => {
    messages.value = [];
  });

  return {
    allMessages: computed(() => messages.value.filter(msg => !muted.value[msg.sender.id])),
    sendChatMessage,
    mutePlayer,
    muted,
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
