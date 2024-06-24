<template>
  <h3 class="text-xl text-text my-2">{{ t('wait.sharesheet.title') }}</h3>
  <p class="my-1 text-sm italic text-text-secondary">
    {{ t('wait.sharesheet.info') }}
  </p>
  <div v-if="!canShare" class="">
    <div class="flex">
      <button
        class="link-wrapper px-3 py-2 rounded-lg bg-bg text-text flex items-center mr-2 hover:bg-surface active:scale-105 transition-transform"
        @click="copyLink()"
      >
        <Icon icon="akar-icons:copy" class="mr-2 text-primary-text text-xl" />
        <span>
          {{ t('share.link') }}
        </span>
      </button>

      <a
        class="link-wrapper px-3 py-2 rounded-lg bg-bg text-text flex items-center hover:bg-surface active:scale-105 transition-transform"
        :href="`mailto:?subject=${title}&body=${encodeURIComponent(`${text}\n${url}`)}`"
        target="_self"
        rel="noopener"
        :aria-label="t('share.email')"
      >
        <Icon icon="mdi:email-outline" class="mr-2 text-primary-text text-xl" />
        <span>
          {{ t('share.email') }}
        </span>
      </a>
    </div>
  </div>
  <div class="error-message">
    <Error v-if="error" :errors="[error]" />
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

import { Icon } from '@/components/misc/Icon';
import { useShare } from '@/composables/useShare';
import { useGameConfig } from '@/core/adapter/game';
import { useServerUrl } from '@/core/adapter/helper/useServerUrl';
import Logger from '@/core/services/logging/logImport';

import Error from '../misc/Error.vue';

const emit = defineEmits<{
  (event: 'shouldClose'): void;
}>();

const game = useGameConfig();
const { t } = useI18n();
const { share, canShare } = useShare();
const url = ref('');
const title = ref('');
const text = ref('');
const error = ref<string | undefined>(undefined);

onMounted(() => {
  title.value = t('share.join.title', [game.name]);
  text.value = t('share.join.text');
  url.value = useServerUrl(game.gameId?.toString() ?? '');
  if (canShare.value) {
    try {
      share({
        title: title.value,
        text: text.value,
        url: url.value
      })
        .catch((err: string) => {
          Logger.error(`sharing error: ${err}`);
          error.value = err;
        })
        .then(() => {
          emit('shouldClose');
        });
    } catch (e: unknown) {
      error.value = (e || {}).toString();
    }
  }
});

const copyLink = () => {
  navigator.clipboard?.writeText(url.value);
};
</script>
