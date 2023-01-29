<template>
  <Widget v-model="isOpen" title="wait.bots" widget-class="bg-light" button-class="bg-main hover:bg-dark">
    <template #actions>
      <div class="flex flex-row">
        <button @click="createBot()" class="share rounded m-1 bg-main hover:bg-dark tc-main-light">
          <div class="transform transition-transform hover:scale-110 p-1">
            <Icon icon="fluent:bot-add-20-regular" class="icon text-2xl"></Icon>
          </div>
        </button>
      </div>
    </template>
    <template #default>
      <div class="w-full flex flex-col">
        <div v-if="Object.keys(bots).length === 0">
          <p class="tc-main-dark italic">{{ t('wait.noBots') }}</p>
        </div>
        <div v-if="botDialogOpen">
          <FloatingDialog content-class="sm:max-w-lg">
            <div class="absolute top-2 right-2 z-10">
              <button @click="botDialogOpen = false" class="bg-lightest hover:bg-light p-1.5 tc-main-dark rounded">
                <Icon icon="akar-icons:cross" class="text-xl" />
              </button>
            </div>
            <Form>
              <FormTitle> {{ t('wait.editBot') }}</FormTitle>
              <TextInput
                v-if="!isBotUpdate"
                id="bot-name"
                v-model="botName"
                labelKey="wait.botName"
                :placeholder="t('wait.exampleBotName')"
              ></TextInput>
              <div v-else class="m-2 text-xl">
                <p class="tc-main-light">{{ botName }}</p>
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
          v-for="bot of bots"
          :key="bot.id"
          class="flex flex-nowrap justify-between items-center px-2 py-1 my-1 bg-dark border bc-darkest transition mouse:hover:bc-primary rounded-lg mouse:hover:bg-darkest"
        >
          <div class="flex justify-start items-center">
            <p class="text-lg tc-main-dark">
              <span>
                {{ bot.name }}
              </span>
            </p>
          </div>
          <div class="flex items-center h-full justify-end">
            <!--  TODO: add back when there is something to edit
              <button @click="updateBot(bot.id)" v-tooltip="t('wait.edit')" class="tc-primary h-full bg-light hover:bg-main rounded p-1 mr-2">
                <Icon icon="carbon:settings" />
              </button>
            -->
            <button v-tooltip="t('wait.kick')" @click="deleteBot(bot.id)" class="tc-secondary h-full bg-light hover:bg-main rounded p-1">
              <Icon icon="iconoir:delete-circled-outline" />
            </button>
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
import { useBotManager } from '@/core/adapter/play/botManager';

import Widget from '../Widget.vue';

const { t } = useI18n();
const isOpen = useUserDefaults('lobby:widgetBotsOpen', true);
const botManager = useBotManager();
const bots = computed(() => botManager.botConfigs);
const botDialogOpen = ref(false);
const isBotUpdate = ref(false);
const botName = ref('');
const botId = ref('');

const createBot = () => {
  botName.value = '';
  botId.value = '';
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

const deleteBot = (id: string) => {
  botManager.deleteBot(id);
};

const submitBot = () => {
  if (isBotUpdate.value) {
    botManager.updateBot(botId.value, {
      type: 1
    });
  } else {
    botManager.addBot(botName.value, {
      type: 1
    });
  }
  botDialogOpen.value = false;
  botName.value = '';
  botId.value = '';
};
</script>
