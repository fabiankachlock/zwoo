<template>
  <template v-if="hasContent">
    <SettingsSection :title="t('settings.sections.danger')">
      <SettingsRow v-if="isLoggedIn" title="">
        <DeleteAccount />
      </SettingsRow>
    </SettingsSection>
  </template>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';

import { useAuth } from '@/core/adapter/auth';

import DeleteAccount from '../DeleteAccount.vue';
import SettingsRow from '../SettingsRow.vue';
import SettingsSection from '../SettingsSection.vue';

const { t } = useI18n();
const auth = useAuth();
const isLoggedIn = computed(() => auth.isLoggedIn);
const hasContent = computed(() => auth.isLoggedIn);
</script>
