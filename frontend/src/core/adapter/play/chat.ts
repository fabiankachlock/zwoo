import { ZRPRole } from '@/core/services/zrp/zrpTypes';
import { defineStore } from 'pinia';

export type ChatMessage = {
  id: number;
  message: string;
  sender: {
    id: string;
    role: ZRPRole;
  };
};

export const useChatStore = defineStore('chat', {
  state: () => ({
    messages: [] as ChatMessage[],
    muted: {} as Record<string, boolean>
  }),

  getters: {
    allMessages(state) {
      return state.messages.filter(msg => !state.muted[msg.sender.id]);
    }
  },

  actions: {
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
});
