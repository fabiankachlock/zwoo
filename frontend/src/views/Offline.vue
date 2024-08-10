<template>
  <MaxWidthLayout size="small" class="pt-10">
    <div class="w-full flex justify-center">
      <div style="max-width: 10rem" class="logo" v-html="Logo"></div>
    </div>
    <h1 class="text-6xl text-primary-text text-center">zwoo</h1>
    <p class="text-2xl italic text-text text-center">{{ t('landing.tagline') }}</p>
    <Environment show="offline">
      <div class="flex justify-center items-center">
        <Icon icon="ic:baseline-wifi-off" class="text-sm text-text-secondary"></Icon>
        <p class="text-sm text-text-secondary ml-1">{{ t('offline.statusOffline') }}</p>
      </div>
    </Environment>
    <div class="relative w-full flex flex-col my-3 px-5">
      <div class="relative flex-1 w-full">
        <div class="action list">
          <div @click="create">
            <Icon class="icon text-secondary-text" icon="fluent:window-new-16-regular" />
            <p class="text-text">{{ t('offline.create') }}</p>
          </div>
        </div>
      </div>
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
    </div>
  </MaxWidthLayout>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';

import Logo from '@/assets/zwoo_logo_none_auto.svg?raw';
import Environment from '@/components/misc/Environment.vue';
import { Icon } from '@/components/misc/Icon';
import Platform from '@/components/misc/Platform.vue';
import { useGameConfig } from '@/core/adapter/game';
import { SnackBarPosition, useSnackbar } from '@/core/adapter/snackbar';
import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';

const { t } = useI18n();
const gameConfig = useGameConfig();
const router = useRouter();
const snackbar = useSnackbar();

const create = async () => {
  try {
    snackbar.pushMessage({
      message: 'errors.zrp.loading',
      needsTranslation: true,
      showClose: true,
      position: SnackBarPosition.Top,
      mode: 'loading',
      onClosed() {
        router.push('/game/wait');
      }
    });
    await gameConfig.create(t('offline.gameName'), true, '');
  } catch (e: unknown) {
    (Array.isArray(e) ? e : [(e as Error).toString()]).forEach(err => {
      snackbar.pushMessage({
        message: err.toString(),
        color: 'secondary',
        showClose: true,
        position: SnackBarPosition.Top
      });
    });
  }
};
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
