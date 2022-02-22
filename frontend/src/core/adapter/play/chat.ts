import { defineStore } from 'pinia';

export type ChatMessage = {
  id: number;
  message: string;
  sender: string;
};

export const useChat = defineStore('chat', {
  state: () => ({
    messages: [] as ChatMessage[],
    muted: {} as Record<string, boolean>
  }),

  getters: {
    allMessages(state) {
      return state.messages.filter(msg => !state.muted[msg.sender]);
    }
  },

  actions: {
    pushMessage(msg: string, from: string) {
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
