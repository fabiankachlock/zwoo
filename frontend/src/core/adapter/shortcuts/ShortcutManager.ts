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

export class ShortcutManager {
  constructor(private readonly keyboardManager = new KeyboardShortcuts(AllShortcuts.filter(s => s.type === 'keyboard'))) {}

  activate() {
    this.keyboardManager.bind();
  }

  deActivate() {
    this.keyboardManager.detach();
  }
}
