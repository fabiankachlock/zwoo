// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// TODO; just examples
// import { dotnet } from './dotnet.js'
// 
// const { setModuleImports, getAssemblyExports, getConfig } = await dotnet
//     .withDiagnosticTracing(false)
//     .withApplicationArgumentsFromQuery()
//     .create();
// 
// setModuleImports('main.js', {
//     window: {
//         location: {
//             href: () => globalThis.window.location.href
//         }
//     }
// });
// 
// const config = getConfig();
// const exports = await getAssemblyExports(config.mainAssemblyName);
// 
// export async function HelloWorld() {
//     return exports.HelloWorld.Test()
// }