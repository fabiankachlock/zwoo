import { useRoute, useRouter } from 'vue-router';

const redirectKey = 'redirect';

export const useRedirect = () => ({
  /**
   * push a new location but keep a redirect to the current location
   * @param to the new location
   */
  redirect(to: string) {
    const route = useRoute();
    const router = useRouter();
    const current = route.fullPath;
    router.push({ path: to, query: { [redirectKey]: current } });
  },

  /**
   * replace a new location but keep a redirect to the current location
   * @param to the new location
   */
  redirectReplace(to: string) {
    const route = useRoute();
    const router = useRouter();
    const current = route.fullPath;
    router.replace({ path: to, query: { [redirectKey]: current } });
  },

  /**
   * pushes a new location keeping an existing redirect alive
   * @param to the new location
   */
  redirectSafePush(to: string) {
    const router = useRouter();
    const redirect = this.getRedirect();
    router.push({ path: to, query: { [redirectKey]: redirect } });
  },

  /**
   * replaces a new location while keeping an existing redirect alive
   * @param to the enw location
   */
  redirectSafeReplace(to: string) {
    const router = useRouter();
    const redirect = this.getRedirect();
    router.replace({ path: to, query: { [redirectKey]: redirect } });
  },

  /**
   * try reading a redirect from the url and use it if existing
   * @returns whether a redirect was made
   */
  applyRedirect(): boolean {
    const router = useRouter();
    const redirect = this.getRedirect();
    if (redirect) {
      router.push(redirect);
      return true;
    }
    return false;
  },

  /**
   * try reading a redirect from the url and use it if existing
   * @returns whether a redirect was made
   */
  applyRedirectReplace(): boolean {
    const router = useRouter();
    const redirect = this.getRedirect();
    if (redirect) {
      router.replace(redirect);
      return true;
    }
    return false;
  },

  /**
   * remove any existing redirect
   */
  clearRedirect() {
    this.setRedirect(undefined);
  },

  /**
   * read an existing redirect
   */
  getRedirect(): string | undefined {
    const route = useRoute();
    return (Array.isArray(route.query[redirectKey]) ? route.query[redirectKey][0] : route.query[redirectKey]) ?? undefined;
  },

  /**
   * set a new redirect manually
   * @param to the location to redirect
   */
  setRedirect(to: string | undefined) {
    const router = useRouter();
    router.replace({ query: { [redirectKey]: to } });
  },

  /**
   * create a query parameter thats understood by useRedirect
   */
  createRedirect(to: string): string {
    return `${redirectKey}=${to}`;
  }
});
