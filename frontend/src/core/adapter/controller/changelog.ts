import { defineStore } from 'pinia';

import { useRootApp } from '../app';
import { MigrationRunner } from '../migrations/MigrationRunner';

export const useChangelog = defineStore('changelog', {
  state: () => ({
    popupOpen: false,
    version: ''
  }),

  actions: {
    setup() {
      const lastVersion = MigrationRunner.lastVersion;
      const version = useRootApp().versionInfo.version;

      if (lastVersion) {
        this.popupOpen = lastVersion !== version;
        this.version = version;
      }
    },
    didShowDialog() {
      this.popupOpen = false;
    }
  }
});
