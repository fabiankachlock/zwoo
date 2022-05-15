export type Shortcut<E extends Event> = {
  type: 'keyboard';
  execute(event: E): void;
};
