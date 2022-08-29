/* eslint-disable @typescript-eslint/ban-types */

type ExtractRouteParams<str extends string> = str extends ''
  ? {}
  : str extends `/${infer rest}`
  ? ExtractRouteParams<rest>
  : str extends `:${infer front}:${infer rest}`
  ? { [K in front]: string } & ExtractRouteParams<rest>
  : str extends `${string}/${infer rest}`
  ? ExtractRouteParams<rest>
  : str extends `${string}?${string}=:${infer paramName}:${infer rest}`
  ? { [K in paramName]: string } & ExtractRouteParams<rest>
  : str extends `&${string}=:${infer paramName}:${infer rest}`
  ? { [K in paramName]: string } & ExtractRouteParams<rest>
  : {};

export enum Endpoint {
  CreateAccount = 'auth/create',
  Recaptcha = 'auth/recaptcha',
  AccountVerify = 'auth/verify?id=:id:&code=:code:',
  AccountLogin = 'auth/login',
  AccountLogout = 'auth/logout',
  UserInfo = 'auth/user',
  DeleteAccount = 'auth/delete',
  JoinGame = 'game/join',
  Websocket = 'game/join/:id:',
  LeaderBoard = 'game/leaderboard',
  LeaderBoardPosition = 'game/leaderboard/position',
  Games = 'game/games',
  Game = 'game/games/:id:',
  Version = 'version'
}

export class Backend {
  public static readonly URL = process.env.VUE_APP_BACKEND_URL as string;

  public static getUrl(endpoint: Endpoint): string {
    if (endpoint === Endpoint.Websocket) {
      return process.env.VUE_APP_WS_OVERRIDE ? `${process.env.VUE_APP_WS_OVERRIDE}${endpoint}` : `${Backend.URL}${endpoint}`;
    }
    return `${Backend.URL}${endpoint}`;
  }

  public static getDynamicUrl<U extends Endpoint>(endpoint: U, params: ExtractRouteParams<U>): string {
    let url = '';
    if (endpoint === Endpoint.Websocket) {
      url = process.env.VUE_APP_WS_OVERRIDE ? `${process.env.VUE_APP_WS_OVERRIDE}${endpoint}` : `${Backend.URL}${endpoint}`;
    } else {
      url = `${Backend.URL}${endpoint}`;
    }

    for (const key in params) {
      url = url.replaceAll(`:${key}:`, params[key] as unknown as string);
    }

    return url;
  }
}

export class Frontend {
  public static domain = process.env.VUE_APP_DOMAIN ?? '';
  public static url = `https://${Frontend.domain}`;
}
