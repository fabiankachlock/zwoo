import { ZRPRole } from '../domain/zrp/zrpTypes';
import { FetchOptions, FetchResponse } from './ApiEntities';
import { CreateAccount, Login, UserSession, UserSettings } from './entities/Authentication';
import { ContactForm } from './entities/ContactForm';
import { GameJoinResponse, GameMeta, GamesList, Leaderboard, LeaderboardPosition } from './entities/Game';
import { ClientInfo, VersionHistory } from './entities/Misc';

export interface ApiAdapter {
  /**
   * returns the url of the current backend
   */
  getServer(): string;

  /**
   * Change the password of the current
   * @param oldPassword the users old password
   * @param newPassword the users new password
   */
  changeUserPassword(oldPassword: string, newPassword: string): FetchResponse<undefined>;

  /**
   * Request a password reset for a  user
   * @param email the email address of the user
   * @param captchaToken the token generated by the captcha challenge
   * @param lng the users preferred language
   */
  requestUserPasswordReset(email: string, captchaToken: string, lng?: string): FetchResponse<undefined>;

  /**
   * Reset the password of a user
   * @param code the auto generated password reset code
   * @param password the users new password
   * @param captchaToken the token generated by the captcha challenge
   */
  resetUserPassword(code: string, password: string, captchaToken: string): FetchResponse<undefined>;

  /**
   * Load the synced settings of the current user
   */
  loadUserSettings(): FetchResponse<UserSettings>;

  /**
   * Set the synced settings of the current user
   * @param settings the new serialized settings
   */
  storeUserSettings(settings: string): FetchResponse<undefined>;

  /**
   * Load information about the currently logged in user
   */
  loadUserInfo(): FetchResponse<UserSession>;

  /**
   * Log a user in
   * @param data the login information
   */
  loginUser(data: Login): FetchResponse<UserSession>;

  /**
   * Log out a user
   */
  logoutUser(): FetchResponse<void>;

  /**
   * Create a account for a user
   * @param data the new users data
   * @param lng the users preferred language
   */
  createUserAccount(data: CreateAccount, lng?: string): FetchResponse<void>;

  /**
   * Delete the account of a user
   * @param password the users password
   */
  deleteUserAccount(password: string): FetchResponse<void>;

  /**
   * Verify the account of a user
   * @param id the users id
   * @param code the account auto generate verify code
   */
  verifyUserAccount(id: string, code: string): FetchResponse<void>;

  /**
   * Redeliver the verification email to a user
   * @param email the accounts email address
   * @param lng the users preferred language
   */
  resendVerificationEmail(email: string, lng?: string): FetchResponse<void>;

  /**
   * Load the servers current version
   * @param version the own application version
   * @param zrp the running zrp version
   */
  checkVersion(version: string, zrp: string): FetchResponse<ClientInfo>;

  /**
   * Load the versions with available changelogs from the server
   */
  loadVersionHistory(): FetchResponse<VersionHistory>;

  /**
   * Load a changelog for a specified version
   * @param version the changelogs version
   */
  loadChangelog(version: string): FetchResponse<string>;

  /**
   * Create a new online game
   * @param name the games name
   * @param isPublic whether the game is public
   * @param password the games password (empty if public)
   */
  createGame(name: string, isPublic: boolean, password: string): FetchResponse<GameJoinResponse>;

  /**
   * Load all games that a currently available
   */
  loadAvailableGames(): FetchResponse<GamesList>;

  /**
   * Load meta information of a game
   * @param gameId the games id
   */
  loadGameMeta(gameId: number): FetchResponse<GameMeta>;

  /**
   * Join a game
   * @param gameId the games id to join
   * @param role the players role
   * @param password the password of the game
   */
  joinGame(gameId: number, role: ZRPRole, password: string): FetchResponse<GameJoinResponse>;

  /**
   * Load the current leader board
   */
  loadLeaderBoard(): FetchResponse<Leaderboard>;

  /**
   * Load the currents user position in the leader board
   */
  loadOwnLeaderBoardPosition(): FetchResponse<LeaderboardPosition>;

  /**
   * Submit a contact request
   * @param sender the senders name
   * @param message teh senders message
   */
  submitContactForm(form: ContactForm): FetchResponse<void>;

  /**
   * exposes the raw fetch method
   * @param url the request url
   * @param init the request options
   */
  fetchRaw<T>(url: string, init: FetchOptions<T>): FetchResponse<T>;
}
