import { AppConfig } from '@/config.js';

import { CSharpExport } from './Interpop.js';

export class WasmLoader {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  private exports: any;

  public async load() {
    // eslint-disable-next-line @typescript-eslint/ban-ts-comment
    // @ts-ignore
    //
    // const awaiter = new Awaiter();
    // const scriptTag = document.createElement('script');
    // //scriptTag.setAttribute('type', 'module');
    // scriptTag.setAttribute('src', './wasm/dotnet.js');
    // document.body.appendChild(scriptTag);
    // scriptTag.onload = ev => {
    //   console.log(ev);
    //   awaiter.callback({});
    // };
    // await awaiter.promise;
    let path = '/wasm/dotnet.js';
    if (AppConfig.IsDev) {
      path = '../../../../public' + path;
    }
    console.log('importing:', path);
    const { dotnet } = await import(/* @vite-ignore */ path);

    const { getAssemblyExports, getConfig } = await dotnet.withDiagnosticTracing(false).withApplicationArgumentsFromQuery().create();
    const config = getConfig();
    console.log(config);
    this.exports = await getAssemblyExports(config.mainAssemblyName);
    console.log(this.exports);
  }

  public getInstance(): CSharpExport {
    return {
      Test: this.exports.HelloWorld.Test
    };
  }
}
