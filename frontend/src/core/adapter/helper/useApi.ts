import { ApiAdapter } from '@/core/api/ApiAdapter';

import { useRootApp } from '../app';

const ApiRef: ApiAdapter = {
  changeUserPassword: (...args) => useRootApp().api.changeUserPassword(...args),
  createGame: (...args) => useRootApp().api.createGame(...args),
  createUserAccount: (...args) => useRootApp().api.createUserAccount(...args),
  deleteUserAccount: (...args) => useRootApp().api.deleteUserAccount(...args),
  joinGame: (...args) => useRootApp().api.joinGame(...args),
  loadAvailableGames: (...args) => useRootApp().api.loadAvailableGames(...args),
  loadChangelog: (...args) => useRootApp().api.loadChangelog(...args),
  loadGameMeta: (...args) => useRootApp().api.loadGameMeta(...args),
  loadLeaderBoard: (...args) => useRootApp().api.loadLeaderBoard(...args),
  loadOwnLeaderBoardPosition: (...args) => useRootApp().api.loadOwnLeaderBoardPosition(...args),
  loadUserInfo: (...args) => useRootApp().api.loadUserInfo(...args),
  loadUserSettings: (...args) => useRootApp().api.loadUserSettings(...args),
  loadVersion: (...args) => useRootApp().api.loadVersion(...args),
  loadVersionHistory: (...args) => useRootApp().api.loadVersionHistory(...args),
  loginUser: (...args) => useRootApp().api.loginUser(...args),
  logoutUser: (...args) => useRootApp().api.logoutUser(...args),
  verifyCaptchaToken: (...args) => useRootApp().api.verifyCaptchaToken(...args),
  requestUserPasswordReset: (...args) => useRootApp().api.requestUserPasswordReset(...args),
  resendVerificationEmail: (...args) => useRootApp().api.resendVerificationEmail(...args),
  resetUserPassword: (...args) => useRootApp().api.resetUserPassword(...args),
  storeUserSettings: (...args) => useRootApp().api.storeUserSettings(...args),
  submitContactForm: (...args) => useRootApp().api.submitContactForm(...args),
  verifyUserAccount: (...args) => useRootApp().api.verifyUserAccount(...args),
  generateJoinUrl: (...args) => useRootApp().api.generateJoinUrl(...args),
  fetchRaw: (...args) => useRootApp().api.fetchRaw(...args)
};

export const useApi = () => ApiRef;
