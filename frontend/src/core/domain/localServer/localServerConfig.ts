export type LocalServerConfig = {
  serverId: string;
  port: number;
  ip: string;
  useDynamicPort: boolean;
  useLocalhost: boolean;
  useAllIPs: boolean;
  useStrictOrigins: boolean;
  allowedOrigins: string[];
};
