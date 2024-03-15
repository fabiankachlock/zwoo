<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';

const { t } = useI18n();
const router = useRouter();

const logIn = () => {
  router.push('/login-local');
};
</script>

<template>
  <button
    class="flex justify-center items-center bg-light border-2 border-transparent px-2 rounded transition hover:bg-main cursor-pointer select-none"
    @click="logIn"
  >
    <p class="tc-main-light text-center">{{ t('settings.localServer.login') }}</p>
  </button>
</template>
