import { useGameEventDispatch } from '@/composables/eventDispatch';
import { CardDescriptor } from '@/core/services/cards/CardThemeConfig';
import { Card } from '@/core/services/game/card';
import Logger from '@/core/services/logging/logImport';
import { ZRPOPCode, ZRPPlayerCardAmountPayload, ZRPStateUpdatePayload } from '@/core/services/zrp/zrpTypes';
import router from '@/router';
import { defineStore } from 'pinia';
import { ref } from 'vue';
import { useAuth } from '../auth';
import { useGameCardDeck } from './deck';
import { MonolithicEventWatcher } from './util/MonolithicEventWatcher';

export type GamePlayer = {
  name: string;
  cards: number;
  order: number;
  state: 'disconnected' | 'connected';
};

const gameWatcher = new MonolithicEventWatcher(
  ZRPOPCode.GameStarted,
  ZRPOPCode.StartTurn,
  ZRPOPCode.EndTurn,
  ZRPOPCode.StateUpdate,
  ZRPOPCode.GetPlayerCardAmount,
  ZRPOPCode.GetPileTop,
  ZRPOPCode.PlayerWon,
  ZRPOPCode.PlayerLeft
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
      dispatchEvent(ZRPOPCode._ResetState, {});
    } else if (msg.code == ZRPOPCode.PlayerLeft) {
      removePlayer(msg.data.username);
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
    verifyDeck();
  };

  const updatePlayers = (data: ZRPPlayerCardAmountPayload) => {
    let activePlayer: string | undefined;
    players.value = data.players
      .map(p => {
        if (p.isActivePlayer) {
          activePlayer = p.username;
        }
        return {
          name: p.username,
          cards: p.cards,
          order: p.order,
          state: players.value.find(pp => pp.name === p.username)?.state ?? 'connected'
        };
      })
      .sort((a, b) => a.order - b.order);

    if (activePlayer) {
      activePlayerName.value = activePlayer;
      if (auth.username === activePlayer) {
        activateSelf();
      }
    }
    verifyDeck();
  };

  const removePlayer = (playerToDelete: string) => {
    players.value = players.value.filter(p => p.name !== playerToDelete);
  };

  const verifyDeck = () => {
    // TODO: Optimize this
    if (useGameCardDeck().cards.length !== players.value.find(p => p.name === auth.username)?.cards) {
      Logger.warn(`local deck didnt match remote state: ${JSON.stringify(useGameCardDeck().cards)}`);
      dispatchEvent(ZRPOPCode.RequestHand, {});
    }
  };

  const reset = () => {
    isActivePlayer.value = false;
    activePlayerName.value = '';
    topCard.value = CardDescriptor.BackUpright;
    players.value = [];
  };

  gameWatcher.onMessage(_receiveMessage);
  gameWatcher.onReset(reset);
  gameWatcher.onClose(reset);

  return {
    topCard,
    isActivePlayer,
    activePlayerName,
    players,
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
