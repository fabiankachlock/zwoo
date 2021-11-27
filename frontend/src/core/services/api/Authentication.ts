export type AuthenticationStatus = {
  username: string;
  isLoggedIn: boolean;
  error?: string;
};

export class AuthenticationService {
  static performLogin = async (username: string, password: string): Promise<AuthenticationStatus> => {
    // make api call
    console.log('login:', { username, password });

    return {
      username: 'test-user',
      isLoggedIn: true
    };
  };

  static performLogout = async (): Promise<AuthenticationStatus> => {
    // make api call

    return {
      username: '',
      isLoggedIn: false
    };
  };

  static performCreateAccount = async (username: string, email: string, password: string): Promise<AuthenticationStatus> => {
    // make api call
    console.log('create Account:', { username, email, password });

    return {
      username: 'test-user',
      isLoggedIn: true
    };
  };
}

