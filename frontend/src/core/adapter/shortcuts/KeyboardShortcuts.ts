import { Shortcut } from './types';

export class KeyboardShortcuts {
  // eslint-disable-next-line @typescript-eslint/no-empty-function

  constructor(private readonly shortcuts: Shortcut<KeyboardEvent>[]) {}

  bind() {
    this.detach();
    window.addEventListener('keyup', this.eventHandler);
  }

  private eventHandler = (event: KeyboardEvent) => {
    event.preventDefault();
    for (const shortcut of this.shortcuts) {
      shortcut.execute(event);
    }
  };

  detach() {
    window.removeEventListener('keyup', this.eventHandler);
  }
}
