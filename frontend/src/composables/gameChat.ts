import { useChatStore } from '@/core/adapter/play/chat';
import { useWatchGameEvents } from '@/core/adapter/play/util/gameEventWatcher';
import { createZRPOPCodeMatcher } from '@/core/adapter/play/util/zrpMatcher';
import { ZRPOPCode } from '@/core/services/zrp/zrpTypes';
import { useGameEventDispatch } from './eventDispatch';

export const useGameChat = () => {
  const store = useChatStore();
  const sendMessage = useGameEventDispatch();

  useWatchGameEvents<ZRPOPCode.ReceiveMessage>(createZRPOPCodeMatcher(ZRPOPCode.ReceiveMessage), msg => {
    if (msg.code === ZRPOPCode.ReceiveMessage) {
      store.pushMessage(msg.data.message, {
        id: msg.data.name,
        role: msg.data.role
      });
    }
  });

  const sendChatMessage = (msg: string) => {
    sendMessage(ZRPOPCode.SendMessage, { message: msg });
  };

  return {
    sendChatMessage,
    messages: store.allMessages
  };
};
