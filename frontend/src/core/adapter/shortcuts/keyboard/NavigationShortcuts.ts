import { RouterService } from '@/core/services/global/Router';

import { Shortcut } from '../types';

export const HomeShortcut: Shortcut<KeyboardEvent> = {
  id: 'navigate-home',
  type: 'keyboard',
  keyCombination: 'h',
  execute(event) {
    if (event.key === 'h') {
      RouterService.getRouter().push('/home');
    }
  }
};

export const NewGameShortcut: Shortcut<KeyboardEvent> = {
  id: 'new-game',
  type: 'keyboard',
  keyCombination: 'n',
  execute(event) {
    if (event.key === 'n') {
      RouterService.getRouter().push('/create-game');
    }
  }
};

export const ListGamesShortcut: Shortcut<KeyboardEvent> = {
  id: 'list-games',
  type: 'keyboard',
  keyCombination: 'j',
  execute(event) {
    if (event.key === 'j') {
      RouterService.getRouter().push('/available-games');
    }
  }
};

export const SettingsShortcut: Shortcut<KeyboardEvent> = {
  id: 'navigate-settings',
  type: 'keyboard',
  keyCombination: 's',
  execute(event) {
    if (event.key === 's') {
      RouterService.getRouter().push('/settings');
    }
  }
};

export const LeaderboardShortcut: Shortcut<KeyboardEvent> = {
  id: 'navigate-leaderboard',
  type: 'keyboard',
  keyCombination: 'l',
  execute(event) {
    if (event.key === 'l') {
      RouterService.getRouter().push('/leaderboard');
    }
  }
};

export const LoginShortcut: Shortcut<KeyboardEvent> = {
  id: 'navigate-login',
  type: 'keyboard',
  keyCombination: 'a',
  execute(event) {
    if (event.key === 'a') {
      RouterService.getRouter().push('/login');
    }
  }
};

export const InfoShortcut: Shortcut<KeyboardEvent> = {
  id: 'show-info',
  type: 'keyboard',
  keyCombination: 'i',
  execute(event) {
    if (event.key === 'i' || event.key === '?') {
      RouterService.getRouter().push('/shortcut-info');
    }
  }
};
