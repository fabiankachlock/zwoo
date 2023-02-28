import { defineStore } from 'pinia';
import { computed, ref } from 'vue';

import { useGameEventDispatch } from '@/core/adapter/game/util/useGameEventDispatch';
import { CardDescriptor } from '@/core/domain/cards/CardThemeConfig';
import { Card } from '@/core/domain/game/CardTypes';
import { ZRPOPCode, ZRPPlayerCardAmountPayload, ZRPStateUpdatePayload } from '@/core/domain/zrp/zrpTypes';
import { RouterService } from '@/core/global/Router';
import Logger from '@/core/services/logging/logImport';

import { useAuth } from '../auth';
import { useGameCardDeck } from './deck';
import { usePlayerManager } from './playerManager';
import { MonolithicEventWatcher } from './util/MonolithicEventWatcher';

export type GamePlayer = {
  id: string;
  name: string;
  cards: number;
  order: number;
  isConnected: boolean;
};

const gameWatcher = new MonolithicEventWatcher(
  ZRPOPCode.GameStarted,
  ZRPOPCode.StartTurn,
  ZRPOPCode.EndTurn,
  ZRPOPCode.StateUpdate,
  ZRPOPCode.GetPlayerCardAmount,
  ZRPOPCode.GetPileTop,
  ZRPOPCode.PlayerWon,
  ZRPOPCode.PlayerLeft,
  ZRPOPCode.PlayerReconnected,
  ZRPOPCode.PlayerDisconnected
);

export const useGameState = defineStore('game-state', () => {
  const isActivePlayer = ref(false);
  const playerManager = usePlayerManager();
  const topCard = ref<Card | CardDescriptor>(CardDescriptor.BackUpright);
  const activePlayerId = ref('');
  const players = ref<Omit<GamePlayer, 'isConnected'>[]>([]);
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
      RouterService.getRouter().replace('/game/summary');
    } else if (msg.code == ZRPOPCode.PlayerLeft) {
      removePlayer(msg.data.id);
    } else if (msg.code === ZRPOPCode.PlayerDisconnected) {
      playerManager.setPlayerDisconnected(msg.data.id);
    } else if (msg.code === ZRPOPCode.PlayerReconnected) {
      playerManager.setPlayerConnected(msg.data.id);
    }
  };

  const initialSetup = () => {
    dispatchEvent(ZRPOPCode.RequestPileTop, {});
    dispatchEvent(ZRPOPCode.RequestPlayerCardAmount, {});
  };

  const activateSelf = () => {
    isActivePlayer.value = true;
    activePlayerId.value = auth.publicId;
  };

  const deactivateSelf = () => {
    isActivePlayer.value = false;
    activePlayerId.value = '';
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
      if (players.value[i].id === data.activePlayer) {
        players.value[i].cards = data.activePlayerCardAmount;
      }
      if (players.value[i].id === data.lastPlayer) {
        players.value[i].cards = data.lastPlayerCardAmount;
      }
    }
    activePlayerId.value = data.activePlayer;
    verifyDeck();
  };

  const updatePlayers = (data: ZRPPlayerCardAmountPayload) => {
    let activePlayer: string | undefined;
    players.value = data.players
      .map(p => {
        if (p.isActivePlayer) {
          activePlayer = p.id;
        }
        return {
          id: p.id,
          name: p.username,
          cards: p.cards,
          order: p.order
        };
      })
      .sort((a, b) => a.order - b.order);

    if (activePlayer) {
      activePlayerId.value = activePlayer;
      if (auth.publicId === activePlayer) {
        activateSelf();
      }
    }
    verifyDeck();
  };

  const removePlayer = (id: string) => {
    players.value = players.value.filter(p => p.id !== id);
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
    activePlayerId.value = '';
    topCard.value = CardDescriptor.BackUpright;
    players.value = [];
  };

  gameWatcher.onMessage(_receiveMessage);
  gameWatcher.onReset(reset);
  gameWatcher.onClose(reset);

  return {
    topCard,
    isActivePlayer,
    activePlayerId: activePlayerId,
    players: computed<GamePlayer[]>(() =>
      players.value.map(p => ({
        ...p,
        isConnected: playerManager.isPlayerActive(p.id)
      }))
    ),
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
