export enum Endpoint {
  CreateAccount = 'auth/create',
  Recaptcha = 'auth/recaptcha',
  AccountVerify = 'auth/verify',
  AccountLogin = 'auth/login',
  AccountLogout = 'auth/logout',
  UserInfo = 'auth/user',
  DeleteAccount = 'auth/delete',
  JoinGame = 'game/join',
  Websocket = 'game/join/' // TODO: insert real url
}

export class Backend {
  public static readonly URL = process.env.VUE_APP_BACKEND_URL as string;
  public static getUrl = (endpoint: Endpoint): string => {
    if (endpoint === Endpoint.Websocket) {
      return process.env.VUE_APP_WS_OVERRIDE ? `${process.env.VUE_APP_WS_OVERRIDE}${endpoint}` : `${Backend.URL}${endpoint}`;
    }
    return `${Backend.URL}${endpoint}`;
  };
}
