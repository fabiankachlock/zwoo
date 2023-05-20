<template>
  <template v-if="isLoggedIn">
    <SettingsSection>
      <SettingsRow v-if="isLoggedIn" :title="t('settings.logout')">
        <router-link class="tc-main-light bg-light border-2 border-transparent px-2 rounded transition hover:bg-main" to="/logout">
          {{ t('settings.logout') }}
        </router-link>
      </SettingsRow>
      <SettingsRow v-if="isLoggedIn" :title="t('settings.settingsSync')">
        <SettingsSync />
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

import { useAuth } from '@/core/adapter/auth';

import ChangePassword from '../ChangePassword.vue';
import SettingsRow from '../common/SettingsRow.vue';
import SettingsSection from '../common/SettingsSection.vue';
import DeleteAccount from '../DeleteAccount.vue';
import SettingsSync from '../SettingsSync.vue';

const { t } = useI18n();
const auth = useAuth();
const isLoggedIn = computed(() => auth.isLoggedIn);
</script>
