<template>
  <SettingsSection :title="t('settings.sections.developers')">
    <SettingsRow :title="t('settings.deviceId')">
      <p class="tc-main text-right">{{ did }}</p>
    </SettingsRow>
    <SettingsRow :title="t('settings.logging')">
      <LoggingSettings />
    </SettingsRow>
  </SettingsSection>
</template>

<script setup lang="ts">
import SettingsSection from '../SettingsSection.vue';
import SettingsRow from '../SettingsRow.vue';
import { useI18n } from 'vue-i18n';
import LoggingSettings from '../LoggingSettings.vue';

const { t } = useI18n();
const did = window.DEVICE_ID;
</script>
