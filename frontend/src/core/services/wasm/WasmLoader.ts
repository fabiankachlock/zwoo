import { AppConfig } from '@/config.js';

import Logger from '../logging/logImport.js';
import { CSharpExport } from './Interpop.js';

export class WasmLoader {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  private exports: any;

  public async load() {
    Logger.Wasm.info('loading webassembly');
    let path = '/wasm/dotnet.js';
    if (AppConfig.IsDev) {
      path = '../../../../public' + path;
    }
    Logger.Wasm.info(`import path: ${path}`);
    const { dotnet } = await import(/* @vite-ignore */ path);

    const { getAssemblyExports, getConfig } = await dotnet.withDiagnosticTracing(false).withApplicationArgumentsFromQuery().create();
    const config = getConfig();
    Logger.Wasm.info(`config loaded - assembly: ${config.mainAssemblyName}`);
    this.exports = await getAssemblyExports(config.mainAssemblyName);
    Logger.Wasm.info(`loaded webassembly ${config.mainAssemblyName}`);
  }

  public getInstance(): CSharpExport {
    return {
      Test: this.exports.ZwooWasm.GameManager.Test
    };
  }
}
