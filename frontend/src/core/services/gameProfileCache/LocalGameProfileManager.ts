import type { BaseLogger } from '@/core/services/logging/logTypes';

export class LocalGameProfileManager {
  private static StorageKey = 'zwoo:gameProfiles';
  private logger: BaseLogger;

  private constructor(log: BaseLogger) {
    this.logger = log;
  }

  private readonlyError = new Error('xy.errors.onlineOnly');

  public static async create() {
    const loggerModule = await import('@/core/services/logging/logImport');
    return new LocalGameProfileManager(loggerModule.Logger);
  }

  public syncProfiles(content: string) {
    localStorage.setItem(LocalGameProfileManager.StorageKey, content);
    this.logger.info('synced local game profiles');
  }

  public getProfiles(): string {
    return localStorage.getItem(LocalGameProfileManager.StorageKey) ?? '[]';
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  public saveProfile(name: string, profile: string) {
    throw this.readonlyError;
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  public updateProfile(id: string, profile: string) {
    throw this.readonlyError;
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  public deleteProfile(id: string) {
    throw this.readonlyError;
  }
}
