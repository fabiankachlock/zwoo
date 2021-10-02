<template>
  <h1 class="tc-main text-4xl mx-6 my-5">{{ t('settings.title') }}</h1>
  <div class="mx-6 my-5">
    <div class="relative w-full p-2 flex flex-col justify-end items-stretch bg-lightest rounded-lg">
      <SettingsRow title="darkmode">
        <DarkModeSwitch />
      </SettingsRow>
      <SettingsRow title="fullscreen">
        <FullScreenSwitch />
      </SettingsRow>
      <SettingsRow title="quickMenu">
        <QuickMenuSwitch />
      </SettingsRow>
      <SettingsRow title="language">
        <LanguageSelection />
      </SettingsRow>
    </div>
  </div>
</template>

<script setup lang="ts">
import SettingsRow from '@/components/settings/SettingsRow.vue';
import DarkModeSwitch from '@/components/settings/DarkModeSwitch.vue';
import LanguageSelection from '@/components/settings/LanguageSelection.vue';
import FullScreenSwitch from '@/components/settings/FullScreenSwitch.vue';
import QuickMenuSwitch from '@/components/settings/QuickMenuSwitch.vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
</script>
