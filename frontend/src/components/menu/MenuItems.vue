<template>
  <div class="flex flex-col sm:flex-row tc-main-dark">
    <Environment show="online">
      <div v-if="!isLoggedIn">
        <router-link to="/login" class="link">
          {{ t('nav.login') }}
        </router-link>
        <router-link to="/create-account" class="link">
          {{ t('nav.createAccount') }}
        </router-link>
        <div class="divider bc-invert-main"></div>
      </div>
    </Environment>
    <Environment show="offline">
      <router-link to="/home" class="link">
        {{ t('nav.home') }}
      </router-link>
      <div class="divider bc-invert-main"></div>
    </Environment>
    <Environment show="online">
      <div v-if="isLoggedIn" class="sm:flex flex-row">
        <router-link to="/home" class="link">
          {{ t('nav.home') }}
        </router-link>
        <div class="divider bc-invert-main"></div>
        <router-link to="/create-game" class="link">
          {{ t('nav.createGame') }}
        </router-link>
        <router-link to="/available-games" class="link">
          {{ t('nav.joinGame') }}
        </router-link>
        <div class="divider bc-invert-main"></div>
      </div>
    </Environment>
    <div>
      <router-link to="/imprint" class="link">
        {{ t('nav.imprint') }}
      </router-link>
      <router-link to="/settings" class="link">
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

const { t } = useI18n();

const auth = useAuth();
const isLoggedIn = computed(() => auth.isLoggedIn);
</script>

<style scoped>
.divider {
  @apply h-0 my-1 border border-solid border-t-0 opacity-50 sm:hidden;
}

.link {
  @apply block cursor-pointer transition-transform px-3 my-2 text-lg hover:translate-x-4;
}

.link:hover::before {
  content: '⮞';
  @apply fixed -left-2 opacity-100;
}

.link::before {
  content: '';
  @apply opacity-0 transition-opacity;
}
</style>
