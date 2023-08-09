import { defineStore } from 'pinia';
import { ref, watch } from 'vue';

import { ZRPOPCode } from '@/core/domain/zrp/zrpTypes';

import { useGameConfig } from '../../game';
import { ChatMessage as ChatMessageType, useChatStore } from '../chat';
import { MonolithicEventWatcher } from '../util/MonolithicEventWatcher';

const BroadcastChannelID = 'zwoo:$gameChat';
const ResetMessage = '$recv:reset';
const SetupMessage = '$recv:setup';
const RequestSetupMessage = '$send:setup';
const ChatMessage = '$recv:chat';
const RequestSendMessage = '$send:send';

type SetupPayload = {
  ownId: number;
  hasActiveGame: boolean;
  gameName?: string;
};

const chatBroadcastWatcher = new MonolithicEventWatcher(ZRPOPCode.PlayerWon);

export const useChatBroadcast = defineStore('chat-broadcast', () => {
  const chatStore = useChatStore();
  const gameConfigStore = useGameConfig();
  const channel = new BroadcastChannel(BroadcastChannelID);

  const messages = ref<ChatMessageType[]>([]);
  const isActive = ref(false);
  const gameName = ref('');
  const ownId = ref(0);
  let isPublisher = true;

  /*
    PUBLISHER
    - runs in 'main' zwoo windows
    - pushes events to the pop-out window
  */
  // message updates
  watch(
    () => chatStore.lastMessage,
    newMessage => {
      if (newMessage) {
        channel.postMessage(`${ChatMessage}${JSON.stringify(newMessage) ?? ''}`);
      }
    }
  );

  const createSetupPayload = (): SetupPayload => ({
    hasActiveGame: gameConfigStore.inActiveGame,
    gameName: gameConfigStore.inActiveGame ? gameConfigStore.name : undefined,
    ownId: gameConfigStore.lobbyId ?? 0
  });

  // ZRP Events
  chatBroadcastWatcher.onMessage(msg => {
    if (msg.code === ZRPOPCode.PlayerWon) {
      // reset chat
      channel.postMessage(ResetMessage);
    }
  });
  chatBroadcastWatcher.onOpen(() => channel.postMessage(`${SetupMessage}${JSON.stringify(createSetupPayload())}`));
  chatBroadcastWatcher.onReset(() => channel.postMessage(ResetMessage));
  chatBroadcastWatcher.onClose(() => channel.postMessage(ResetMessage));

  /*
    LISTENER
    - runs in the pop-out windows
    - receives events from the main window
  */
  channel.addEventListener('message', message => {
    if (!message.data) {
      return;
    }

    try {
      const msg = (message.data || '') as string;
      console.log(msg);
      if (msg.startsWith(ResetMessage)) {
        // reset pop-out
        isActive.value = false;
        messages.value = [];
      } else if (msg.startsWith(SetupMessage)) {
        // setup pop-out
        const payload = JSON.parse(msg.substring(SetupMessage.length)) as SetupPayload;
        isActive.value = payload.hasActiveGame;
        gameName.value = payload.gameName ?? '';
        ownId.value = payload.ownId;
      } else if (msg.startsWith(ChatMessage)) {
        // add message to pop-out
        const payload = JSON.parse(msg.substring(ChatMessage.length)) as ChatMessageType;
        messages.value.push(payload);
      } else if (msg.startsWith(RequestSetupMessage) && isPublisher) {
        // START: PUBLISHER-SITE
        // request setup
        channel.postMessage(`${SetupMessage}${JSON.stringify(createSetupPayload())}`);
      } else if (msg.startsWith(RequestSendMessage) && isPublisher) {
        // send message
        const payload = msg.substring(RequestSendMessage.length);
        chatStore.sendChatMessage(payload, false); // don't open popup from host window triggered by this window
        // END: PUBLISHER-SITE
      }
    } catch {
      // ignore
    }
  });

  const requireSetup = () => {
    isPublisher = false;
    channel.postMessage(RequestSetupMessage);
  };

  const sendMessage = (message: string) => {
    channel.postMessage(`${RequestSendMessage}${message}`);
  };

  return {
    allMessages: messages,
    gameName,
    ownId,
    isActive,
    requireSetup,
    sendMessage,
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
