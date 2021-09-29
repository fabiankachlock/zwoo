export type AuthenticationStatus = {
  username: string;
  isLoggedIn: boolean;
  error?: string;
};

// TODO: receive errors and translate them (like validators) before throwing
export class AuthenticationService {
  static performLogin = async (username: string, password: string): Promise<AuthenticationStatus> => {
    // make api call

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

    return {
      username: 'test-user',
      isLoggedIn: true
    };
  };
}
