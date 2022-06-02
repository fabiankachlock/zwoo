import { Shortcut } from './types';
import { register, unregisterAll, isRegistered } from '@tauri-apps/api/globalShortcut';

export class KeyboardShortcuts {
  // eslint-disable-next-line @typescript-eslint/no-empty-function

  constructor(private shortcuts: Shortcut<KeyboardEvent>[]) {}

  bind() {
    this.detach();
    window.addEventListener('keyup', this.eventHandler);
    this.setupTauri();
  }

  setShortcuts(shortcuts: Shortcut<KeyboardEvent>[]) {
    this.shortcuts = shortcuts;
    this.setupTauri();
  }

  private eventHandler = (event: KeyboardEvent) => {
    event.preventDefault();
    for (const shortcut of this.shortcuts) {
      shortcut.execute(event);
    }
  };

  detach() {
    window.removeEventListener('keyup', this.eventHandler);
    unregisterAll();
  }

  private async setupTauri() {
    for (const shortcut of this.shortcuts) {
      for (const key of Array.isArray(shortcut.keyCombination) ? shortcut.keyCombination : [shortcut.keyCombination]) {
        if (await isRegistered(key)) continue;
        await register(key, () => {
          shortcut.execute(
            new KeyboardEvent('', {
              key: key
            })
          );
        });
      }
    }
  }
}
