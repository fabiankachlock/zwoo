export enum Endpoint {
  CreateAccount = 'auth/create',
  Recaptcha = 'auth/recaptcha',
  AccountVerify = 'auth/verify',
  AccountLogin = 'auth/login',
  AccountLogout = 'auth/login',
  UserInfo = 'auth/user',
  DeleteAccount = 'auth/delete'
}

export class Backend {
  public static readonly URL = process.env.VUE_APP_BACKEND_URL as string;
  public static getUrl = (endpoint: Endpoint): string => `${Backend.URL}${endpoint}`;
}
