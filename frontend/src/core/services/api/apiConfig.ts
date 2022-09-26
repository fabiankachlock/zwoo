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
  Version = 'version',
  Changelog = 'changelog?version=:version:',
  ChangePassword = 'account/changePassword',
  RequestPasswordReset = 'account/requestPasswordReset',
  ResetPassword = 'account/resetPassword'
}

export class Backend {
  public static readonly isDev = process.env.VUE_APP_DEVELOPMENT === 'true';
  public static readonly Url: string = Backend.isDev ? process.env.VUE_APP_DEV_BACKEND : process.env.VUE_APP_PROD_BACKEND;
  public static readonly WsOverride: string | undefined = Backend.isDev ? process.env.VUE_APP_DEV_WS_OVERRIDE : process.env.VUE_APP_PROD_WS_OVERRIDE;

  public static getUrl(endpoint: Endpoint): string {
    if (endpoint === Endpoint.Websocket) {
      return Backend.WsOverride ? `${Backend.WsOverride}${endpoint}` : `${Backend.Url}${endpoint}`;
    }
    return `${Backend.Url}${endpoint}`;
  }

  public static getDynamicUrl<U extends Endpoint>(endpoint: U, params: ExtractRouteParams<U>): string {
    let url = '';
    if (endpoint === Endpoint.Websocket) {
      url = Backend.WsOverride ? `${Backend.WsOverride}${endpoint}` : `${Backend.Url}${endpoint}`;
    } else {
      url = `${Backend.Url}${endpoint}`;
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
