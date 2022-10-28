import { Ref, ref } from 'vue';

export const useShare = (): {
  canShare: Ref<boolean>;
  share: (options: { title: string; text: string; url?: string; files?: File[] }) => Promise<void>;
} => ({
  canShare: ref('share' in navigator),
  share: (options: { title: string; text: string; url?: string; files?: File[] }): Promise<void> =>
    new Promise((resolve, reject) => {
      try {
        if (navigator.share) {
          navigator
            .share(options)
            .then(() => resolve())
            .catch(() => reject('share.error'));
        } else {
          reject('share.notAvailable');
        }
      } catch (e: unknown) {
        reject('share.error');
      }
    })
});
