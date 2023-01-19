<template>
  <Widget v-model="isOpen" title="wait.bots" widget-class="bg-light" button-class="bg-main hover:bg-dark">
    <template #actions>
      <div class="flex flex-row">
        <button class="share rounded m-1 bg-main hover:bg-dark tc-main-light">
          <div class="transform transition-transform hover:scale-110 p-1">
            <Icon icon="iconoir:share-android" class="icon text-2xl"></Icon>
          </div>
        </button>
      </div>
    </template>
    <template #default>
      <div class="w-full flex flex-col">
        <div v-if="bots.length === 0">
          <p class="tc-main-dark italic">{{ t('wait.notBots') }}</p>
        </div>
        <div
          v-for="bot of bots"
          :key="bot.id"
          class="flex flex-nowrap justify-between items-center px-2 py-1 my-1 bg-dark border bc-darkest transition mouse:hover:bc-primary rounded-lg mouse:hover:bg-darkest"
        >
          <div class="flex justify-start items-center">
            <p class="text-lg tc-main-dark">
              <span :class="{ 'tc-primary': username === bot.username }">
                {{ bot.username }}
              </span>
            </p>
          </div>
          <div class="flex items-center h-full justify-end">
            <button v-tooltip="t('wait.edit')" class="tc-primary h-full bg-light hover:bg-main rounded p-1">
              <Icon icon="iconoir:eye-alt" />
            </button>
          </div>
        </div>
      </div>
    </template>
  </Widget>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';

import { Icon } from '@/components/misc/Icon';
import { useUserDefaults } from '@/composables/userDefaults';
import { useAuth } from '@/core/adapter/auth';
import { useLobbyStore } from '@/core/adapter/play/lobby';

import Widget from '../Widget.vue';

const { t } = useI18n();
const isOpen = useUserDefaults('lobby:widgetPlayersOpen', true);
const lobby = useLobbyStore();
const auth = useAuth();
const username = computed(() => auth.username);
const bots = computed(() => lobby.players);
</script>
