<template>
  <template v-if="isLoggedIn">
    <SettingsSection>
      <SettingsRow v-if="isLoggedIn" :title="t('settings.logout')">
        <router-link class="text-text-light bg-bg border-2 border-transparent px-2 rounded transition hover:bg-bg" to="/logout">
          {{ t('settings.logout') }}
        </router-link>
      </SettingsRow>
      <SettingsRow v-if="isLoggedIn" :title="t('settings.settingsSync')">
        <SettingsSync />
      </SettingsRow>
      <SettingsRow v-if="isLoggedIn" :title="t('settings.userDefaultsSync')">
        <SyncUserDefaults />
      </SettingsRow>
      <SettingsRow v-if="isLoggedIn" :title="t('settings.changePassword')">
        <ChangePassword />
      </SettingsRow>
      <SettingsRow v-if="isLoggedIn" :title="t('settings.deleteAccount')">
        <DeleteAccount />
      </SettingsRow>
    </SettingsSection>
  </template>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';

import ChangePassword from '@/components/settings/sections/account/ChangePassword.vue';
import DeleteAccount from '@/components/settings/sections/account/DeleteAccount.vue';
import SettingsSync from '@/components/settings/sections/account/SettingsSync.vue';
import SyncUserDefaults from '@/components/settings/sections/account/SyncUserDefaults.vue';
import SettingsRow from '@/components/settings/SettingsRow.vue';
import SettingsSection from '@/components/settings/SettingsSection.vue';
import { useAuth } from '@/core/adapter/auth';

const { t } = useI18n();
const auth = useAuth();
const isLoggedIn = computed(() => auth.isLoggedIn);
</script>
