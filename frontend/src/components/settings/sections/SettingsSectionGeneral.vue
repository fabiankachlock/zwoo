<template>
  <SettingsSection :title="t('settings.sections.general')">
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
  </SettingsSection>
</template>

<script setup lang="ts">
import SettingsSection from '../SettingsSection.vue';
import SettingsRow from '@/components/settings/SettingsRow.vue';
import DarkModeSwitch from '@/components/settings/DarkModeSwitch.vue';
import LanguageSelection from '@/components/settings/LanguageSelection.vue';
import FullScreenSwitch from '@/components/settings/FullScreenSwitch.vue';
import QuickMenuSwitch from '@/components/settings/QuickMenuSwitch.vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
</script>
