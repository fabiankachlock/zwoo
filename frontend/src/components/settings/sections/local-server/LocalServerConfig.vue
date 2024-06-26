<script setup lang="ts">
import { computed, ref } from 'vue';
import { useI18n } from 'vue-i18n';

import LocalServerConfigEditor from '@/components/settings/sections/local-server/LocalServerConfigEditor.vue';
import { useLocalServer } from '@/core/adapter/tauri/localServer';
import { LocalServerConfig } from '@/core/domain/localServer/localServerConfig';

const { t } = useI18n();
const server = useLocalServer();
const config = computed(() => server.config);
const editOpen = ref(false);

const editDialogClosed = (newConfig: LocalServerConfig) => {
  server.saveConfig(newConfig);
  editOpen.value = false;
};
</script>

<template>
  <div class="w-full flex flex-col py-3 rounded-lg">
    <LocalServerConfigEditor v-if="editOpen" :config="config" @close="editDialogClosed" />
    <div class="flex flex-row justify-between items-center mb-2">
      <p class="text-text">
        {{ t('localServer.config') }}
      </p>
      <button class="bg-alt border border-border px-2 rounded transition hover:bg-alt-hover select-none text-text" @click="editOpen = true">
        {{ t('localServer.editConfig') }}
      </button>
    </div>
    <div v-if="config" class="flex flex-col">
      <div v-for="kv in Object.entries(server.config)" :key="kv[0]" class="flex justify-between items-center">
        <p class="text-text">{{ t(`localServer.configKey.${kv[0]}`) }}</p>
        <div v-if="typeof kv[1] === 'boolean'" class="text-text">
          <p v-show="kv[1]">{{ t('controls.toggle.on') }}</p>
          <p v-show="!kv[1]">{{ t('controls.toggle.off') }}</p>
        </div>
        <p v-else-if="kv[0] === 'secretKey'" class="text-text">*********</p>
        <p v-else class="text-text">{{ Array.isArray(kv[1]) ? kv[1].join(', ') : kv[1] ? kv[1] : '""' }}</p>
      </div>
    </div>
    <div v-else>
      <p class="text-text-secondary">{{ t('localServer.noConfig') }}</p>
    </div>
  </div>
</template>
