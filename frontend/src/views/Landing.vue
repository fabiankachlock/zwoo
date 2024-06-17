<template>
  <MaxWidthLayout size="small" classes="pt-10">
    <div class="w-full flex justify-center">
      <div style="max-width: 10rem" class="logo" v-html="Logo"></div>
    </div>
    <h1 class="text-6xl text-primary-text text-center">zwoo</h1>
    <p class="text-2xl italic text-text text-center">{{ t('landing.tagline') }}</p>
    <div class="relative w-full flex flex-col my-3 px-5">
      <div class="action list">
        <router-link class="link" to="/login">
          <Icon class="icon" icon="mdi:login-variant" />
          <p class="text-text">{{ t('landing.login') }}</p>
        </router-link>
      </div>
      <div class="action list">
        <router-link class="link" to="/create-account">
          <Icon class="icon" icon="ic:outline-add-box" />
          <p class="text-text">{{ t('landing.create') }}</p>
        </router-link>
      </div>
      <div class="action list">
        <router-link class="link" to="/leaderboard">
          <Icon class="icon" icon="mdi:trophy-outline" />
          <p class="text-text">{{ t('landing.leaderboard') }}</p>
        </router-link>
      </div>
      <!-- TODO tmp(beta): <div class="action bg-surface hover:bg-darkest">
        <router-link class="link" to="/tutorial">
          <Icon class="icon text-secondary-text" icon="mdi:arrow-right-bold-box-outline" />
          <p class="text-text">{{ t('landing.tutorial') }}</p>
        </router-link>
      </div> -->
    </div>
  </MaxWidthLayout>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';

import Logo from '@/assets/zwoo_logo_none_auto.svg?raw';
import { Icon } from '@/components/misc/Icon';
import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';

const { t } = useI18n();
</script>

<style scoped>
.action {
  @apply bg-alt hover:bg-alt-hover;
}

.action.list {
  @apply px-4 py-1 rounded m-2;
}

.action.list a {
  @apply flex flex-row items-center;
}

.icon {
  @apply text-primary-text transform transition-transform inline-block mx-1;
}

.action p {
  @apply inline-block mx-1;
}

.action:hover .icon {
  @apply scale-125;
}

.logo :deep(svg) {
  width: 100%;
  height: auto;
}
</style>
