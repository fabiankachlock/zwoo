import { ApiAdapter } from '../ApiAdapter';
import { AccountService } from './Account';
import { AuthenticationService } from './Authentication';
import ReCaptchaService from './Captcha';
import { ConfigService } from './Config';
import { WrappedFetch } from './FetchWrapper';
import { GameManagementService } from './GameManagement';
import { LeaderBoardService } from './LeaderBoard';
import { MiscApiService } from './Misc';

export const RestApi: ApiAdapter = {
  changeUserPassword: AccountService.performChangePassword,
  createGame: GameManagementService.createGame,
  createUserAccount: AuthenticationService.performCreateAccount,
  deleteUserAccount: AuthenticationService.performDeleteAccount,
  joinGame: GameManagementService.joinGame,
  loadAvailableGames: GameManagementService.listAll,
  loadChangelog: ConfigService.fetchChangelog,
  loadGameMeta: GameManagementService.getJoinMeta,
  loadLeaderBoard: LeaderBoardService.fetchLeaderBoard,
  loadOwnLeaderBoardPosition: LeaderBoardService.fetchOwnLeaderBoardPosition,
  loadUserInfo: AuthenticationService.getUserInfo,
  loadUserSettings: AccountService.loadSettings,
  loadVersion: ConfigService.fetchVersion,
  loadVersionHistory: ConfigService.fetchVersionHistory,
  loginUser: AuthenticationService.performLogin,
  logoutUser: AuthenticationService.performLogout,
  performCaptchaCheck: ReCaptchaService.checkUser,
  requestUserPasswordReset: AccountService.requestPasswordReset,
  resendVerificationEmail: AuthenticationService.resendVerificationEmail,
  resetUserPassword: AccountService.requestPasswordReset,
  storeUserSettings: AccountService.storeSettings,
  submitContactForm: MiscApiService.submitContactForm,
  verifyUserAccount: AuthenticationService.verifyAccount,
  fetchRaw: WrappedFetch
};
