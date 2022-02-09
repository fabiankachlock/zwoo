import { Backend, Endpoint } from './apiConfig';

export type AuthenticationStatus = {
  username: string;
  isLoggedIn: boolean;
};

export class AuthenticationService {
  static performLogin = async (email: string, password: string): Promise<AuthenticationStatus> => {
    if (process.env.VUE_APP_USE_BACKEND !== 'true') {
      console.log('login:', { email, password });
      return {
        username: 'test-user',
        isLoggedIn: true
      };
    }

    return fetch(Backend.getUrl(Endpoint.AccountLogin), {
      method: 'POST',
      body: JSON.stringify({
        email: email,
        password: password
      })
    })
      .then(async res => {
        if (res.status !== 200) {
          throw await res.text();
        }
        return res.text();
      })
      .then(() => ({
        username: email,
        isLoggedIn: true
      }));
  };

  static performLogout = async (): Promise<AuthenticationStatus> => {
    console.log('logout');
    return {
      username: 'test-user',
      isLoggedIn: true
    };
  };

  static performCreateAccount = async (username: string, email: string, password: string): Promise<AuthenticationStatus> => {
    if (process.env.VUE_APP_USE_BACKEND !== 'true') {
      console.log('create Account:', { username, email, password });
      return {
        username: 'test-user',
        isLoggedIn: true
      };
    }

    return fetch(Backend.getUrl(Endpoint.CreateAccount), {
      method: 'POST',
      body: JSON.stringify({
        username: username,
        email: email,
        password: password
      })
    })
      .then(async res => {
        if (res.status !== 200) {
          throw await res.text();
        }
        return res.text();
      })
      .then(() => ({
        username: '',
        isLoggedIn: false
      }));
  };
}
