import { useGameEventDispatch } from '@/composables/eventDispatch';
import { CardDescriptor } from '@/core/services/cards/CardThemeConfig';
import { Card } from '@/core/services/game/card';
import { ZRPOPCode, ZRPPlayerCardAmountPayload, ZRPStateUpdatePayload } from '@/core/services/zrp/zrpTypes';
import router from '@/router';
import { defineStore } from 'pinia';
import { ref } from 'vue';
import { useAuth } from '../auth';
import { MonolithicEventWatcher } from './util/MonolithicEventWatcher';

export type GamePlayer = {
  name: string;
  cards: number;
};

const gameWatcher = new MonolithicEventWatcher(
  ZRPOPCode.GameStarted,
  ZRPOPCode.StartTurn,
  ZRPOPCode.EndTurn,
  ZRPOPCode.StateUpdate,
  ZRPOPCode.GetPlayerCardAmount,
  ZRPOPCode.GetPileTop,
  ZRPOPCode.PlayerWon
);

export const useGameState = defineStore('game-state', () => {
  const isActivePlayer = ref(false);
  const topCard = ref<Card | CardDescriptor>(CardDescriptor.BackUpright);
  const activePlayerName = ref('');
  const players = ref<GamePlayer[]>([]);
  const dispatchEvent = useGameEventDispatch();
  const auth = useAuth();

  const _receiveMessage: typeof gameWatcher['_msgHandler'] = msg => {
    if (msg.code === ZRPOPCode.GameStarted) {
      initialSetup();
    } else if (msg.code === ZRPOPCode.StartTurn) {
      activateSelf();
    } else if (msg.code === ZRPOPCode.EndTurn) {
      deactivateSelf();
    } else if (msg.code === ZRPOPCode.GetPileTop) {
      updatePile({
        type: msg.data.symbol,
        color: msg.data.type
      });
    } else if (msg.code === ZRPOPCode.GetPlayerCardAmount) {
      updatePlayers(msg.data);
    } else if (msg.code === ZRPOPCode.StateUpdate) {
      updateGame(msg.data);
    } else if (msg.code == ZRPOPCode.PlayerWon) {
      router.replace('/game/summary');
    }
  };

  const initialSetup = () => {
    dispatchEvent(ZRPOPCode.RequestPileTop, {});
    dispatchEvent(ZRPOPCode.RequestPlayerCardAmount, {});
  };

  const activateSelf = () => {
    isActivePlayer.value = true;
    activePlayerName.value = auth.username;
  };

  const deactivateSelf = () => {
    isActivePlayer.value = false;
    activePlayerName.value = '';
  };

  const updatePile = (card: Card) => {
    topCard.value = card;
  };

  const updateGame = (data: ZRPStateUpdatePayload) => {
    updatePile({
      color: data.pileTop.type,
      type: data.pileTop.symbol
    });
    for (let i = 0; i < players.value.length; i++) {
      if (players.value[i].name === data.activePlayer) {
        players.value[i].cards = data.activePlayerCardAmount;
      }
      if (players.value[i].name === data.lastPlayer) {
        players.value[i].cards = data.lastPlayerCardAmount;
      }
    }
    activePlayerName.value = data.activePlayer;
  };

  const updatePlayers = (data: ZRPPlayerCardAmountPayload) => {
    let activePlayer: string | undefined;
    players.value = data.players.map(p => {
      if (p.isActivePlayer) {
        activePlayer = p.username;
      }
      return {
        name: p.username,
        cards: p.cards
      };
    });

    if (activePlayer) {
      activePlayerName.value = activePlayer;
      if (auth.username === activePlayer) {
        activateSelf();
      }
    }
  };

  gameWatcher.onMessage(_receiveMessage);
  gameWatcher.onClose(() => {
    isActivePlayer.value = false;
    activePlayerName.value = '';
    topCard.value = CardDescriptor.BackUpright;
    players.value = [];
  });

  return {
    topCard,
    isActivePlayer,
    activePlayerName,
    players,
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
