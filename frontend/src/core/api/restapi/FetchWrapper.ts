import { FetchOptions, FetchResponse } from '../ApiEntities';
import { BackendError, createEmptyProblem, parseBackendError } from '../ApiError';

const defaultStatusValidator = (status: number) => status >= 200 && status < 300;

export const WrappedFetch = async <T = undefined>(url: string, init: FetchOptions<T> = {}): FetchResponse<T> => {
  if (!init?.useBackend && init.fallbackValue) {
    return {
      isFallback: true,
      wasSuccessful: true,
      isError: false,
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
        isError: true,
        isFallback: false,
        wasSuccessful: false,
        error: parseBackendError(await response.text())
      };
    }

    if (init.responseOptions?.decodeJson === false) {
      return {
        isError: false,
        isFallback: false,
        wasSuccessful: true,
        data: (await response.text()) as T
      };
    }

    const data = await response.json();
    return {
      isError: false,
      isFallback: false,
      wasSuccessful: true,
      data
    };
  } catch (err: unknown) {
    return {
      isError: true,
      isFallback: false,
      wasSuccessful: false,
      error: {
        problem: createEmptyProblem(),
        code: BackendError._InternalError,
        type: `${err}`
      }
    };
  }
};
