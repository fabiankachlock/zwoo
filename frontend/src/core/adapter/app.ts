import { defineStore } from 'pinia';

export const useRootApp = defineStore('app', {
  state: () => {
    return {
      updateAvailable: false,
      offlineReady: false,
      // eslint-disable-next-line @typescript-eslint/no-empty-function
      _updateFunc: (() => {}) as unknown as (reload: boolean) => Promise<void>
    };
  },
  actions: {
    _setUpdateFunc(func: (reload: boolean) => Promise<void>) {
      this._updateFunc = func;
    },
    onOfflineReady() {
      this.offlineReady = true;
    },
    onNeedsRefresh() {
      this.updateAvailable = true;
    }
  }
});
