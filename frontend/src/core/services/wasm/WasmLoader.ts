import { AppConfig } from '@/config.js';

import Logger from '../logging/logImport.js';
import { CSharpExport } from './Interpop.js';

export class WasmLoader {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  private exports: any;

  public async load() {
    Logger.Wasm.info('loading webassembly');
    let path = '/wasm/_framework/dotnet.js';
    if (AppConfig.IsDev) {
      path = '../../../../public' + path;
    }
    Logger.Wasm.info(`import path: ${path}`);
    const { dotnet } = await import(/* @vite-ignore */ path);

    const { getAssemblyExports, getConfig } = await dotnet.withDiagnosticTracing(false).withApplicationArgumentsFromQuery().create();
    const config = getConfig();
    console.log(config);
    Logger.Wasm.info(`config loaded - assembly: ${config.mainAssemblyName}`);
    this.exports = await getAssemblyExports(config.mainAssemblyName);
    Logger.Wasm.info(`loaded webassembly ${config.mainAssemblyName}`);
    if (AppConfig.IsDev) {
      Logger.Wasm.debug(`provided interface: ${JSON.stringify(this.exports, null, 2)}`);
      console.log(this.exports);
    }
  }

  public getInstance(): CSharpExport {
    return {
      ...this.exports.ZwooWasm
    };
  }
}
