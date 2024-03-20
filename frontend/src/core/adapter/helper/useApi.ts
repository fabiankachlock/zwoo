import { ApiAdapter } from '@/core/api/ApiAdapter';
import { GameAdapter } from '@/core/api/GameAdapter';

import { useRootApp } from '../app';

const ApiRef: ApiAdapter & GameAdapter = {
  getServer: () => useRootApp().api.getServer(),
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
  checkVersion: (...args) => useRootApp().api.checkVersion(...args),
  loadVersionHistory: (...args) => useRootApp().api.loadVersionHistory(...args),
  loginUser: (...args) => useRootApp().api.loginUser(...args),
  logoutUser: (...args) => useRootApp().api.logoutUser(...args),
  requestUserPasswordReset: (...args) => useRootApp().api.requestUserPasswordReset(...args),
  resendVerificationEmail: (...args) => useRootApp().api.resendVerificationEmail(...args),
  resetUserPassword: (...args) => useRootApp().api.resetUserPassword(...args),
  storeUserSettings: (...args) => useRootApp().api.storeUserSettings(...args),
  submitContactForm: (...args) => useRootApp().api.submitContactForm(...args),
  verifyUserAccount: (...args) => useRootApp().api.verifyUserAccount(...args),
  fetchRaw: (...args) => useRootApp().api.fetchRaw(...args),

  createConnection: (...args) => useRootApp().realtimeApi.createConnection(...args)
};

export const useApi = () => ApiRef;
