import { useRoute, useRouter } from 'vue-router';

const redirectKey = 'redirect';

export const useRedirect = () => {
  const router = useRouter();
  const route = useRoute();

  const getRedirect = (): string | undefined => {
    return (Array.isArray(route.query[redirectKey]) ? route.query[redirectKey][0] : route.query[redirectKey]) ?? undefined;
  };

  const setRedirect = (to: string | undefined) => {
    router.replace({ query: { [redirectKey]: to } });
  };

  return {
    /**
     * push a new location but keep a redirect to the current location
     * @param to the new location
     */
    redirect: (to: string) => {
      const current = route.fullPath;
      router.push({ path: to, query: { [redirectKey]: current } });
    },

    /**
     * replace a new location but keep a redirect to the current location
     * @param to the new location
     */
    redirectReplace: (to: string) => {
      const current = route.fullPath;
      router.replace({ path: to, query: { [redirectKey]: current } });
    },

    /**
     * pushes a new location keeping an existing redirect alive
     * @param to the new location
     */
    redirectSafePush: (to: string) => {
      const redirect = getRedirect();
      router.push({ path: to, query: { [redirectKey]: redirect } });
    },

    /**
     * replaces a new location while keeping an existing redirect alive
     * @param to the enw location
     */
    redirectSafeReplace: (to: string) => {
      const redirect = getRedirect();
      router.replace({ path: to, query: { [redirectKey]: redirect } });
    },

    /**
     * try reading a redirect from the url and use it if existing
     * @returns whether a redirect was made
     */
    applyRedirect: (): boolean => {
      const redirect = getRedirect();
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
    applyRedirectReplace: function (): boolean {
      const redirect = getRedirect();
      if (redirect) {
        router.replace(redirect);
        return true;
      }
      return false;
    },

    /**
     * remove any existing redirect
     */
    clearRedirect: function () {
      setRedirect(undefined);
    },

    /**
     * read an existing redirect
     */
    getRedirect,

    /**
     * set a new redirect manually
     * @param to the location to redirect
     */
    setRedirect
  };
};

/**
 * create a query parameter thats understood by useRedirect
 */
export const createRedirect = (to: string): string => {
  return `${redirectKey}=${to}`;
};

/**
 * when redirecting again, reapply the redirect parameter to the new route
 */
export const keepRedirect = (from: string, to: string): string => {
  const redirect = getRedirectFromUrl(from);
  if (redirect) {
    if (to.includes('?')) {
      return `${to}&${redirectKey}=${redirect}`;
    } else {
      return `${to}?${redirectKey}=${redirect}`;
    }
  }

  return to;
};

const getRedirectFromUrl = (url: string): string | null => {
  const queryString = url.split('?')[1];
  if (queryString) {
    const params = new URLSearchParams(queryString);
    return params.get(redirectKey);
  }
  return null;
};
