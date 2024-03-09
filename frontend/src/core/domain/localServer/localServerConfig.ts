import { invoke } from '@tauri-apps/api';

export type LocalServerConfig = {
  serverId: string;
  port: number;
  ip: string;
  useDynamicPort: boolean;
  useLocalhost: boolean;
  useAllIPs: boolean;
  useStrictOrigins: boolean;
  allowedOrigins: string;
};

export class LocalServerConfigManager {
  public static defaultConfig: LocalServerConfig = {
    serverId: '',
    port: 0,
    ip: '',
    useDynamicPort: false,
    useLocalhost: false,
    useAllIPs: false,
    allowedOrigins: '',
    useStrictOrigins: false
  };

  public static async load(): Promise<LocalServerConfig> {
    const config = await invoke<string>('get_local_server_config');
    return JSON.parse(config) as LocalServerConfig;
  }

  public static async save(config: LocalServerConfig): Promise<void> {
    await invoke('update_local_server_config', config);
  }
}
