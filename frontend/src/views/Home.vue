<template>
  <MaxWidthLayout size="small" class="pt-10">
    <div class="w-full flex justify-center">
      <div style="max-width: 10rem" class="logo" v-html="Logo"></div>
    </div>
    <h1 class="text-6xl text-primary-text text-center">zwoo</h1>
    <p class="text-2xl italic text-text text-center">{{ t('landing.tagline') }}</p>
    <div class="relative w-full flex flex-col my-3 px-5">
      <div class="relative w-full">
        <p class="text-text text-center">
          {{ greeting }}
        </p>
      </div>
      <!-- Add stats when implemented -->
      <div class="relative flex-1 w-full grid grid-cols-2">
        <div class="action main">
          <router-link to="/create-game">
            <Icon class="icon" icon="fluent:window-new-16-regular" />
            <p class="text-text">{{ t('home.create') }}</p>
          </router-link>
        </div>
        <div class="action main">
          <router-link to="/available-games">
            <Icon class="icon" icon="fluent:square-arrow-forward-32-regular" />
            <p class="text-text">{{ t('home.join') }}</p>
          </router-link>
        </div>
        <!--
          <div class="action main">
            <router-link class="link" to="/stats">
            <Icon class="icon text-secondary-text" icon="fluent:ribbon-star-20-regular" />
            <p class="text-text">{{ t('home.stats') }}</p>
          </router-link>
        </div>
        -->
      </div>
      <Environment show="online">
        <div class="action list">
          <router-link to="/leaderboard">
            <Icon class="icon" icon="mdi:trophy-outline" />
            <p class="text-text inline-block mx-1">{{ t('landing.leaderboard') }}</p>
          </router-link>
        </div>
      </Environment>
      <Environment :exclude="['local']">
        <Platform :exclude="['linux']">
          <div class="action list">
            <router-link class="flex flex-row items-center" to="/login-local">
              <Icon class="icon text-secondary-text" icon="akar-icons:link-chain" />
              <p class="text-text inline-block mx-1">{{ t('home.localGame') }}</p>
            </router-link>
          </div>
        </Platform>
      </Environment>
      <!-- TODO tmp(beta): <div class="action list">
        <router-link class="flex flex-row items-center" to="/tutorial">
          <Icon class="icon text-secondary-text" icon="mdi:arrow-right-bold-box-outline" />
          <p class="text-text inline-block mx-1">{{ t('landing.tutorial') }}</p>
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
import Platform from '@/components/misc/Platform.vue';
import { useAuth } from '@/core/adapter/auth';
import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';

const { t } = useI18n();
const auth = useAuth();
const greeting = computed(() => t('home.greeting', { name: auth.username }));
</script>

<style scoped>
.action {
  @apply bg-alt hover:bg-alt-hover;
}

.action.main {
  @apply m-2 p-2 rounded flex-1;
}

.action.list {
  @apply px-4 py-1 rounded m-2;
}

.action.list a {
  @apply flex flex-row items-center;
}

.action.main a {
  @apply flex flex-col items-center justify-evenly;
}

.action.main .icon {
  @apply text-5xl;
}

.icon {
  @apply text-primary-text transform transition-transform inline-block mx-1;
}

.action p {
  @apply my-1 text-center;
}

.action:hover .icon {
  @apply scale-125;
}

.logo :deep(svg) {
  width: 100%;
  height: auto;
}
</style>
