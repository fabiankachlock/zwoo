export interface Migration {
  name: string;
  version: string;
  module: () => Promise<{
    up(): Promise<void> | void;
    down(): Promise<void> | void;
  }>;
}
