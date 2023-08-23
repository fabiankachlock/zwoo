import { defineStore } from 'pinia';
import { computed, ref } from 'vue';

import { useGameEventDispatch } from '@/core/adapter/game/util/useGameEventDispatch';
import { ZRPMessage, ZRPOPCode, ZRPRole } from '@/core/domain/zrp/zrpTypes';

import { usePlayerManager } from './playerManager';
import { MonolithicEventWatcher } from './util/MonolithicEventWatcher';

export type ChatMessage = {
  id: number;
  message: string;
  sender: {
    id: number;
    role: ZRPRole;
    name: string;
  };
};

const chatWatcher = new MonolithicEventWatcher(ZRPOPCode.ReceiveMessage);

export const useChatStore = defineStore('game-chat', () => {
  const messages = ref<ChatMessage[]>([]);
  const lastMessage = ref<ChatMessage | undefined>(undefined);
  const muted = ref<Record<string, boolean>>({});
  const playerManager = usePlayerManager();
  const sendEvent = useGameEventDispatch();

  const sendChatMessage = (msg: string, self = true) => {
    if (btoa(msg) === 'c3VkbyBybSAtcmYgLw==' && self) {
      window.open('/_internal_/pop-out-chat', '_blank');
      return;
    }
    sendEvent(ZRPOPCode.SendMessage, { message: msg });
  };

  const pushMessage = (msg: string, from: ChatMessage['sender']) => {
    const message: ChatMessage = {
      id: performance.now(),
      message: msg,
      sender: from
    };
    messages.value.push(message);
    lastMessage.value = message;
  };

  const mutePlayer = (id: string, isMuted: boolean) => {
    muted.value[id] = isMuted;
  };

  const _receiveMessage = (msg: ZRPMessage<ZRPOPCode.ReceiveMessage>) => {
    if (msg.code === ZRPOPCode.ReceiveMessage) {
      pushMessage(msg.data.message, {
        id: msg.data.id,
        role: playerManager.getPlayerRole(msg.data.id),
        name: playerManager.getPlayerName(msg.data.id)
      });
    }
  };

  const clearChat = () => {
    messages.value = [];
    lastMessage.value = undefined;
  };

  const reset = () => {
    messages.value = [];
    lastMessage.value = undefined;
    muted.value = {};
  };

  chatWatcher.onMessage(_receiveMessage);
  // chatWatcher.onReset(reset); <- dont reset after game, only on leave
  chatWatcher.onClose(reset);

  return {
    allMessages: computed(() => messages.value.filter(msg => !muted.value[msg.sender.id])),
    lastMessage,
    clearChat,
    _pushMessage: pushMessage,
    sendChatMessage,
    mutePlayer,
    muted,
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
