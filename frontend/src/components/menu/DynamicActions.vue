<template>
  <Environment show="offline">
    <div class="hidden xs:block tc-main">
      <router-link to="/" class="link">
        <Icon icon="mi:home" class="tc-primary"></Icon>
        <span>
          {{ t('nav.home') }}
        </span>
      </router-link>
    </div>
  </Environment>
  <Environment :include="['online', 'local']">
    <div class="flex items-center tc-main">
      <div class="hidden lg:block">
        <router-link to="/" class="link">
          <Icon icon="mi:home" class="tc-primary"></Icon>
          <span>
            {{ t('nav.home') }}
          </span>
        </router-link>
      </div>
      <div class="hidden xs:block">
        <router-link v-if="isLoggedIn" to="/available-games" class="link">
          <Icon icon="fluent:square-arrow-forward-32-regular" class="tc-primary"></Icon>
          <span>
            {{ t('nav.joinGame') }}
          </span>
        </router-link>
        <router-link v-else to="/login" class="link">
          <Icon icon="mdi:login-variant" class="tc-primary"></Icon>
          <span>
            {{ t('nav.login') }}
          </span>
        </router-link>
      </div>
      <div class="hidden sm:block">
        <router-link v-if="isLoggedIn" to="/create-game" class="link">
          <Icon icon="fluent:window-new-16-regular" class="tc-primary"></Icon>
          <span>
            {{ t('nav.createGame') }}
          </span>
        </router-link>
        <Environment show="online">
          <router-link v-if="!isLoggedIn" to="/create-account" class="link">
            <Icon icon="ic:outline-add-box" class="tc-primary"></Icon>
            <span>
              {{ t('nav.createAccount') }}
            </span>
          </router-link>
        </Environment>
      </div>
    </div>
  </Environment>
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
.link {
  @apply flex items-center gap-1 mx-2;
}

.link #icon {
  @apply transition-transform duration-300;
}
.link:hover #icon {
  @apply scale-125;
}
</style>
