<template>
  <MaxWidthLayout size="normal">
    <h1 class="tc-main text-4xl my-5">{{ t('settings.title') }}</h1>
    <div class="my-5">
      <SettingsSectionGeneral />
      <SettingsSectionGame />
      <SettingsSectionAccount />
      <SettingsSectionDevelopers v-if="showDevSettings" />
      <Version @click="clickVersion()" />
      <UpdateDaemon />
    </div>
  </MaxWidthLayout>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';

import UpdateDaemon from '@/components/misc/UpdateDaemon.vue';
import Version from '@/components/misc/Version.vue';
import SettingsSectionAccount from '@/components/settings/sections/SettingsSectionAccount.vue';
import SettingsSectionDevelopers from '@/components/settings/sections/SettingsSectionDevelopers.vue';
import SettingsSectionGame from '@/components/settings/sections/SettingsSectionGame.vue';
import SettingsSectionGeneral from '@/components/settings/sections/SettingsSectionGeneral.vue';
import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';

const { t } = useI18n();
const showDevSettings = ref(localStorage.getItem('zwoo:dev-settings') === 'true');

let clicks = 0;
const clickVersion = () => {
  clicks++;
  if (clicks === 10) {
    localStorage.setItem('zwoo:dev-settings', 'true');
    showDevSettings.value = true;
  }
};
</script>
