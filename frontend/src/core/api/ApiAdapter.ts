import { ZRPRole } from '../domain/zrp/zrpTypes';
import { FetchOptions, FetchResponse } from './ApiEntities';
import { BackendErrorAble } from './ApiError';
import { AuthenticationStatus } from './entities/AuthenticationStatus';
import { CaptchaResponse } from './entities/Captcha';
import { GameJoinResponse, GameMeta, GamesList } from './entities/Game';
import { LeaderBoardPositionResponse, LeaderBoardResponse } from './entities/Leaderboard';

// TODO: clean up response definitions

export interface ApiAdapter {
  /**
   * Change the password of the current
   * @param oldPassword the users old password
   * @param newPassword the users new password
   */
  changeUserPassword(oldPassword: string, newPassword: string): Promise<FetchResponse<undefined>>;

  /**
   * Request a password reset for a  user
   * @param email the email address of the user
   * @param lng the users preferred language
   */
  requestUserPasswordReset(email: string, lng?: string): Promise<FetchResponse<undefined>>;

  /**
   * Reset the password of a user
   * @param code the auto generated password reset code
   * @param password the users new password
   */
  resetUserPassword(code: string, password: string): Promise<FetchResponse<undefined>>;

  /**
   * Load the synced settings of the current user
   */
  loadUserSettings(): Promise<string | undefined>;

  /**
   * Set the synced settings of the current user
   * @param settings the new serialized settings
   */
  storeUserSettings(settings: string): Promise<FetchResponse<undefined>>;

  /**
   * Load information about the currently logged in user
   */
  loadUserInfo(): Promise<AuthenticationStatus>;

  /**
   * Log a user in
   * @param email the users email address
   * @param password his password
   */
  loginUser(email: string, password: string): Promise<AuthenticationStatus>;

  /**
   * Log out a user
   */
  logoutUser(): Promise<AuthenticationStatus>;

  /**
   * Create a account for a user
   * @param username the users preferred in game name
   * @param email the users email address
   * @param password the users password
   * @param beta the beta code entered by the user
   * @param lng the users preferred language
   */
  createUserAccount(username: string, email: string, password: string, beta?: string, lng?: string): Promise<AuthenticationStatus>;

  /**
   * Delete the account of a user
   * @param password the users password
   */
  deleteUserAccount(password: string): Promise<AuthenticationStatus>;

  /**
   * Verify the account of a user
   * @param id the users id
   * @param code the account auto generate verify code
   */
  verifyUserAccount(id: string, code: string): Promise<BackendErrorAble<boolean>>;

  /**
   * Redeliver the verification email to a user
   * @param email the accounts email address
   * @param lng the users preferred language
   */
  resendVerificationEmail(email: string, lng?: string): Promise<BackendErrorAble<boolean>>;

  /**
   * Load the servers current version
   */
  loadVersion(): Promise<BackendErrorAble<string>>;

  /**
   * Load the versions with available changelogs from the server
   */
  loadVersionHistory(): Promise<BackendErrorAble<string[]>>;

  /**
   * Load a changelog for a specified version
   * @param version the changelogs version
   */
  loadChangelog(version: string): Promise<BackendErrorAble<string>>;

  /**
   * Create a new online game
   * @param name the games name
   * @param isPublic whether the game is public
   * @param password the games password (empty if public)
   */
  createGame(name: string, isPublic: boolean, password: string): Promise<GameJoinResponse>;

  /**
   * Generate a url to join a game by id
   * @param gameId id of the game to join
   */
  generateJoinUrl(gameId: string): string;

  /**
   * Load all games that a currently available
   */
  loadAvailableGames(): Promise<BackendErrorAble<GamesList>>;

  /**
   * Load meta information of a game
   * @param gameId the games id
   */
  loadGameMeta(gameId: number): Promise<BackendErrorAble<GameMeta>>;

  /**
   * Join a game
   * @param gameId the games id to join
   * @param role the players role
   * @param password the password of the game
   */
  joinGame(gameId: number, role: ZRPRole, password: string): Promise<GameJoinResponse>;

  /**
   * Load the current leader board
   */
  loadLeaderBoard(): Promise<BackendErrorAble<LeaderBoardResponse>>;

  /**
   * Load the currents user position in the leader board
   */
  loadOwnLeaderBoardPosition(): Promise<BackendErrorAble<LeaderBoardPositionResponse>>;

  /**
   * Submit a contact request
   * @param sender the senders name
   * @param message teh senders message
   */
  submitContactForm(sender: string, message: string): Promise<BackendErrorAble<string>>;

  /**
   * Verify a generated captcha token
   * @param token the token generated on the client
   */
  verifyCaptchaToken(token: string): Promise<BackendErrorAble<CaptchaResponse>>;

  /**
   * exposes the raw fetch method
   * @param url the request url
   * @param init the request options
   */
  fetchRaw<T>(url: string, init: FetchOptions<T>): Promise<FetchResponse<T>>;
}
