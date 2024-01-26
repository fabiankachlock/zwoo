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
  Discover = 'discover',
  AccountLogin = 'auth/login',
  GuestLogin = 'auth/login-guest',
  AccountLogout = 'auth/logout',
  UserInfo = 'auth/user',
  CreateAccount = 'account/create',
  DeleteAccount = 'account/delete',
  AccountVerify = 'account/verify?id=:id:&code=:code:',
  ResendVerificationEmail = 'account/verify/resend',
  UserSettings = 'account/settings',
  ChangePassword = 'account/password/change',
  RequestPasswordReset = 'account/password/request-reset',
  ResetPassword = 'account/password/reset',
  LeaderBoard = 'leaderboard',
  LeaderBoardPosition = 'leaderboard/self',
  Games = 'game/list?recommended=:recommended:&offset=:offset:&limit=:limit:&filter=:filter:&publicOnly=:public:',
  CreateGame = 'game/create',
  JoinGame = 'game/join',
  Game = 'game/:id:',
  Websocket = 'game/:id:/connect',
  VersionHistory = 'changelog',
  Changelog = 'changelog/:version:',
  ContactFormSubmission = 'contact'
}

export class Backend {
  public readonly isDev = AppConfig.IsDev;
  public readonly apiUrl: string;
  public readonly wsOverride: string;

  public constructor(apiUrl: string = AppConfig.ApiUrl, wsUrl: string = AppConfig.WsUrl) {
    this.apiUrl = apiUrl;
    this.wsOverride = wsUrl;
  }

  public static from(apiUrl: string, wsUrl: string): Backend {
    return new Backend(apiUrl, wsUrl);
  }

  public getUrl(endpoint: Endpoint): string {
    if (endpoint === Endpoint.Websocket) {
      return this.wsOverride ? `${this.wsOverride}${endpoint}` : `${this.apiUrl}${endpoint}`;
    }
    return `${this.apiUrl}${endpoint}`;
  }

  public getDynamicUrl<U extends Endpoint>(endpoint: U, params: ExtractRouteParams<U>): string {
    let url = '';
    if (endpoint === Endpoint.Websocket) {
      url = this.wsOverride ? `${this.wsOverride}${endpoint}` : `${this.apiUrl}${endpoint}`;
    } else {
      url = `${this.apiUrl}${endpoint}`;
    }

    for (const key in params) {
      url = url.replaceAll(`:${key}:`, params[key] as unknown as string);
    }

    return url;
  }

  public getUrlWithQuery(endpoint: Endpoint, query: LocationQuery): string {
    return `${this.getUrl(endpoint)}?${joinQuery(query)}`;
  }

  public getDynamicUrlWithQuery<U extends Endpoint>(endpoint: U, params: ExtractRouteParams<U>, query: LocationQuery): string {
    return `${this.getDynamicUrl(endpoint, params)}?${joinQuery(query)}`;
  }
}

export class Frontend {
  public static domain = AppConfig.Domain ?? '';
  public static url = `https://${Frontend.domain}`;
}
