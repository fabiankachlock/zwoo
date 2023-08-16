<template>
  <SettingsSection>
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
    </Environment>
  </SettingsSection>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';

import Environment from '@/components/misc/Environment.vue';
import DarkModeSwitch from '@/components/settings/sections/general/DarkModeSwitch.vue';
import FullScreenSwitch from '@/components/settings/sections/general/FullScreenSwitch.vue';
import LanguageSelection from '@/components/settings/sections/general/LanguageSelection.vue';
import ManageCookies from '@/components/settings/sections/general/ManageCookies.vue';
import QuickMenuSwitch from '@/components/settings/sections/general/QuickMenuSwitch.vue';
import SettingsRow from '@/components/settings/SettingsRow.vue';
import SettingsSection from '@/components/settings/SettingsSection.vue';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

const { t } = useI18n();
const config = useConfig();
const darkModeOn = computed(() => config.get(ZwooConfigKey.UiMode) === 'dark');
const fullScreenOn = computed(() => config.useFullScreen);
const quickMenuOn = computed(() => config.get(ZwooConfigKey.QuickMenu));
</script>
