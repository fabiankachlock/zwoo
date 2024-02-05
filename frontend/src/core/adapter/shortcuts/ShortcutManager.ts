import { RouteLocationNormalized } from 'vue-router';

import { RouterInterceptor } from '@/router/types';

import {
  HomeShortcut,
  InfoShortcut,
  LeaderboardShortcut,
  ListGamesShortcut,
  LoginShortcut,
  NewGameShortcut,
  SettingsShortcut
} from './keyboard/NavigationShortcuts';
import { FullScreenShortcut, QuickMenuShortcut, ToggleDarkModeShortcut } from './keyboard/UIShortcuts';
import { KeyboardShortcuts } from './KeyboardShortcuts';
import { Shortcut } from './types';

const AllShortcuts = [
  HomeShortcut,
  NewGameShortcut,
  ListGamesShortcut,
  SettingsShortcut,
  LeaderboardShortcut,
  ToggleDarkModeShortcut,
  FullScreenShortcut,
  LoginShortcut,
  QuickMenuShortcut,
  InfoShortcut
];

const WhiteLists: Record<string, Shortcut<Event>[]> = {
  '/game': [ToggleDarkModeShortcut, FullScreenShortcut]
};

const BlackLists: Record<string, Shortcut<Event>[]> = {};

export class ShortcutManager implements RouterInterceptor {
  private keyboardManager: KeyboardShortcuts;

  static global = new ShortcutManager();

  private constructor() {
    this.keyboardManager = new KeyboardShortcuts([]);
  }

  afterEachAsync = (_from: RouteLocationNormalized, current: RouteLocationNormalized) => {
    this.updatedRoute(current.fullPath);
  };

  private updatedRoute(path: string) {
    const whitelist = WhiteLists[Object.keys(WhiteLists).find(rule => path.startsWith(rule)) ?? ''] ?? [];
    const blacklist = BlackLists[Object.keys(BlackLists).find(rule => path.startsWith(rule)) ?? ''] ?? [];
    const allowedShortcuts =
      blacklist.length > 0 && whitelist.length > 0
        ? AllShortcuts.filter(shortcut => !!whitelist.find(s => s.id === shortcut.id)).filter(shortcut => !blacklist.find(s => s.id === shortcut.id))
        : whitelist.length > 0
        ? AllShortcuts.filter(shortcut => !!whitelist.find(s => s.id === shortcut.id))
        : AllShortcuts.filter(shortcut => !blacklist.find(s => s.id === shortcut.id));
    this.setKeyboardShortcuts(allowedShortcuts);
  }

  private setKeyboardShortcuts(shortcuts: Shortcut<Event>[]) {
    this.keyboardManager.setShortcuts(shortcuts.filter(s => s.type === 'keyboard') as Shortcut<KeyboardEvent>[]);
  }

  activate() {
    this.keyboardManager.bind();
  }

  deActivate() {
    this.keyboardManager.detach();
  }
}
