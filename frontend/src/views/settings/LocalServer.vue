<template>
  <SettingsSection>
    <SettingsRow :title="t('settings.sections.server.controls')">
      <LocalServerControls />
    </SettingsRow>
    <LocalServerConfig />
    <LocalServerStats />
  </SettingsSection>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

import LocalServerConfig from '@/components/settings/sections/local-server/LocalServerConfig.vue';
import LocalServerControls from '@/components/settings/sections/local-server/LocalServerControls.vue';
import LocalServerStats from '@/components/settings/sections/local-server/LocalServerStats.vue';
import SettingsRow from '@/components/settings/SettingsRow.vue';

const { t } = useI18n();
const serverIsRunning = ref(false);

onMounted(async () => {
  const status = await loadServerStatus();
  serverIsRunning.value = status != undefined;
});

const loadServerStatus = (): void => {};
</script>
