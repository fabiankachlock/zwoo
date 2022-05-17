<template>
  <div class="info m-8 mb-20 tc-main-secondary lg:max-w-3xl lg:mx-auto">
    <h1 class="tc-main">{{ t('shortcuts.title') }}</h1>

    <h2 class="tc-main">{{ t('shortcuts.ui') }}</h2>
    <p><kbd>d</kbd>: {{ t('shortcuts.darkMode') }}</p>
    <p><kbd>q</kbd>: {{ t('shortcuts.quickMenu') }}</p>
    <p><kbd>f</kbd>: {{ t('shortcuts.fullScreen') }}</p>

    <h2 class="tc-main">{{ t('shortcuts.navigation') }}</h2>
    <p><kbd>h</kbd>: {{ t('shortcuts.home') }}</p>
    <p><kbd>n</kbd>: {{ t('shortcuts.newGame') }}</p>
    <p><kbd>j</kbd>: {{ t('shortcuts.join') }}</p>
    <p><kbd>s</kbd>: {{ t('shortcuts.settings') }}</p>
    <p><kbd>l</kbd>: {{ t('shortcuts.leaderboard') }}</p>
    <p><kbd>a</kbd>: {{ t('shortcuts.login') }}</p>
    <p><kbd>i</kbd>, <kbd>?</kbd>: {{ t('shortcuts.info') }}</p>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
</script>

<style>
.info p {
  @apply my-2 text-lg;
}

.info p kbd {
  @apply bg-_bg-light-dark dark:bg-_bg-dark-dark px-2 py-0.5 rounded;
  user-select: none;
}

.info h1 {
  @apply mt-4 mb-2 text-4xl;
}

.info h2 {
  @apply mt-4 mb-2 text-2xl;
}
</style>
