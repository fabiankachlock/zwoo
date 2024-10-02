import { defineStore } from 'pinia';
import { computed, ref } from 'vue';

import { useGameEventDispatch } from '@/core/adapter/game/util/useGameEventDispatch';
import { CardDescriptor } from '@/core/domain/cards/CardThemeConfig';
import { Card } from '@/core/domain/game/CardTypes';
import { ZRPOPCode, ZRPPlayerCardAmountPayload, ZRPStateUpdatePayload } from '@/core/domain/zrp/zrpTypes';
import { RouterService } from '@/core/global/Router';
import Logger from '@/core/services/logging/logImport';

import { useGameConfig } from '../game';
import { useGameCardDeck } from './deck';
import { usePlayerManager } from './playerManager';
import { MonolithicEventWatcher } from './util/MonolithicEventWatcher';

export type GamePlayer = {
  id: number;
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
  const activePlayerId = ref<number | undefined>(undefined);
  const currentDrawAmount = ref<number | null>(null);
  const players = ref<Omit<GamePlayer, 'isConnected'>[]>([]);
  const dispatchEvent = useGameEventDispatch();
  const gameConfig = useGameConfig();

  const _receiveMessage: (typeof gameWatcher)['_msgHandler'] = msg => {
    if (msg.code === ZRPOPCode.GameStarted) {
      if (msg.data.pile && msg.data.pile.symbol > 0 && msg.data.pile.type > 0) {
        updatePile({
          type: msg.data.pile.symbol,
          color: msg.data.pile.type
        });
      } else {
        dispatchEvent(ZRPOPCode.RequestPileTop, {});
      }

      if (msg.data.players && msg.data.players.length > 0) {
        updatePlayers(msg.data);
      } else {
        dispatchEvent(ZRPOPCode.RequestPlayerCardAmount, {});
      }
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

  const activateSelf = () => {
    isActivePlayer.value = true;
    activePlayerId.value = gameConfig.lobbyId;
  };

  const deactivateSelf = () => {
    isActivePlayer.value = false;
    activePlayerId.value = undefined;
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
      if (data.cardAmounts[players.value[i].id]) {
        players.value[i].cards = data.cardAmounts[players.value[i].id];
      }
    }
    activePlayerId.value = data.activePlayer;
    currentDrawAmount.value = data.currentDrawAmount ?? null;
    verifyDeck();
  };

  const updatePlayers = (data: ZRPPlayerCardAmountPayload) => {
    let activePlayer: number | undefined;
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
      if (gameConfig.lobbyId === activePlayer) {
        activateSelf();
      }
    }
    verifyDeck();
  };

  const removePlayer = (id: number) => {
    players.value = players.value.filter(p => p.id !== id);
  };

  const verifyDeck = () => {
    // TODO: Optimize this
    if (useGameCardDeck().cards.length !== players.value.find(p => p.id === gameConfig.lobbyId)?.cards) {
      Logger.warn(`local deck didnt match remote state: ${JSON.stringify(useGameCardDeck().cards)}`);
      dispatchEvent(ZRPOPCode.RequestHand, {});
    }
  };

  const reset = () => {
    isActivePlayer.value = false;
    activePlayerId.value = undefined;
    topCard.value = CardDescriptor.BackUpright;
    players.value = [];
    currentDrawAmount.value = null;
  };

  const mappedPlayers = computed<GamePlayer[]>(() =>
    players.value.map(p => ({
      ...p,
      isConnected: playerManager.isPlayerActive(p.id)
    }))
  );

  gameWatcher.onMessage(_receiveMessage);
  gameWatcher.onReset(reset);
  gameWatcher.onClose(reset);

  return {
    topCard,
    isActivePlayer,
    activePlayerId: activePlayerId,
    currentDrawAmount: currentDrawAmount,
    players: mappedPlayers,
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});

const useExample = defineStore('example', () => {
  const a = ref(0);
  const b = computed(() => a.value + 1);

  return {
    a,
    b
  };
});

const exampleStore = useExample();
exampleStore.b;
