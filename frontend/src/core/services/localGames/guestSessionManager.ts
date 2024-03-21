const guestSessionKey = 'zwoo:local-session';

export type GuestSession = {
  started: number;
  server: string;
};

export class GuestSessionManager {
  static saveSession(session: GuestSession) {
    localStorage.setItem(
      guestSessionKey,
      JSON.stringify({
        started: session.started,
        server: session.server
      })
    );
  }

  static destroySession() {
    localStorage.removeItem(guestSessionKey);
  }

  static tryGetSession(): GuestSession | undefined {
    try {
      const content = localStorage.getItem(guestSessionKey);
      return JSON.parse(content ?? '');
    } catch {
      return undefined;
    }
  }
}
