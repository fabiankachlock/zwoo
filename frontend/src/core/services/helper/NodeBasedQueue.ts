import { DoublyLinkedList } from './DoublyLinkedList';

export class NodeBasedQueue<T> {
  private list: DoublyLinkedList<T>;

  get length(): number {
    return this.list.length;
  }

  get isEmpty(): boolean {
    return this.list.isEmpty;
  }

  constructor() {
    this.list = new DoublyLinkedList();
  }

  private static fromList<E>(list: DoublyLinkedList<E>): NodeBasedQueue<E> {
    const queue = new NodeBasedQueue<E>();
    queue.list = list;
    return queue;
  }

  public copy(): NodeBasedQueue<T> {
    const queue = new NodeBasedQueue<T>();
    queue.list = this.list.copy();
    return queue;
  }

  public clear(): void {
    this.list.clear();
  }

  public enqueue(value: T) {
    this.list.append(value);
  }

  public dequeue(): T | undefined {
    return this.list.removeFirst();
  }

  public peek(): T | undefined {
    return this.list.get(0);
  }

  public forEach(action: (value: T, index: number) => void): void {
    this.list.forEach(action);
  }

  public map<U>(mutation: (value: T, index: number) => U): NodeBasedQueue<U> {
    return NodeBasedQueue.fromList(this.list.map(mutation));
  }

  public reduce<A>(action: (aggregated: A, value: T, index: number) => A, startValue: A): A {
    return this.list.reduce(action, startValue);
  }

  public toArray(): T[] {
    return this.list.toArray();
  }
}
