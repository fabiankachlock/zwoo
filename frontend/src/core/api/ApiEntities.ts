import { BackendErrorType } from './ApiError';

export type FetchResponse<T> = {
  isFallback?: boolean;
  data?: T;
  error?: BackendErrorType;
};

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
