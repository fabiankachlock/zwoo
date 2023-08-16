<template>
  <SettingsSection>
    <SettingsRow :title="t('settings.deviceId')">
      <p class="tc-main text-right">{{ did }}</p>
    </SettingsRow>
    <SettingsRow :title="t('settings.logging')">
      <LoggingSettings />
    </SettingsRow>
    <SettingsRow :title="t('settings.localLogs')">
      <ClearLocalLogs />
    </SettingsRow>
  </SettingsSection>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';

import ClearLocalLogs from '@/components/settings/sections/developers/ClearLocalLogs.vue';
import LoggingSettings from '@/components/settings/sections/developers/LoggingSettings.vue';
import SettingsRow from '@/components/settings/SettingsRow.vue';
import SettingsSection from '@/components/settings/SettingsSection.vue';

const { t } = useI18n();
const did = window.DEVICE_ID;
</script>
