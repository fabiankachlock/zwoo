<template>
  <Widget v-model="isOpen" title="wait.bots">
    <template #actions>
      <div class="flex flex-row">
        <template v-if="isHost">
          <button class="share rounded m-1 bg-alt hover:bg-alt-hover border border-border text-text" @click="createBot()">
            <div class="transform transition-transform hover:scale-110 p-1">
              <Icon icon="fluent:bot-add-20-regular" class="icon text-2xl"></Icon>
            </div>
          </button>
        </template>
      </div>
    </template>
    <template #default>
      <div class="w-full flex flex-col">
        <div v-if="Object.keys(isHost ? realBots : fakeBots).length === 0">
          <p class="text-text italic">{{ t('wait.noBots') }}</p>
        </div>
        <div v-if="botDialogOpen && isHost">
          <FloatingDialog content-class="sm:max-w-lg">
            <div class="absolute top-2 right-2 z-10">
              <button class="bg-bg hover:bg-bg p-1.5 text-text rounded" @click="botDialogOpen = false">
                <Icon icon="akar-icons:cross" class="text-xl" />
              </button>
            </div>
            <Form>
              <FormTitle> {{ t('wait.editBot') }}</FormTitle>
              <TextInput
                v-if="!isBotUpdate"
                id="bot-name"
                v-model="botName"
                label-key="wait.botName"
                :placeholder="t('wait.exampleBotName')"
              ></TextInput>
              <div v-else class="m-2 text-xl">
                <p class="text-text">{{ botName }}</p>
              </div>

              <div class="w-full mb-4">
                <div class="m-2">
                  <label class="block text-text-secondary text-sm font-bold mb-2" for="bot-type">
                    {{ t('wait.botType') }}
                  </label>
                  <div class="w-full grid grid-cols-3">
                    <button
                      class="shadow appearance-none bg-alt hover:bg-alt-hover border border-border rounded-l-lg w-full p-2 text-text focus:outline-none focus:border-primary"
                      :class="{ '!bg-surface text-primary-text': botType === -1 }"
                      @click="botType = -1"
                    >
                      {{ t('wait.dumpBot') }}
                    </button>
                    <button
                      class="shadow appearance-none bg-alt hover:bg-alt-hover border border-border w-full p-2 text-text focus:outline-none focus:border-primary"
                      :class="{ '!bg-surface text-primary-text': botType === 0 }"
                      @click="botType = 0"
                    >
                      {{ t('wait.normalBot') }}
                    </button>
                    <button
                      class="shadow appearance-none bg-alt hover:bg-alt-hover border border-border rounded-r-lg w-full p-2 text-text focus:outline-none focus:border-primary"
                      :class="{ '!bg-surface text-primary-text': botType === 1 }"
                      @click="botType = 1"
                    >
                      {{ t('wait.smartBot') }}
                    </button>
                  </div>
                </div>
              </div>

              <FormActions>
                <FormSubmit @click="submitBot()">
                  {{ t(isBotUpdate ? 'wait.updateBot' : 'wait.createBot') }}
                </FormSubmit>
              </FormActions>
            </Form>
          </FloatingDialog>
        </div>
        <div
          v-for="bot of isHost ? realBots : fakeBots"
          :key="bot.id"
          class="flex flex-nowrap justify-between items-center px-2 py-1 my-1 bg-bg border border-border transition mouse:hover:border-primary rounded-lg"
        >
          <div class="flex justify-start items-center">
            <p class="text-lg text-text">
              <span>
                {{ bot.name }}
              </span>
            </p>
          </div>
          <div class="flex items-center h-full justify-end">
            <!--  TODO: add back when there is something to edit
              <button @click="updateBot(bot.id)" v-tooltip="t('wait.edit')" class="text-primary-text h-full bg-bg hover:bg-bg rounded p-1 mr-2">
                <Icon icon="carbon:settings" />
              </button>
            -->
            <template v-if="isHost">
              <button
                v-tooltip="t('wait.kick')"
                class="text-warning-text h-full bg-alt hover:bg-alt-hover border border-border rounded p-1"
                @click="deleteBot(bot.id)"
              >
                <Icon icon="iconoir:delete-circled-outline" />
              </button>
            </template>
          </div>
        </div>
      </div>
    </template>
  </Widget>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { useI18n } from 'vue-i18n';

import { FormActions, FormSubmit, FormTitle, TextInput } from '@/components/forms';
import Form from '@/components/forms/Form.vue';
import FloatingDialog from '@/components/misc/FloatingDialog.vue';
import { Icon } from '@/components/misc/Icon';
import { useUserDefaults } from '@/composables/userDefaults';
import { BotType, useBotManager } from '@/core/adapter/game/botManager';
import { useLobbyStore } from '@/core/adapter/game/lobby';
import { useIsHost } from '@/core/adapter/game/util/userRoles';

import Widget from '../Widget.vue';

const { t } = useI18n();
const { isHost } = useIsHost();
const isOpen = useUserDefaults('lobby:widgetBotsOpen', true);
const botManager = useBotManager();
const lobby = useLobbyStore();
const realBots = computed(() => botManager.botConfigs);
const fakeBots = computed(() =>
  lobby.bots.reduce((sum, bot) => ({ ...sum, [bot.id]: { id: bot.id, name: bot.username, config: { type: 0 } } }), {} as Record<string, BotType>)
);
const botDialogOpen = ref(false);
const isBotUpdate = ref(false);
const botName = ref('');
const botType = ref(0);
const botId = ref<number | undefined>(undefined);

const createBot = () => {
  botName.value = '';
  botId.value = undefined;
  botDialogOpen.value = true;
  isBotUpdate.value = false;
};

// const updateBot = (id: string) => {
//   botName.value = botManager.botConfigs[id].name;
//   botId.value = id;
//   // const config = botManager.botConfigs[name];
//   // set config
//   botDialogOpen.value = true;
//   isBotUpdate.value = true;
// };

const deleteBot = (id: number) => {
  botManager.deleteBot(id);
};

const submitBot = () => {
  if (isBotUpdate.value && botId.value) {
    botManager.updateBot(botId.value, {
      type: botType.value
    });
  } else {
    botManager.addBot(botName.value, {
      type: botType.value
    });
  }
  botDialogOpen.value = false;
  botName.value = '';
  botId.value = undefined;
};
</script>
