<template>
  <footer class="h-full">
    <div class="h-8 py-1 grid grid-cols-3 items-center mx-3">
      <div class="flex items-center">
        <router-link to="/">
          <Icon icon="mi:home" class="text-xl text-text hover:scale-110 transition-transform" />
        </router-link>
        <Environment show="offline">
          <Icon icon="ic:baseline-wifi-off" class="ml-3 text-xs text-text-secondary"></Icon>
          <p class="text-xs text-text-secondary ml-1">{{ t('nav.statusOffline') }}</p>
        </Environment>
        <Environment show="local">
          <Icon icon="akar-icons:link-chain" class="ml-3 text-xs text-text-secondary"></Icon>
          <p class="text-xs text-text-secondary ml-1 none hidden sm:block">
            {{ t('nav.statusLocal') }}<span class="hidden lg:inline-block">: {{ server }}</span>
          </p>
        </Environment>
      </div>
      <div class="flex justify-center items-center">
        <!-- <span class="footer-item">© {{ year }}</span> -->
        <router-link to="/imprint" class="footer-item">{{ t('nav.imprint') }}</router-link>
        <router-link to="/privacy" class="footer-item">{{ t('nav.privacy') }}</router-link>
        <Environment show="online">
          <router-link to="/contact" class="footer-item">{{ t('nav.contact') }}</router-link>
        </Environment>
      </div>
      <!-- TODO: implement
        <button class="help flex items-center">
        <Icon icon="material-symbols:help-outline-rounded" class="text-xl text-text mr-1 transition-transform" />
        <span class="text-text"> {{ t('nav.help') }} </span>
      </button> -->
      <div></div>
    </div>
  </footer>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';

import Environment from '@/components/misc/Environment.vue';
import { Icon } from '@/components/misc/Icon';
import { useApi } from '@/core/adapter/helper/useApi';

const { t } = useI18n();
const api = useApi();

const server = computed(() => {
  return api.getServer().replace(/(^\w+:|^)\/\//, '');
});
// const year = new Date().getFullYear();
</script>

<style lang="css" scoped>
.footer-item:not(:last-child) {
  @apply border-r border-border px-3;
}

a.footer-item {
  text-decoration: underline;
}

.footer-item,
.footer-item:last-child {
  @apply px-3 text-text-secondary;
}

.help:hover #icon {
  @apply scale-110;
}
</style>
