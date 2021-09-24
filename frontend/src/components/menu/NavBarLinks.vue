<template>
  <div v-if="!isLoggedIn" class="md:flex flex-row">
    <NavBarItem :title="t('nav.login')" link="/login" />
    <NavBarItem :title="t('nav.createAccount')" link="/create-account" />
    <div class="divider bc-invert-main"></div>
  </div>
  <NavBarItem :title="t('nav.idea')" link="https://github.com/fabiankachlock/zwoo/discussions/categories/ideas" is-external />
  <NavBarItem :title="t('nav.github')" link="https://github.com/fabiankachlock/zwoo" is-external />
  <div class="divider bc-invert-main"></div>
  <NavBarItem :title="t('nav.settings')" link="/settings" />
  <div v-if="isLoggedIn" class="md:flex flex-row">
    <div class="divider bc-invert-main"></div>
    <NavBarItem :title="t('nav.logout')" link="/logout" />
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue';
import { useI18n } from 'vue-i18n';
import NavBarItem from './NavBarItem.vue';

const { t } = useI18n();

defineProps<{
  isLoggedIn?: boolean;
}>();
</script>

<style scoped>
.divider {
  @apply h-0 my-2 border border-solid border-t-0 opacity-50 md:hidden;
}
</style>
