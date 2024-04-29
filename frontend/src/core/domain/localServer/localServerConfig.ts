import { invoke } from '@tauri-apps/api/core';

export type LocalServerConfig = {
  serverId: string;
  secretKey: string;
  port: number;
  ip: string;
  useDynamicPort: boolean;
  useLocalhost: boolean;
  useAllIps: boolean;
  useStrictOrigins: boolean;
  allowedOrigins: string;
};

export class LocalServerConfigManager {
  public static defaultConfig: LocalServerConfig = {
    serverId: '',
    secretKey: '',
    port: 0,
    ip: '',
    useDynamicPort: false,
    useLocalhost: false,
    useAllIps: false,
    allowedOrigins: '',
    useStrictOrigins: false
  };

  public static async load(): Promise<LocalServerConfig> {
    const config = await invoke<string>('get_local_server_config');
    return JSON.parse(config) as LocalServerConfig;
  }

  public static async save(config: LocalServerConfig): Promise<void> {
    await invoke('update_local_server_config', { newConfig: config });
  }
}
