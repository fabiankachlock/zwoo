export enum Key {
  ArrowLeft = 'ArrowLeft',
  ArrowUp = 'ArrowUp',
  ArrowRight = 'ArrowRight',
  ArrowDown = 'ArrowDown',
  w = 'w',
  a = 'a',
  s = 's',
  d = 'd'
}

export const useKeyPress = (keys: (Key | string)[], handler: (key: string, event: KeyboardEvent) => void): (() => void) => {
  const eventHandler = (event: KeyboardEvent) => {
    if (keys.includes(event.key)) {
      handler(event.key, event);
    }
  };

  window.addEventListener('keyup', eventHandler);
  return () => {
    window.removeEventListener('keyup', eventHandler);
  };
};
