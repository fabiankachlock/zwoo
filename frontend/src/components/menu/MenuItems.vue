<template>
  <div class="flex flex-col tc-main-dark">
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
        <div class="divider bc-invert-main"></div>
      </div>
    </Environment>
    <Environment show="offline">
      <router-link to="/home" class="link">
        <Icon icon="material-symbols:keyboard-double-arrow-right-rounded" />
        {{ t('nav.home') }}
      </router-link>
      <div class="divider bc-invert-main"></div>
    </Environment>
    <Environment show="online">
      <div v-if="isLoggedIn">
        <router-link to="/home" class="link">
          <Icon icon="material-symbols:keyboard-double-arrow-right-rounded" />
          {{ t('nav.home') }}
        </router-link>
        <div class="divider bc-invert-main"></div>
        <router-link to="/create-game" class="link">
          <Icon icon="material-symbols:keyboard-double-arrow-right-rounded" />
          {{ t('nav.createGame') }}
        </router-link>
        <router-link to="/available-games" class="link">
          <Icon icon="material-symbols:keyboard-double-arrow-right-rounded" />
          {{ t('nav.joinGame') }}
        </router-link>
        <div class="divider bc-invert-main"></div>
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
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';

import { useAuth } from '@/core/adapter/auth';

import Environment from '../misc/Environment.vue';
import { Icon } from '../misc/Icon';

const { t } = useI18n();

const auth = useAuth();
const isLoggedIn = computed(() => auth.isLoggedIn);
</script>

<style scoped>
.divider {
  @apply h-0 my-1 border border-solid border-t-0 opacity-50;
}

.link {
  @apply relative flex items-center cursor-pointer transition-transform px-3 my-2 text-lg -translate-x-4 hover:translate-x-0;
}

.link::before {
  content: '';
  @apply absolute left-0 bg-darkest top-0 bottom-0 w-7 transition-all;
}

.link:hover::before {
  @apply w-0;
}
</style>
