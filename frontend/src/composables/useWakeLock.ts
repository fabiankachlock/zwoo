import Logger from '@/core/services/logging/logImport';

// TODO: ts definitions
interface WakeLock {
  request(type: string): Promise<WakeLockSentinel>;
}

interface WakeLockSentinel {
  release(): Promise<void>;
  readonly onrelease: (event: Event) => unknown;
  readonly type: WakeLockType;
  addEventListener(event: string, callback: EventListenerOrEventListenerObject, options?: unknown): void;
  removeEventListener(event: string, callback: EventListenerOrEventListenerObject, options?: unknown): void;
}

type WakeLockType = 'screen' | null;

interface NavigatorExtended extends Navigator {
  // Only available in a secure context.
  readonly wakeLock: WakeLock;
}

export const useWakeLock = async (): Promise<(() => void) | undefined> => {
  try {
    const wakeLock = await (navigator as NavigatorExtended).wakeLock.request('screen');
    if (wakeLock) {
      Logger.debug(`WakeLock instantiated: ${wakeLock.type}`);
    }
    return () => {
      if (wakeLock) {
        Logger.debug(`WakeLock released: ${wakeLock.type}`);
        wakeLock.release();
      }
    };
  } catch (e: unknown) {
    Logger.warn(`WakeLock request failed: ${e}`);
    return undefined;
  }
};
