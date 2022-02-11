import { Backend, Endpoint } from './apiConfig';

type UserInfo = {
  username: string;
  email: string;
};

export type AuthenticationStatus =
  | {
      username: string;
      email: string;
      isLoggedIn: true;
    }
  | {
      isLoggedIn: false;
    };

export class AuthenticationService {
  static getUserInfo = async (puid: string, sid: string): Promise<AuthenticationStatus> => {
    console.log('getting user info');
    const response = await fetch(Backend.getUrl(Endpoint.UserInfo), {
      method: 'GET',
      body: JSON.stringify({
        puid,
        sid
      })
    });

    if (response.status === 404 || response.status === 401) {
      return {
        isLoggedIn: false
      };
    }

    if (response.status !== 200) {
      throw await response.text();
    }

    const data = (await response.json()) as UserInfo;
    return {
      isLoggedIn: true,
      username: data.username,
      email: data.email
    };
  };

  static performLogin = async (email: string, password: string): Promise<AuthenticationStatus> => {
    if (process.env.VUE_APP_USE_BACKEND !== 'true') {
      console.log('login:', { email, password });
      return {
        username: 'test-user',
        email: 'test@test.com',
        isLoggedIn: true
      };
    }

    const response = await fetch(Backend.getUrl(Endpoint.AccountLogin), {
      method: 'POST',
      body: JSON.stringify({
        email: email,
        password: password
      })
    });

    if (response.status !== 200) {
      throw await response.text();
    }

    const ids = (await response.json()) as {
      puid: string;
      sid: string;
    };

    console.log(ids);

    return await AuthenticationService.getUserInfo(ids.puid, ids.sid);
  };

  static performLogout = async (): Promise<AuthenticationStatus> => {
    console.log('logout');
    return {
      username: 'test-user',
      email: 'test@test.com',
      isLoggedIn: true
    };
  };

  static performCreateAccount = async (username: string, email: string, password: string): Promise<AuthenticationStatus> => {
    if (process.env.VUE_APP_USE_BACKEND !== 'true') {
      console.log('create Account:', { username, email, password });
      return {
        username: 'test-user',
        email: 'test@test.com',
        isLoggedIn: true
      };
    }

    const response = await fetch(Backend.getUrl(Endpoint.CreateAccount), {
      method: 'POST',
      body: JSON.stringify({
        username: username,
        email: email,
        password: password
      })
    });

    if (response.status !== 200) {
      throw await response.text();
    }

    return {
      isLoggedIn: false // users can only log in, when the account is verified
    };
  };

  static performDeleteAccount = async (password: string): Promise<void> => {
    console.log('delete account', password);
  };
}
