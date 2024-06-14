<template>
  <MaxWidthLayout size="small" class="pt-10">
    <div class="w-full flex justify-center">
      <div style="max-width: 10rem" class="logo" v-html="Logo"></div>
    </div>
    <h1 class="text-6xl tc-primary text-center">zwoo</h1>
    <p class="text-2xl italic tc-main text-center">{{ t('landing.tagline') }}</p>
    <div class="relative w-full flex flex-col my-3 px-5">
      <div class="relative w-full">
        <p class="tc-main text-center">
          {{ greeting }}
        </p>
      </div>
      <!-- Add stats when implemented -->
      <div class="relative flex-1 w-full grid grid-cols-2">
        <div class="action main bg-dark hover:bg-darkest">
          <router-link class="link" to="/create-game">
            <Icon class="icon tc-secondary" icon="fluent:window-new-16-regular" />
            <p class="tc-main-light">{{ t('home.create') }}</p>
          </router-link>
        </div>
        <div class="action main bg-dark hover:bg-darkest">
          <router-link class="link" to="/available-games">
            <Icon class="icon tc-secondary" icon="fluent:square-arrow-forward-32-regular" />
            <p class="tc-main-light">{{ t('home.join') }}</p>
          </router-link>
        </div>
        <!--
          <div class="action main bg-dark hover:bg-darkest">
            <router-link class="link" to="/stats">
            <Icon class="icon tc-secondary" icon="fluent:ribbon-star-20-regular" />
            <p class="tc-main-light">{{ t('home.stats') }}</p>
          </router-link>
        </div>
        -->
      </div>
      <Environment show="online">
        <div class="action px-4 py-1 rounded m-2 bg-dark hover:bg-darkest">
          <router-link class="flex flex-row items-center" to="/leaderboard">
            <Icon class="icon tc-secondary" icon="mdi:trophy-outline" />
            <p class="tc-main-light inline-block mx-1">{{ t('landing.leaderboard') }}</p>
          </router-link>
        </div>
      </Environment>
      <div v-if="!AppConfig.IsTauri" class="action px-4 py-1 rounded m-2 bg-dark hover:bg-darkest">
        <router-link class="flex flex-row items-center" to="/login-local">
          <Icon class="icon tc-secondary" icon="akar-icons:link-chain" />
          <p class="tc-main-light inline-block mx-1">{{ t('home.localGame') }}</p>
        </router-link>
      </div>
      <!-- TODO tmp(beta): <div class="action px-4 py-1 rounded m-2 bg-dark hover:bg-darkest">
        <router-link class="flex flex-row items-center" to="/tutorial">
          <Icon class="icon tc-secondary" icon="mdi:arrow-right-bold-box-outline" />
          <p class="tc-main-light inline-block mx-1">{{ t('landing.tutorial') }}</p>
        </router-link>
      </div> -->
    </div>
  </MaxWidthLayout>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';

import Logo from '@/assets/zwoo_logo_none_auto.svg?raw';
import Environment from '@/components/misc/Environment.vue';
import { Icon } from '@/components/misc/Icon';
import { AppConfig } from '@/config';
import { useAuth } from '@/core/adapter/auth';
import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';

const { t } = useI18n();
const auth = useAuth();
const greeting = computed(() => t('home.greeting', { name: auth.username }));
</script>

<style scoped>
.action.main {
  @apply m-2 p-2 rounded flex-1;
}

.link {
  @apply flex flex-col items-center justify-evenly;
}

.link .icon {
  @apply text-5xl;
}

.action p {
  @apply my-1 text-center;
}

.icon {
  @apply transform transition-transform inline-block mx-1;
  color: var(--color-secondary-text);
}

.action:hover .icon {
  @apply scale-125;
}

.logo :deep(svg) {
  width: 100%;
  height: auto;
}
</style>
