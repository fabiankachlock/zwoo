export type Shortcut<E extends Event> = {
  id: string;
  type: 'keyboard';
  execute(event: E): void;
} & (E extends KeyboardEvent
  ? {
      keyCombination: string | string[];
    }
  : // eslint-disable-next-line @typescript-eslint/ban-types
    {});
