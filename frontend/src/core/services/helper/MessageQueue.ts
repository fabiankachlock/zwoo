import { Awaiter } from './Awaiter';

export type Task<T> = () => Promise<T>;

export type AsyncQueueItem<T> = {
  execute: Task<T>;
  awaiter: Awaiter;
};

export class AsyncMessageQueue {
  private queue: Array<AsyncQueueItem<unknown>>;

  private working = false;

  private stopped = false;

  get isStopped(): boolean {
    return this.stopped;
  }

  constructor() {
    this.queue = [];
  }

  public stop(): void {
    this.stopped = true;
  }

  public continue(): void {
    this.stopped = false;
    this.tryExecute();
  }

  async execute<T>(task: Task<T>): Promise<T> {
    const item: AsyncQueueItem<T> = {
      execute: task,
      awaiter: new Awaiter<T>()
    };

    this.queue.push(item as AsyncQueueItem<unknown>);
    if (!this.stopped) {
      this.tryExecute();
    }

    return item.awaiter.promise;
  }

  private async tryExecute(): Promise<void> {
    if (!this.stopped && !this.working && this.queue.length > 0) {
      this.working = true;
      const child = this.queue[0];
      const result = await child.execute();
      child.awaiter.callback(result);
      this.queue.shift();
      this.working = false;
    }
  }
}
