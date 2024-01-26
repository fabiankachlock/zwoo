<template>
  <SettingsSection>
    <div>
      <button v-if="!serverIsRunning" @click="startServer">
        {{ t('localServer.start') }}
      </button>
      <button v-else @click="stopServer">
        {{ t('localServer.stop') }}
      </button>
    </div>
    <div>
      <p>...Server Stats...</p>
    </div>
  </SettingsSection>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
const serverIsRunning = ref(false);

onMounted(async () => {
  const status = await loadServerStatus();
  serverIsRunning.value = status != undefined;
});

const startServer = () => {};
const stopServer = () => {};

const loadServerStatus = (): Promise<unknown> => {};
</script>
