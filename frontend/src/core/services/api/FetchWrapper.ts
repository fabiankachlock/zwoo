import { BackendError, BackendErrorType, parseBackendError } from './Errors';

export type FetchOptions<T> = {
  /**
   * indicates wether the backend should be used or not
   */
  useBackend?: boolean;

  /**
   * fallback value
   */
  fallbackValue?: T;

  /**
   * configure how the request should be handled
   */
  requestOptions?: {
    /**
     * the body content type (only applied at post)
     * use `null` to unset Content-Type header
     * @default application/json
     */
    contentType?: string;

    /**
     * configure wether client side credentials should be sent
     * @default false
     */
    withCredentials?: boolean;
  };

  /**
   * validate if a response status is valid
   * @default 200 <= status < 300
   */
  statusValidator?: (status: number) => boolean;

  /**
   * configure how the response should be handled
   */
  responseOptions?: {
    /**
     * indicates wether the response should be interpreted as json
     * @default true
     */
    decodeJson?: boolean;
  };
} & RequestInit;

export type FetchResponse<T> = {
  isFallback?: boolean;
  data?: T;
  error?: BackendErrorType;
};

const defaultStatusValidator = (status: number) => status >= 200 && status < 300;

export const WrappedFetch = async <T>(url: string, init: FetchOptions<T> = {}): Promise<FetchResponse<T>> => {
  if (!init?.useBackend) {
    return {
      isFallback: true,
      data: init.fallbackValue
    };
  }

  const newHeader: Record<string, string> = {};
  for (const pair of ((init.headers as Headers) ?? new Headers()).entries()) {
    newHeader[pair[0]] = pair[1];
  }

  if (init.requestOptions?.contentType) {
    newHeader['Content-Type'] = init.requestOptions?.contentType;
  } else if (init.requestOptions?.contentType !== null) {
    newHeader['Content-Type'] = 'application/json';
  }
  init.headers = newHeader;

  if (init.requestOptions?.withCredentials) {
    init.credentials = 'include';
  }

  try {
    const response = await fetch(url, init);
    const isStatusValid = (init.statusValidator ?? defaultStatusValidator)(response.status);
    if (!isStatusValid) {
      return {
        error: parseBackendError(await response.text())
      };
    }

    if (init.responseOptions?.decodeJson === false) {
      return {
        data: (await response.text()) as T
      };
    }

    const data = await response.json();
    return {
      data
    };
  } catch (err: unknown) {
    return {
      error: {
        code: BackendError._InternalError,
        message: `${err}`
      }
    };
  }
};
