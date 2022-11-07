export class IconCache {
  private static cache: Record<string, string> = {};

  static async getIcon(icon: string): Promise<{ icon: string; cacheHit: boolean }> {
    if (IconCache.cache[icon]) {
      return {
        icon: IconCache.cache[icon],
        cacheHit: true
      };
    }

    const iconContent = await import(`./icons/${icon}.js`);
    IconCache.cache[icon] = iconContent.default;
    return {
      icon: iconContent.default,
      cacheHit: false
    };
  }
}
