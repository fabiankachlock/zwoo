<template>
  <div class="flex flex-col sm:flex-row gap-1 sm:gap-3">
    <div v-if="!isLoggedIn">
      <NavBarItem :title="t('nav.login')" link="/login" />
    </div>
    <div v-if="!isLoggedIn">
      <NavBarItem :title="t('nav.createAccount')" link="/create-account" />
      <div class="divider bc-invert-main"></div>
    </div>
    <div v-if="isLoggedIn" class="sm:flex flex-row">
      <NavBarItem :title="t('nav.home')" link="/home" />
      <div class="divider bc-invert-main"></div>
    </div>
    <NavBarItem :title="t('nav.idea')" link="https://github.com/fabiankachlock/zwoo/discussions/categories/ideas" is-external />
    <div class="divider bc-invert-main"></div>
    <NavBarItem :title="t('nav.imprint')" link="/imprint" />
    <NavBarItem :title="t('nav.settings')" link="/settings" />
    <div v-if="isLoggedIn">
      <NavBarItem :title="t('nav.logout')" link="/logout" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuth } from '@/core/adapter/auth';
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';
import NavBarItem from './NavBarItem.vue';

const { t } = useI18n();

const auth = useAuth();
const isLoggedIn = computed(() => auth.isLoggedIn);
</script>

<style scoped>
.divider {
  @apply h-0 my-2 border border-solid border-t-0 opacity-50 sm:hidden;
}
</style>
