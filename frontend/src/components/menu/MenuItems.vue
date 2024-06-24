<template>
  <div class="flex flex-col text-text">
    <Environment show="offline">
      <div>
        <div class="flex items-center p-1">
          <Icon icon="ic:baseline-wifi-off" class="ml-3 text-xs text-text-secondary"></Icon>
          <p class="text-xs text-text-secondary ml-1">{{ t('nav.statusOffline') }}</p>
        </div>
        <div class="divider border-border-divider"></div>
      </div>
    </Environment>
    <Environment show="local">
      <div>
        <div class="flex items-center p-1">
          <Icon icon="akar-icons:link-chain" class="ml-3 text-xs text-text-secondary"></Icon>
          <p class="text-xs text-text-secondary ml-1 none">{{ t('nav.statusLocal') }}: {{ server }}</p>
        </div>
        <button class="link" style="margin-top: 0" @click="disconnect">
          <Icon icon="material-symbols:keyboard-double-arrow-right-rounded" />
          {{ t('nav.disconnect') }}
        </button>
        <div class="divider border-border-divider"></div>
      </div>
    </Environment>

    <Environment show="online">
      <div v-if="!isLoggedIn">
        <router-link to="/login" class="link">
          <Icon icon="material-symbols:keyboard-double-arrow-right-rounded" />
          {{ t('nav.login') }}
        </router-link>
        <router-link to="/create-account" class="link">
          <Icon icon="material-symbols:keyboard-double-arrow-right-rounded" />
          {{ t('nav.createAccount') }}
        </router-link>
        <div class="divider border-border-divider"></div>
      </div>
    </Environment>
    <Environment show="local">
      <div v-if="!isLoggedIn">
        <router-link to="/login" class="link">
          <Icon icon="material-symbols:keyboard-double-arrow-right-rounded" />
          {{ t('nav.login') }}
        </router-link>
        <div class="divider border-border-divider"></div>
      </div>
    </Environment>
    <Environment show="offline">
      <router-link to="/home" class="link">
        <Icon icon="material-symbols:keyboard-double-arrow-right-rounded" />
        {{ t('nav.home') }}
      </router-link>
      <div class="divider border-border-divider"></div>
    </Environment>
    <Environment :include="['online', 'local']">
      <div v-if="isLoggedIn">
        <router-link to="/home" class="link">
          <Icon icon="material-symbols:keyboard-double-arrow-right-rounded" />
          {{ t('nav.home') }}
        </router-link>
        <div class="divider border-border-divider"></div>
        <router-link to="/create-game" class="link">
          <Icon icon="material-symbols:keyboard-double-arrow-right-rounded" />
          {{ t('nav.createGame') }}
        </router-link>
        <router-link to="/available-games" class="link">
          <Icon icon="material-symbols:keyboard-double-arrow-right-rounded" />
          {{ t('nav.joinGame') }}
        </router-link>
        <div class="divider border-border-divider"></div>
      </div>
    </Environment>
    <div>
      <router-link to="/imprint" class="link">
        <Icon icon="material-symbols:keyboard-double-arrow-right-rounded" />
        {{ t('nav.imprint') }}
      </router-link>
      <router-link to="/settings" class="link">
        <Icon icon="material-symbols:keyboard-double-arrow-right-rounded" />
        {{ t('nav.settings') }}
      </router-link>
    </div>
    <Environment :include="['online', 'local']">
      <div v-if="isLoggedIn">
        <div class="divider border-border-divider"></div>
        <router-link to="/logout" class="link">
          <Icon icon="material-symbols:keyboard-double-arrow-right-rounded" />
          {{ t('nav.logout') }}
        </router-link>
      </div>
    </Environment>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';

import { useRootApp } from '@/core/adapter/app';
import { useAuth } from '@/core/adapter/auth';
import { useApi } from '@/core/adapter/helper/useApi';
import { GuestSessionManager } from '@/core/services/localGames/guestSessionManager';

import Environment from '../misc/Environment.vue';
import { Icon } from '../misc/Icon';

const { t } = useI18n();
const auth = useAuth();
const app = useRootApp();
const isLoggedIn = computed(() => auth.isLoggedIn);
const api = useApi();
const router = useRouter();

const server = computed(() => {
  const serverUrl = api.getServer().replace(/(^\w+:|^)\/\//, '');
  if (serverUrl.length < 20) {
    return serverUrl;
  }
  return `${serverUrl.substring(0, 20)}...`;
});

const disconnect = () => {
  const current = GuestSessionManager.tryGetSession();
  GuestSessionManager.destroySession();
  router.push('/');
  app.configure();
  if (current) {
    GuestSessionManager.saveSession(current);
  }
};
</script>

<style scoped>
.divider {
  @apply h-0 my-1 border border-t-0 opacity-50;
}

.link {
  @apply relative flex items-center cursor-pointer transition-transform px-3 my-2 text-lg -translate-x-4 hover:translate-x-0;
}

.link::before {
  content: '';
  @apply absolute left-0 bg-surface top-0 bottom-0 w-7 transition-all;
}

.link:hover::before {
  @apply w-0;
}
</style>
