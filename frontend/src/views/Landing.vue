<template>
  <div class="relative w-full p-5 py-10 max-w-lg mx-auto">
    <div class="w-full flex justify-center">
      <img style="width: 10rem; max-width: 80%" src="/img/logo/zwoo_logo_none.svg" alt="" />
    </div>
    <h1 class="text-6xl tc-primary text-center">zwoo</h1>
    <p class="text-2xl italic tc-main text-center">{{ t('landing.tagline') }}</p>
    <div class="relative w-full flex flex-col my-3 px-5">
      <div class="action bg-dark hover:bg-darkest">
        <router-link class="link" to="/login">
          <Icon class="icon tc-secondary" icon="mdi:login-variant" />
          <p class="tc-main-light">{{ t('landing.login') }}</p>
        </router-link>
      </div>
      <div class="action bg-dark hover:bg-darkest">
        <router-link class="link" to="/create-account">
          <Icon class="icon tc-secondary" icon="ic:outline-add-box" />
          <p class="tc-main-light">{{ t('landing.create') }}</p>
        </router-link>
      </div>
      <div class="action bg-dark hover:bg-darkest">
        <router-link class="link" to="/leaderboard">
          <Icon class="icon tc-secondary" icon="mdi:trophy-outline" />
          <p class="tc-main-light">{{ t('landing.leaderboard') }}</p>
        </router-link>
      </div>
      <!-- TODO tmp(beta): <div class="action bg-dark hover:bg-darkest">
        <router-link class="link" to="/tutorial">
          <Icon class="icon tc-secondary" icon="mdi:arrow-right-bold-box-outline" />
          <p class="tc-main-light">{{ t('landing.tutorial') }}</p>
        </router-link>
      </div> -->
    </div>
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
</script>

<style scoped>
.action {
  @apply px-4 py-1 rounded my-2;
}

.link {
  @apply flex flex-row items-center;
}

.action p {
  @apply inline-block mx-1;
}

.icon {
  @apply transform transition-transform inline-block mx-1;
}

.action:hover .icon {
  @apply scale-125;
}
</style>
