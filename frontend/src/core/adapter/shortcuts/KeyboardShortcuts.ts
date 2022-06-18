import { Shortcut } from './types';
import { register, unregisterAll, isRegistered } from '@tauri-apps/api/globalShortcut';

export class KeyboardShortcuts {
  // eslint-disable-next-line @typescript-eslint/no-empty-function

  constructor(private shortcuts: Shortcut<KeyboardEvent>[]) {}

  bind() {
    this.detach();
    if (process.env.VUE_APP_IS_TAURI === 'true') {
      this.setupTauri();
    } else {
      window.addEventListener('keyup', this.eventHandler);
    }
  }

  setShortcuts(shortcuts: Shortcut<KeyboardEvent>[]) {
    if (process.env.VUE_APP_IS_TAURI === 'true') {
      this.setupTauri();
    } else {
      this.shortcuts = shortcuts;
    }
  }

  private eventHandler = (event: KeyboardEvent) => {
    event.preventDefault();
    for (const shortcut of this.shortcuts) {
      shortcut.execute(event);
    }
  };

  detach() {
    if (process.env.VUE_APP_IS_TAURI === 'true') {
      unregisterAll();
    } else {
      window.removeEventListener('keyup', this.eventHandler);
    }
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
