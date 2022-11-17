<template>
  <template v-if="isLoggedIn">
    <SettingsSection :title="t('settings.sections.account')">
      <SettingsRow :title="t('settings.changePassword')" v-if="isLoggedIn">
        <ChangePassword />
      </SettingsRow>
      <SettingsRow :title="t('settings.deleteAccount')" v-if="isLoggedIn">
        <DeleteAccount />
      </SettingsRow>
    </SettingsSection>
  </template>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';

import { useAuth } from '@/core/adapter/auth';

import ChangePassword from '../ChangePassword.vue';
import DeleteAccount from '../DeleteAccount.vue';
import SettingsRow from '../SettingsRow.vue';
import SettingsSection from '../SettingsSection.vue';

const { t } = useI18n();
const auth = useAuth();
const isLoggedIn = computed(() => auth.isLoggedIn);
</script>
