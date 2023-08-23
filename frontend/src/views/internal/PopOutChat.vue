<template>
  <NonScrollableLayout>
    <div v-if="!isActive" class="mx-auto px-2 max-w-xl">
      <p class="text-xl tc-main text-center m-2">{{ t('popoutChat.noActiveGame') }}</p>
    </div>
    <div v-else>
      <div class="pop-out-grid">
        <div class="bg-darkest m-2 mb-0 rounded-lg p-1 flex flex-row justify-between items-center">
          <p class="ml-2 tc-main text-xl" style="text-overflow: ellipsis; overflow: hidden">{{ gameName }}</p>
        </div>
        <div v-if="messages.length > 0" class="mx-auto px-2 max-w-xl overflow-y-auto w-full">
          <div :ref="r => container = (r as HTMLDivElement)" class="h-full py-2 px-3 flex flex-col flex-nowrap items-center overflow-y-auto">
            <ChatMessage
              v-for="messageItem in messages"
              :key="messageItem.id"
              :message="messageItem.message"
              :is-own="messageItem.sender.id === lobbyId"
              :is-spectator="messageItem.sender.role === ZRPRole.Spectator"
              :is-host="messageItem.sender.role === ZRPRole.Host"
              :is-system="messageItem.sender.role === ZRPRole._System"
              :name="messageItem.sender.name"
            />
          </div>
        </div>
        <div v-else class="mx-auto px-2 max-w-xl">
          <p class="text-xl tc-main text-center m-2">{{ t('popoutChat.noMessages') }}</p>
        </div>
        <div>
          <div class="m-2 flex flex-row flex-nowrap justify-between items-stretch">
            <div class="w-full mr-2">
              <input
                v-model="message"
                class="appearance-none outline-none w-full bg-darkest tc-main-light px-2 py-0.5 rounded transition focus:bg-darkest border border-transparent focus:bc-primary ring-0"
                type="text"
                @keyup.stop
                @keyup.enter="sendMessage"
              />
            </div>
            <div
              class="sendMessageButton flex justify-center items-center bg-darkest px-3 py-0.5 rounded transition cursor-pointer"
              @click="sendMessage"
            >
              <button class="tc-primary">
                <Icon icon="teenyicons:send-outline" class="trasform" />
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </NonScrollableLayout>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';

import ChatMessage from '@/components/game/chat/ChatMessage.vue';
import { Icon } from '@/components/misc/Icon';
import { useChatBroadcast } from '@/core/adapter/game/features/chatBroadcast';
import { ZRPRole } from '@/core/domain/zrp/zrpTypes';
import NonScrollableLayout from '@/layouts/NonScrollableLayout.vue';

const chat = useChatBroadcast();
const { t } = useI18n();
const message = ref('');
const messages = computed(() => chat.allMessages);
const isActive = computed(() => chat.isActive);
const lobbyId = computed(() => chat.ownId);
const gameName = computed(() => chat.gameName);
const container = ref<HTMLDivElement | undefined>(undefined);

watch(messages, () => {
  setTimeout(() => {
    // 'wait for' DOM update before scroll to bottom
    scrollToBottom();
  }, 0);
});

onMounted(() => {
  chat.requireSetup();
});

const scrollToBottom = () => {
  if (container.value) {
    container.value.scrollTo({
      top: container.value.scrollHeight,
      behavior: 'smooth'
    });
  }
};

const sendMessage = () => {
  const chatMessage = (message.value || '').trim();
  if (chatMessage.length === 0) return;
  chat.sendMessage(message.value);
  message.value = '';
};
</script>

<style scoped>
.pop-out-grid {
  @apply grid h-full w-full max-w-full fixed;
  grid-template-rows: min-content auto min-content;
}

.sendMessageButton:hover svg {
  @apply scale-110;
}
</style>
