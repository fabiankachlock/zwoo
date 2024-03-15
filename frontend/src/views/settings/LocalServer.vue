<template>
  <SettingsSection>
    <SettingsRow :title="t('settings.localServer.controls')">
      <LocalServerControls />
    </SettingsRow>
    <SettingsRow title="" v-if="serverIsRunning">
      <LocalServerLogin />
    </SettingsRow>
    <LocalServerConfig />
    <LocalServerStats />
  </SettingsSection>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';

import LocalServerConfig from '@/components/settings/sections/local-server/LocalServerConfig.vue';
import LocalServerControls from '@/components/settings/sections/local-server/LocalServerControls.vue';
import LocalServerLogin from '@/components/settings/sections/local-server/LocalServerLogin.vue';
import LocalServerStats from '@/components/settings/sections/local-server/LocalServerStats.vue';
import SettingsRow from '@/components/settings/SettingsRow.vue';
import SettingsSection from '@/components/settings/SettingsSection.vue';
import { useLocalServer } from '@/core/adapter/tauri/localServer';

const { t } = useI18n();
const localServer = useLocalServer();
const serverIsRunning = computed(() => localServer.isRunning);

onMounted(() => {
  localServer.loadStatus();
});
</script>
