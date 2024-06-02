import { LocalGameProfileManager } from '@/core/services/gameProfileCache/LocalGameProfileManager';

import { CSharpExport } from './Interpop';
import { WasmLoader } from './WasmLoader';

export class WasmManger {
  public static readonly global = new WasmManger();

  private instance?: CSharpExport;
  private loader: WasmLoader;

  constructor() {
    this.loader = new WasmLoader();
  }

  public async initialize(): Promise<CSharpExport> {
    await this.loader.load();
    this.instance = this.loader.getInstance();
    await this.setupInstance(this.instance);
    return this.instance;
  }

  public async getInstance(): Promise<CSharpExport> {
    if (!this.instance) {
      return await this.initialize();
    }
    return this.instance;
  }

  private async setupInstance(instance: CSharpExport) {
    const loggerModule = await import('../logging/logImport');
    instance.Logging.WasmLoggerFactory.OnInfo(msg => loggerModule.Logger.Wasm.info(msg));
    instance.Logging.WasmLoggerFactory.OnDebug(msg => loggerModule.Logger.Wasm.debug(msg));
    instance.Logging.WasmLoggerFactory.OnWarn(msg => loggerModule.Logger.Wasm.warn(msg));
    instance.Logging.WasmLoggerFactory.OnError(msg => loggerModule.Logger.Wasm.error(msg));

    const profiles = await LocalGameProfileManager.create();
    instance.LocalGameProfileProvider.OnGetProfiles(() => profiles.getProfiles());
    instance.LocalGameProfileProvider.OnSave((name, profile) => profiles.saveProfile(name, profile));
    instance.LocalGameProfileProvider.OnUpdate((id, profile) => profiles.updateProfile(id, profile));
    instance.LocalGameProfileProvider.OnDelete(id => profiles.deleteProfile(id));
  }
}
