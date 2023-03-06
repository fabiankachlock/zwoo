<template>
  <MaxWidthLayout size="small" class="pt-10">
    <div class="w-full flex justify-center">
      <img style="max-width: 10rem" src="/img/logo/zwoo_logo_none.svg" alt="" />
    </div>
    <h1 class="text-6xl tc-primary text-center">zwoo</h1>
    <p class="text-2xl italic tc-main text-center">{{ t('landing.tagline') }}</p>
    <Environment show="offline">
      <div class="flex justify-center items-center">
        <Icon icon="ic:baseline-wifi-off" class="text-sm tc-main-secondary"></Icon>
        <p class="text-sm tc-main-secondary ml-1">{{ t('offline.statusOffline') }}</p>
      </div>
    </Environment>
    <div class="relative w-full flex flex-col my-3 px-5">
      <div class="relative flex-1 w-full">
        <div class="action bg-dark hover:bg-darkest cursor-pointer">
          <div @click="create" class="link">
            <Icon class="icon tc-secondary" icon="fluent:window-new-16-regular" />
            <p class="tc-main-light">{{ t('offline.create') }}</p>
          </div>
        </div>
      </div>
    </div>
  </MaxWidthLayout>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';

import Environment from '@/components/misc/Environment.vue';
import { Icon } from '@/components/misc/Icon';
import { useGameConfig } from '@/core/adapter/game';
import { SnackBarPosition, useSnackbar } from '@/core/adapter/snackbar';
import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';

const { t } = useI18n();
const gameConfig = useGameConfig();
const router = useRouter();
const snackbar = useSnackbar();

const create = async () => {
  try {
    await gameConfig.create(t('offline.gameName'), true, '');
    router.push('/game/wait');
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
