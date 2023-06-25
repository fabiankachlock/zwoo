/* eslint-disable @typescript-eslint/ban-types */

import { LocationQuery } from 'vue-router';

import { AppConfig } from '@/config';
import { joinQuery } from '@/core/helper/utils';

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
  ResetPassword = 'account/resetPassword',
  ResendVerificationEmail = 'auth/resendVerificationEmail',
  VersionHistory = 'versionHistory',
  UserSettings = 'account/settings',
  ContactFormSubmission = 'contactForm'
}

export class Backend {
  public static readonly isDev = AppConfig.IsDev;
  public static readonly Url: string = AppConfig.ApiUrl;
  public static readonly WsOverride: string | undefined = AppConfig.WsUrl;

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

  public static getUrlWithQuery(endpoint: Endpoint, query: LocationQuery): string {
    return `${this.getUrl(endpoint)}?${joinQuery(query)}`;
  }

  public static getDynamicUrlWithQuery<U extends Endpoint>(endpoint: U, params: ExtractRouteParams<U>, query: LocationQuery): string {
    return `${this.getDynamicUrl(endpoint, params)}?${joinQuery(query)}`;
  }
}

export class Frontend {
  public static domain = AppConfig.Domain ?? '';
  public static url = `https://${Frontend.domain}`;
}
