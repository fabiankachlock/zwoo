/* eslint-disable @typescript-eslint/no-unused-vars */
import { ApiAdapter } from '../ApiAdapter';
import { FetchResponse } from '../ApiEntities';

// eslint-disable-next-line @typescript-eslint/no-explicit-any
const result: FetchResponse<any> = Promise.resolve({
  isFallback: false,
  isError: false,
  wasSuccessful: true,
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  data: {} as any,
  error: undefined
});

export const NoopApi: ApiAdapter = {
  getServer: () => '',
  changeUserPassword: (..._args) => result,
  createGame: (..._args) => result,
  createUserAccount: (..._args) => result,
  deleteUserAccount: (..._args) => result,
  joinGame: (..._args) => result,
  loadAvailableGames: (..._args) => result,
  loadChangelog: (..._args) => result,
  loadGameMeta: (..._args) => result,
  loadLeaderBoard: (..._args) => result,
  loadOwnLeaderBoardPosition: (..._args) => result,
  loadUserInfo: (..._args) => result,
  loadUserSettings: (..._args) => result,
  checkVersion: (..._args) => result,
  loadVersionHistory: (..._args) => result,
  loginUser: (..._args) => result,
  logoutUser: (..._args) => result,
  requestUserPasswordReset: (..._args) => result,
  resendVerificationEmail: (..._args) => result,
  resetUserPassword: (..._args) => result,
  storeUserSettings: (..._args) => result,
  submitContactForm: (..._args) => result,
  verifyUserAccount: (..._args) => result,
  generateJoinUrl: (..._args) => '',
  fetchRaw: (..._args) => result
};
