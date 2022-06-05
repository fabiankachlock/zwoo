export type Shortcut<E extends Event> = {
  id: string;
  type: 'keyboard';
  execute(event: E): void;
};
