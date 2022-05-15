<template>
  <div class="info m-8 mb-20 tc-main-secondary lg:max-w-3xl lg:mx-auto">
    <h1 class="tc-main">{{ t('shortcuts.title') }}</h1>

    <h2 class="tc-main">{{ t('shortcuts.ui') }}</h2>
    <p><span>Ctrl</span>+<span>d</span>: {{ t('shortcuts.darkMode') }}</p>
    <p><span>Ctrl</span>+<span>q</span>: {{ t('shortcuts.quickMenu') }}</p>
    <p><span>Ctrl</span>+<span>f</span>: {{ t('shortcuts.fullScreen') }}</p>

    <h2 class="tc-main">{{ t('shortcuts.navigation') }}</h2>
    <p><span>Ctrl</span>+<span>h</span>: {{ t('shortcuts.home') }}</p>
    <p><span>Ctrl</span>+<span>n</span>: {{ t('shortcuts.newGame') }}</p>
    <p><span>Ctrl</span>+<span>j</span>: {{ t('shortcuts.join') }}</p>
    <p><span>Ctrl</span>+<span>s</span>: {{ t('shortcuts.settings') }}</p>
    <p><span>Ctrl</span>+<span>l</span>: {{ t('shortcuts.leaderboard') }}</p>
    <p><span>Ctrl</span>+<span>a</span>: {{ t('shortcuts.login') }}</p>
    <p><span>Ctrl</span>+ <span>i</span>, <span>?</span>: {{ t('shortcuts.info') }}</p>
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

.info p span {
  @apply bg-_bg-light-dark dark:bg-_bg-dark-dark px-2 py-0.5 rounded;
}

.info h1 {
  @apply mt-4 mb-2 text-4xl;
}

.info h2 {
  @apply mt-4 mb-2 text-2xl;
}
</style>
