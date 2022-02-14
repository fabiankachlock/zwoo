<template>
  <SettingsSection :title="t('settings.sections.danger')">
    <SettingsRow title="">
      <DeleteAccount />
    </SettingsRow>
  </SettingsSection>
</template>

<script setup lang="ts">
import SettingsSection from '../SettingsSection.vue';
import SettingsRow from '../SettingsRow.vue';
import { useI18n } from 'vue-i18n';
import DeleteAccount from '../DeleteAccount.vue';

const { t } = useI18n();
</script>
