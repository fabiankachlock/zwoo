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
        id: msg.data.name,
        role: msg.data.role
      });
    }
  };

  chatWatcher.onMessage(_receiveMessage);

  chatWatcher.onClose(() => {
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    useChatStore().$reset();
  });

  /*
  state: () => {
    return {
      messages: [] as ChatMessage[],
      muted: {} as Record<string, boolean>
    };
  },

  getters: {
    allMessages(state) {
      return state.messages.filter(msg => !state.muted[msg.sender.id]);
    }
  },

  actions: {
    _receiveMessage(msg: ZRPMessage<ZRPOPCode.ReceiveMessage>) {
      if (msg.code === ZRPOPCode.ReceiveMessage) {
        this.pushMessage(msg.data.message, {
          id: msg.data.name,
          role: msg.data.role
        });
      }
    },
    sendChatMessage(msg: string) {
      useGameEventDispatch()(ZRPOPCode.SendMessage, { message: msg });
    },
    pushMessage(msg: string, from: ChatMessage['sender']) {
      this.messages.push({
        id: performance.now(),
        message: msg,
        sender: from
      });
    },
    mutePlayer(id: string, isMuted: boolean) {
      this.muted[id] = isMuted;
    }
  }

*/

  return {
    allMessages: computed(() => messages.value.filter(msg => !muted.value[msg.sender.id])),
    sendChatMessage,
    mutePlayer,
    muted
  };
});
