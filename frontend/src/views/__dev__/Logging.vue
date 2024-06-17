<template>
  <div class="fixed inset-0 bg-bg overflow-y-auto">
    <div class="w-full px-4 py-2">
      <h1 class="text-2xl text-text mb-4">{{ t('logging.title') }}</h1>
      <div class="flex justify-start items-center flex-wrap gap-6 mb-4">
        <div class="flex flex-col flex-nowrap justify-center items-center gap-2">
          <div class="flex flex-nowrap items-center justify-between w-full">
            <p class="text-text mr-2">{{ t('logging.console') }}</p>
            <RuleSwitch v-model="isConsoleLogging" />
          </div>
          <div class="flex flex-nowrap items-center justify-between w-full">
            <p class="text-text mr-2">{{ t('logging.store') }}</p>
            <RuleSwitch v-model="isStoreLogging" />
          </div>
          <div class="flex flex-nowrap items-center justify-between w-full">
            <p class="text-text mr-2">{{ t('logging.logrush') }}</p>
            <RuleSwitch v-model="isLogrushLogging" />
          </div>
        </div>
        <div class="flex flex-col flex-nowrap justify-center items-center gap-2">
          <div class="flex flex-nowrap justify-start items-center self-start">
            <p class="italic text-sm text-text-secondary">
              {{ t('logging.storedCount', storedLogs.length) }}
            </p>
            <button class="px-2 py-1 rounded bg-surface hover:bg-darkest text-error-dark-border dark:text-error-light-border mx-2" @click="clear()">
              {{ t('logging.clearStore') }}
            </button>
          </div>
          <div class="flex justify-end w-full">
            <button class="px-2 py-1 rounded bg-surface hover:bg-darkest text-text-secondary mx-2" @click="reload()">
              {{ t('logging.reloadLogs') }}
            </button>
          </div>
        </div>
      </div>
      <h2 class="text-xl text-text mb-4">{{ t('logging.storedLogs') }}</h2>
      <div class="relative border border-border-divider m-2 rounded overflow-scroll h-[60vh] whitespace-nowrap">
        <p v-for="log of storedLogs" :key="log.id" class="text-text-secondary font-mono text-sm" style="letter-spacing: -0.5px">
          <span class="mr-1 italic text-xs">
            {{
              new Date(log.date).toLocaleDateString('de', { day: '2-digit', month: '2-digit', hour: '2-digit', minute: '2-digit', second: '2-digit' })
            }}
          </span>
          <span>
            {{ log.log }}
          </span>
        </p>
      </div>
    </div>
    <div class="absolute top-2 right-2">
      <button @click="close()">
        <Icon icon="gg:close" class="text-text text-2xl" />
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';

import RuleSwitch from '@/components/lobby/rules/contentTypes/RuleSwitch.vue';
import { Icon } from '@/components/misc/Icon';
import { LogStore } from '@/core/services/logging/logImport';
import { LogEntry } from '@/core/services/logging/logTypes';

const { t } = useI18n();
const storedLogs = ref<LogEntry[]>([]);
const stored = localStorage.getItem('zwoo:logging') ?? '';
const isStoreLogging = ref(stored.includes('s'));
const isConsoleLogging = ref(stored.includes('c'));
const isLogrushLogging = ref(stored.includes('l'));

onMounted(async () => {
  await reload();
});

const reload = async () => {
  storedLogs.value = await LogStore.getAll();
};

const clear = () => {
  LogStore.reset();
  reload();
};

const close = () => {
  window.location.href = '/settings';
};

watch([isStoreLogging, isConsoleLogging, isLogrushLogging], ([useStore, useConsole, useLogrush]) => {
  localStorage.setItem('zwoo:logging', `${useStore ? 's' : ''}${useConsole ? 'c' : ''}${useLogrush ? 'l' : ''}`);
});
</script>
