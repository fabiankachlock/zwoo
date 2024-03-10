<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';

import { useLocalServer } from '@/core/adapter/tauri/localServer';

const { t } = useI18n();
const server = useLocalServer();
const config = computed(() => server.config);
</script>

<template>
  <div class="w-full flex flex-col bg-dark p-3 my-3 rounded-lg border border-transparent mouse:hover:bc-primary mouse:hover:bg-darkest">
    <div class="h-full tc-main-light mb-2">
      {{ t('settings.localServer.config') }}
    </div>
    <div v-if="config" class="flex flex-col">
      <div v-for="kv in Object.entries(server.config)" :key="kv[0]" class="flex justify-between items-center">
        <p class="tc-main-light">{{ t(`settings.localServer.configKey.${kv[0]}`) }}</p>
        <div v-if="typeof kv[1] === 'boolean'" class="tc-main text-xl">
          <p v-show="kv[1]">#ON#</p>
          <p v-show="!kv[1]">#OFF#</p>
        </div>
        <p v-else class="tc-main-light">{{ Array.isArray(kv[1]) ? kv[1].join(', ') : kv[1] }}</p>
      </div>
    </div>
    <div v-else>
      <p class="tc-main-secondary">{{ t('settings.localServer.noConfig') }}</p>
    </div>
  </div>
</template>
