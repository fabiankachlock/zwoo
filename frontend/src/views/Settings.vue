<template>
  <div class="max-w-4xl mx-auto">
    <h1 class="tc-main text-4xl mx-6 my-5">{{ t('settings.title') }}</h1>
    <div class="mx-6 my-5">
      <SettingsSectionGeneral />
      <SettingsSectionGame />
      <SettingsSectionDanger />
      <Version />
      <UpdateDaemon />
    </div>
    <div class="h-12"></div>
  </div>
</template>

<script setup lang="ts">
import SettingsSectionGeneral from '@/components/settings/sections/SettingsSectionGeneral.vue';
import { useI18n } from 'vue-i18n';
import Version from '@/components/misc/Version.vue';
import SettingsSectionGame from '@/components/settings/sections/SettingsSectionGame.vue';
import UpdateDaemon from '@/components/misc/UpdateDaemon.vue';
import SettingsSectionDanger from '@/components/settings/sections/SettingsSectionDanger.vue';

const { t } = useI18n();
</script>
