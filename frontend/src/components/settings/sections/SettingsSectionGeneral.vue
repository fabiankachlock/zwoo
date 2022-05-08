<template>
  <SettingsSection :title="t('settings.sections.general')">
    <SettingsRow :title="t('settings.darkmode')" :status="t('settings.status.boolean.' + (darkModeOn ? 'on' : 'off'))">
      <DarkModeSwitch />
    </SettingsRow>
    <SettingsRow :title="t('settings.fullscreen')" :status="t('settings.status.boolean.' + (fullScreenOn ? 'on' : 'off'))">
      <FullScreenSwitch />
    </SettingsRow>
    <SettingsRow :title="t('settings.quickMenu')" :status="t('settings.status.boolean.' + (quickMenuOn ? 'on' : 'off'))">
      <QuickMenuSwitch />
    </SettingsRow>
    <SettingsRow :title="t('settings.language')">
      <LanguageSelection />
    </SettingsRow>
    <SettingsRow :title="t('settings.theme')">
      <SelectTheme />
    </SettingsRow>
    <SettingsRow :title="t('settings.cookies')">
      <ManageCookies />
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
import { useConfig } from '@/core/adapter/config';
import { computed } from 'vue';
import ManageCookies from '../ManageCookies.vue';
import SelectTheme from '../SelectTheme.vue';

const { t } = useI18n();
const config = useConfig();
const darkModeOn = computed(() => config.useDarkMode);
const fullScreenOn = computed(() => config.useFullScreen);
const quickMenuOn = computed(() => config.showQuickMenu);
</script>
