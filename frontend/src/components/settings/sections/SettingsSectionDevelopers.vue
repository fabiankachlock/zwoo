<template>
  <SettingsSection>
    <SettingsRow :title="t('settings.deviceId')">
      <p class="tc-main text-right">{{ did }}</p>
    </SettingsRow>
    <SettingsRow :title="t('settings.logging')">
      <LoggingSettings />
    </SettingsRow>
  </SettingsSection>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';

import SettingsRow from '../common/SettingsRow.vue';
import SettingsSection from '../common/SettingsSection.vue';
import LoggingSettings from '../LoggingSettings.vue';

const { t } = useI18n();
const did = window.DEVICE_ID;
</script>
