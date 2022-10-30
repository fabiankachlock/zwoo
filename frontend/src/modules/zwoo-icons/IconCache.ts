export class IconCache {
  private static cache: Record<string, any> = {};

  static async getIcon(icon: string): Promise<{ icon: any; cacheHit: boolean }> {
    if (IconCache.cache[icon]) {
      return {
        icon: IconCache.cache[icon],
        cacheHit: true
      };
    }
    console.log(icon);
    const iconContent = await import(`./icons/${icon}.js`);
    IconCache.cache[icon] = iconContent.default;
    return {
      icon: iconContent.default,
      cacheHit: false
    };
  }
}
