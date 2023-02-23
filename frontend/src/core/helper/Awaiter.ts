export class Awaiter<T = any> {
  private invoker: (data: T | undefined | null) => void;

  private awaiter: Promise<T>;

  public get callback(): (data: T | undefined | null) => void {
    return this.invoker;
  }

  public get promise(): Promise<T> {
    return this.awaiter;
  }

  constructor(extraCallback?: (data: T | undefined | null) => boolean | Promise<boolean>) {
    let resolve: (data: T) => void;
    let reject: (reason: any) => void;
    this.awaiter = new Promise((res, rej) => {
      resolve = res;
      reject = rej;
    });

    this.invoker = async (data: T | undefined | null) => {
      if (extraCallback) {
        const response = await extraCallback(data);
        if (response) {
          return resolve(data!);
        } else {
          return reject(data);
        }
      }

      return resolve(data!);
    };
  }
}
