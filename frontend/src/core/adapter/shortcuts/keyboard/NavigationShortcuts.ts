import { Shortcut } from '../types';
import Router from '@/router';

export const HomeShortcut: Shortcut<KeyboardEvent> = {
  type: 'keyboard',
  execute(event) {
    if (event.key === 'h') {
      Router.push('/home');
    }
  }
};

export const NewGameShortcut: Shortcut<KeyboardEvent> = {
  type: 'keyboard',
  execute(event) {
    if (event.key === 'n') {
      Router.push('/create-game');
    }
  }
};

export const ListGamesShortcut: Shortcut<KeyboardEvent> = {
  type: 'keyboard',
  execute(event) {
    if (event.key === 'j') {
      Router.push('/available-games');
    }
  }
};

export const SettingsShortcut: Shortcut<KeyboardEvent> = {
  type: 'keyboard',
  execute(event) {
    if (event.key === 's') {
      Router.push('/settings');
    }
  }
};

export const LeaderboardShortcut: Shortcut<KeyboardEvent> = {
  type: 'keyboard',
  execute(event) {
    if (event.key === 'l') {
      Router.push('/leaderboard');
    }
  }
};

export const LoginShortcut: Shortcut<KeyboardEvent> = {
  type: 'keyboard',
  execute(event) {
    if (event.key === 'a') {
      Router.push('/login');
    }
  }
};

export const InfoShortcut: Shortcut<KeyboardEvent> = {
  type: 'keyboard',
  execute(event) {
    if (event.key === 'i' || event.key === '?') {
      Router.push('/shortcut-info');
    }
  }
};
