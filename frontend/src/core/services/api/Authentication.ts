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
  static getUserInfo = async (): Promise<AuthenticationStatus> => {
    if (process.env.VUE_APP_USE_BACKEND !== 'true') {
      console.log('getting user info');
      return {
        isLoggedIn: false
      };
    }

    const response = await fetch(Backend.getUrl(Endpoint.UserInfo), {
      method: 'GET',
      credentials: 'include'
    });

    if (response.status === 404 || response.status === 401 || response.status === 400) {
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
      credentials: 'include',
      body: JSON.stringify({
        email: email,
        password: password
      })
    });

    if (response.status !== 200) {
      throw await response.text();
    }

    return await AuthenticationService.getUserInfo();
  };

  static performLogout = async (): Promise<AuthenticationStatus> => {
    if (process.env.VUE_APP_USE_BACKEND !== 'true') {
      console.log('logout:');
      return {
        isLoggedIn: false
      };
    }

    const response = await fetch(Backend.getUrl(Endpoint.AccountLogout), {
      method: 'GET',
      credentials: 'include'
    });

    if (response.status === 404 || response.status === 401) {
      return {
        isLoggedIn: false
      };
    }

    if (response.status !== 200) {
      throw await response.text();
    }

    return {
      isLoggedIn: false
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

  static performDeleteAccount = async (password: string): Promise<AuthenticationStatus> => {
    if (process.env.VUE_APP_USE_BACKEND !== 'true') {
      console.log('delete account');
      return {
        isLoggedIn: false
      };
    }

    const response = await fetch(Backend.getUrl(Endpoint.DeleteAccount), {
      method: 'POST',
      credentials: 'include',
      body: JSON.stringify({
        password: password
      })
    });

    await response.text();

    return {
      isLoggedIn: false
    };
  };
}
