import { defineStore } from 'pinia';

export const useSearch = defineStore('search', {
  state: () => ({
    isOpen: false
  }),
  actions: {
    openSearch() {
      this.isOpen = true;
    },
    closeSearch() {
      this.isOpen = false;
    }
  }
});
