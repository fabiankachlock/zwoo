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
  server.config = newConfig;
};
</script>

<template>
  <div class="w-full flex flex-col bg-dark p-3 my-3 rounded-lg border border-transparent mouse:hover:bc-primary mouse:hover:bg-darkest">
    <LocalServerConfigEditor v-if="editOpen" :config="config" @close="editDialogClosed" />
    <div class="flex flex-row justify-between items-center mb-2">
      <p class="tc-main-light">
        {{ t('localServer.config') }}
      </p>
      <button @click="editOpen = true">{{ t('localServer.editConfig') }}</button>
    </div>
    <div v-if="config" class="flex flex-col">
      <div v-for="kv in Object.entries(server.config)" :key="kv[0]" class="flex justify-between items-center">
        <p class="tc-main-light">{{ t(`localServer.configKey.${kv[0]}`) }}</p>
        <div v-if="typeof kv[1] === 'boolean'" class="tc-main text-xl">
          <p v-show="kv[1]">#ON#</p>
          <p v-show="!kv[1]">#OFF#</p>
        </div>
        <p v-else class="tc-main-light">{{ Array.isArray(kv[1]) ? kv[1].join(', ') : kv[1] }}</p>
      </div>
    </div>
    <div v-else>
      <p class="tc-main-secondary">{{ t('localServer.noConfig') }}</p>
    </div>
  </div>
</template>
