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
    instance.Logging.WasmLoggerFactory.OnInfo(msg => loggerModule.Logger.info(msg));
    instance.Logging.WasmLoggerFactory.OnDebug(msg => loggerModule.Logger.debug(msg));
    instance.Logging.WasmLoggerFactory.OnWarn(msg => loggerModule.Logger.warn(msg));
    instance.Logging.WasmLoggerFactory.OnError(msg => loggerModule.Logger.error(msg));
  }
}