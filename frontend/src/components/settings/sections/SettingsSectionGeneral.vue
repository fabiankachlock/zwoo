<template>
  <SettingsSection :title="t('settings.sections.general')">
    <SettingsRow
      :title="t('settings.darkmode')"
      :settings-key="ZwooConfigKey.UiMode"
      :status="t('settings.status.boolean.' + (darkModeOn ? 'on' : 'off'))"
    >
      <DarkModeSwitch />
    </SettingsRow>
    <SettingsRow :title="t('settings.fullscreen')" :status="t('settings.status.boolean.' + (fullScreenOn ? 'on' : 'off'))">
      <FullScreenSwitch />
    </SettingsRow>
    <SettingsRow
      :title="t('settings.quickMenu')"
      :settings-key="ZwooConfigKey.QuickMenu"
      :status="t('settings.status.boolean.' + (quickMenuOn ? 'on' : 'off'))"
    >
      <QuickMenuSwitch />
    </SettingsRow>
    <SettingsRow :title="t('settings.language')" :settings-key="ZwooConfigKey.Language">
      <LanguageSelection />
    </SettingsRow>
    <Environment show="online">
      <SettingsRow :title="t('settings.cookies')">
        <ManageCookies />
      </SettingsRow>
      <SettingsRow :title="t('settings.versionHistory')">
        <VersionHistory />
      </SettingsRow>
    </Environment>
  </SettingsSection>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';

import Environment from '@/components/misc/Environment.vue';
import SettingsRow from '@/components/settings/common/SettingsRow.vue';
import DarkModeSwitch from '@/components/settings/DarkModeSwitch.vue';
import FullScreenSwitch from '@/components/settings/FullScreenSwitch.vue';
import LanguageSelection from '@/components/settings/LanguageSelection.vue';
import QuickMenuSwitch from '@/components/settings/QuickMenuSwitch.vue';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

import SettingsSection from '../common/SettingsSection.vue';
import ManageCookies from '../ManageCookies.vue';
import VersionHistory from '../VersionHistory.vue';

const { t } = useI18n();
const config = useConfig();
const darkModeOn = computed(() => config.get(ZwooConfigKey.UiMode) === 'dark');
const fullScreenOn = computed(() => config.useFullScreen);
const quickMenuOn = computed(() => config.get(ZwooConfigKey.QuickMenu));
</script>
