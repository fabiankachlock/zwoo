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
import SettingsSection from '../SettingsSection.vue';
import SettingsRow from '../SettingsRow.vue';
import { useI18n } from 'vue-i18n';
import DeleteAccount from '../DeleteAccount.vue';
import { useAuth } from '@/core/adapter/auth';
import { computed } from 'vue';

const { t } = useI18n();
const auth = useAuth();
const isLoggedIn = computed(() => auth.isLoggedIn);
const hasContent = computed(() => auth.isLoggedIn);
</script>
