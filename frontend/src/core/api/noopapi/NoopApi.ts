/* eslint-disable @typescript-eslint/no-unused-vars */
import { ApiAdapter } from '../ApiAdapter';

export const NoopApi: ApiAdapter = {
  changeUserPassword: (..._args) => Promise.resolve({}),
  createGame: (..._args) => Promise.resolve({}),
  createUserAccount: (..._args) => Promise.resolve({}),
  deleteUserAccount: (..._args) => Promise.resolve({}),
  joinGame: (..._args) => Promise.resolve({}),
  loadAvailableGames: (..._args) => Promise.resolve({}),
  loadChangelog: (..._args) => Promise.resolve({}),
  loadGameMeta: (..._args) => Promise.resolve({}),
  loadLeaderBoard: (..._args) => Promise.resolve({}),
  loadOwnLeaderBoardPosition: (..._args) => Promise.resolve({}),
  loadUserInfo: (..._args) => Promise.resolve({}),
  loadUserSettings: (..._args) => Promise.resolve(''),
  loadVersion: (..._args) => Promise.resolve({}),
  loadVersionHistory: (..._args) => Promise.resolve({}),
  loginUser: (..._args) => Promise.resolve({}),
  logoutUser: (..._args) => Promise.resolve({}),
  requestUserPasswordReset: (..._args) => Promise.resolve({}),
  resendVerificationEmail: (..._args) => Promise.resolve({}),
  resetUserPassword: (..._args) => Promise.resolve({}),
  storeUserSettings: (..._args) => Promise.resolve({}),
  submitContactForm: (..._args) => Promise.resolve({}),
  verifyUserAccount: (..._args) => Promise.resolve({}),
  generateJoinUrl: (..._args) => '',
  fetchRaw: (..._args) => Promise.resolve({})
};
