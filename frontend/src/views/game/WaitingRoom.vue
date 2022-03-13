<template>
  <div class="w-full h-full relative flex flex-col flex-nowrap overflow-y-scroll py-2">
    <header class="mx-2 sticky top-0 left-0 right-0 z-10">
      <div class="w-full rounded-lg bg-darkest flex flex-row px-2 py-1 justify-center flex-wrap items-center">
        <p class="text-3xl tc-main font-bold m-2 text-center">#GameName#</p>
        <div class="space flex-1"></div>
        <div class="actions flex flex-row items-center justify-center m-2">
          <template v-if="isHost">
            <button class="tc-main-dark bg-primary hover:bg-primary-dark transition">{{ t('wait.start') }}</button>
            <button class="tc-main-dark bg-secondary hover:bg-secondary-dark transition">{{ t('wait.stop') }}</button>
          </template>
          <template v-if="!isHost">
            <button class="tc-main-dark bg-secondary hover:bg-secondary-dark transition">{{ t('wait.leave') }}</button>
          </template>
        </div>
      </div>
    </header>
    <main class="m-2 relative">
      <div class="main-content grid gap-2 grid-cols-1 md:grid-cols-2 mx-auto max-w-5xl">
        <div class="bg-lightest md:row-span-2">
          <p class="text-xl tc-main my-2">{{ t('wait.players') }}</p>
          <div class="w-full flex flex-col">
            <div
              v-for="player of players"
              :key="player.id"
              class="px-2 py-1 my-1 bg-dark border bc-darkest transition hover:bc-primary rounded-lg hover:bg-darkest"
            >
              <span class="text-lg tc-main-dark">
                {{ player.name }}
              </span>
            </div>
          </div>
        </div>
        <div class="bg-lightest">
          <p class="text-xl tc-main my-2">{{ t('wait.rules') }}</p>
          <div class="w-full flex flex-col">
            <div
              v-for="rule of rules"
              :key="rule"
              @click="selectRule(rule)"
              v-tooltip="t(openRule === rule ? 'wait.collapse' : 'wait.expand')"
              class="px-2 py-1 my-1 bg-dark border bc-darkest transition hover:bc-primary rounded-lg hover:bg-darkest cursor-pointer"
            >
              <p class="text-lg tc-main-dark">{{ t(`rules.${rule}.title`) }}</p>
              <div v-if="openRule === rule">
                <div class="divider w-full my-2 bc-invert-darkest border-b"></div>
                <div class="content">
                  <p class="text-sm italic tc-main-secondary">
                    {{ t(`rules.${rule}.info`) }}
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="bg-lightest">
          <p class="text-xl tc-main my-2">{{ t('wait.qrcode') }}</p>
          <p class="my-1 text-sm italic tc-main-secondary">
            {{ t('wait.qrcodeInfo') }}
          </p>
          <div class="qrcode-wrapper mx-auto max-w-xs">
            <QRCode :data="'https://zwoo-ui.web.app/join/' + gameId" :width="256" :height="256" />
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { useGameConfig } from '@/core/adapter/game';
import { computed, ref, watch } from 'vue';
import QRCode from '@/components/misc/QRCode.vue';
import { useI18n } from 'vue-i18n';
import { useGameEvents } from '@/core/adapter/play/events';
import { ZRPMessage, ZRPOPCode } from '@/core/services/zrp/zrpTypes';

const { t } = useI18n();
const gameEvents = useGameEvents();
const gameConfig = useGameConfig();
const isHost = computed(() => gameConfig.host);
const gameId = computed(() => gameConfig.gameId);
const players = ref([
  {
    name: 'player 1',
    id: '123'
  },
  {
    name: 'player 2',
    id: '234'
  }
]);
const rules = ['test-1', 'key-2'];

watch(
  () => gameEvents.lastEvent,
  zrpMessage => {
    if (zrpMessage && zrpMessage.code === ZRPOPCode.PlayerJoined) {
      const msg = zrpMessage as ZRPMessage<ZRPOPCode.PlayerJoined>;
      players.value.push({
        name: msg.data.name,
        id: msg.data.name
      });
    }
  }
);

const openRule = ref<string | undefined>(undefined);
const selectRule = (ruleKey: string) => {
  openRule.value = openRule.value === ruleKey ? undefined : ruleKey;
};
</script>

<style>
.actions button {
  @apply mx-1 px-2 py-1 rounded;
}

.main-content > div {
  @apply rounded-md p-3;
}

.qrcode-wrapper img {
  width: 100%;
}
</style>
