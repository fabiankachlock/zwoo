import { ApiAdapter } from '../ApiAdapter';
import { AccountService } from './Account';
import { Backend, Frontend } from './ApiConfig';
import { AuthenticationService } from './Authentication';
import { ConfigService } from './Config';
import { WrappedFetch } from './FetchWrapper';
import { GameManagementService } from './GameManagement';
import { LeaderBoardService } from './LeaderBoard';
import { MiscApiService } from './Misc';

export const RestApi = (apiUrl: string, wsOverride: string): ApiAdapter => {
  const backend = Backend.from(apiUrl, wsOverride);
  const accountService = new AccountService(backend);
  const gameManagementService = new GameManagementService(backend);
  const authenticationService = new AuthenticationService(backend);
  const configService = new ConfigService(backend);
  const miscApiService = new MiscApiService(backend);
  const leaderBoardService = new LeaderBoardService(backend);

  return {
    changeUserPassword: accountService.performChangePassword,
    createGame: gameManagementService.createGame,
    createUserAccount: accountService.performCreateAccount,
    deleteUserAccount: accountService.performDeleteAccount,
    joinGame: gameManagementService.joinGame,
    loadAvailableGames: gameManagementService.listAll,
    loadChangelog: configService.fetchChangelog,
    loadGameMeta: gameManagementService.getJoinMeta,
    loadLeaderBoard: leaderBoardService.fetchLeaderBoard,
    loadOwnLeaderBoardPosition: leaderBoardService.fetchOwnLeaderBoardPosition,
    loadUserInfo: authenticationService.getUserInfo,
    loadUserSettings: accountService.loadSettings,
    checkVersion: configService.checkVersion,
    loadVersionHistory: configService.fetchVersionHistory,
    loginUser: authenticationService.performLogin,
    logoutUser: authenticationService.performLogout,
    requestUserPasswordReset: accountService.requestPasswordReset,
    resendVerificationEmail: accountService.resendVerificationEmail,
    resetUserPassword: accountService.performResetPassword,
    storeUserSettings: accountService.storeSettings,
    submitContactForm: miscApiService.submitContactForm,
    verifyUserAccount: accountService.verifyAccount,
    fetchRaw: WrappedFetch,
    generateJoinUrl: id => `${Frontend.url}/join/${id}`
  };
};
