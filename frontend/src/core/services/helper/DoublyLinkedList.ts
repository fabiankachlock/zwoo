/* eslint-disable @typescript-eslint/no-non-null-assertion */
export type DoublyLinkedListNode<T> = {
  value: T;
  next: DoublyLinkedListNode<T> | undefined;
  previous: DoublyLinkedListNode<T> | undefined;
};

export class DoublyLinkedList<T> {
  private head: DoublyLinkedListNode<T> | undefined;
  private tail: DoublyLinkedListNode<T> | undefined;

  private size: number;

  get length(): number {
    return this.size;
  }

  get isEmpty(): boolean {
    return this.size === 0;
  }

  constructor() {
    this.head = undefined;
    this.tail = undefined;
    this.size = 0;
  }

  public append(value: T): void {
    this.size++;
    if (this.isEmpty) {
      this.head = {
        value,
        next: undefined,
        previous: undefined
      };
      this.tail = this.head;
      return;
    }
    this.tail = {
      value,
      next: undefined,
      previous: this.tail
    };
  }

  public prepend(value: T): void {
    this.size++;
    if (this.isEmpty) {
      this.head = {
        value,
        next: undefined,
        previous: undefined
      };
      this.tail = this.head;
      return;
    }
    this.head = {
      value,
      next: this.head,
      previous: undefined
    };
  }

  public addAt(value: T, index: number): boolean {
    if (index === 0) {
      this.prepend(value);
      return true;
    }
    if (index === this.size) {
      this.append(value);
      return true;
    }
    if (index > this.size) return false;
    let node = this.head as DoublyLinkedListNode<T>;
    for (let idx = 0; idx < index - 1; idx++) {
      node = node.next!;
    }
    node.next = {
      value,
      previous: node,
      next: node.next
    };
    if (node.next.next) {
      node.next.next.previous = node.next;
    }
    return true;
  }

  public removeLast(): T | undefined {
    if (this.isEmpty) return undefined;
    this.size--;
    const value = this.tail!.value;
    this.tail = this.tail?.previous;
    if (this.tail) {
      this.tail.next = undefined;
    }
    return value;
  }

  public removeFirst(): T | undefined {
    if (this.isEmpty) return undefined;
    this.size--;
    const value = this.head!.value;
    this.head = this.head?.next;
    if (this.head) {
      this.head.previous = undefined;
    }
    return value;
  }

  public removeAt(index: number): T | undefined {
    if (index === 0) {
      return this.removeFirst();
    }
    if (index === this.size) {
      return this.removeLast();
    }
    if (index > this.size) return undefined;
    let node = this.head as DoublyLinkedListNode<T>;
    for (let idx = 0; idx < index - 1; idx++) {
      node = node.next!;
    }
    this.size--;
    const value = node.next!.value;
    node.next = node.next?.next;
    if (node.next) {
      node.next.previous = node;
    }
    return value;
  }

  public forEach(action: (value: T, index: number) => void): void {
    if (this.isEmpty) return;
    let node = this.head as DoublyLinkedListNode<T>;
    for (let idx = 0; idx < this.size; idx++) {
      action(node.value, idx);
      node = node.next!;
    }
  }

  public map<U>(mutation: (value: T, index: number) => U): DoublyLinkedList<U> {
    const newList = new DoublyLinkedList<U>();
    if (this.isEmpty) return newList;
    let node = this.head as DoublyLinkedListNode<T>;
    for (let idx = 0; idx < this.size; idx++) {
      newList.append(mutation(node.value, idx));
      node = node.next!;
    }
    return newList;
  }

  public reduce<A>(action: (aggregated: A, value: T, index: number) => A, startValue: A): A {
    let aggregatedValue = startValue;
    if (this.isEmpty) return aggregatedValue;
    let node = this.head as DoublyLinkedListNode<T>;
    for (let idx = 0; idx < this.size; idx++) {
      aggregatedValue = action(aggregatedValue, node.value, idx);
      node = node.next!;
    }
    return aggregatedValue;
  }

  public toArray(): T[] {
    const arr = [] as T[];
    this.forEach(item => arr.push(item));
    return arr;
  }
}
