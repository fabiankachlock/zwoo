import Logger from '@/core/services/logging/logImport';

export const useWakeLock = async (): Promise<(() => void) | undefined> => {
  try {
    const wakeLock = await navigator.wakeLock.request('screen');
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
