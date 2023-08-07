<template>
  <div>
    <select
      :ref="
        r => {
          selection = r as HTMLSelectElement;
        }
      "
      :value="selectedConsumer"
      class="bg-light p-1 rounded tc-main-dark"
    >
      <option v-for="opt in availableConsumer" :key="opt" :value="opt">{{ t('feedbackConsumer.option.' + opt) }}</option>
    </select>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

import { useConfig, ZwooConfigKey } from '@/core/adapter/config';
import { FeedbackConsumerReason, getAllowedConsumers, stringifyFeedbackConsumer } from '@/core/adapter/game/features/feedback/feedbackConfig';

const { t } = useI18n();
const config = useConfig();
const selection = ref<HTMLSelectElement>();
const availableConsumer = Object.values(FeedbackConsumerReason);
const selectedConsumer = computed(() => {
  const raw = config.get(ZwooConfigKey.FeedbackDisplay);
  return getAllowedConsumers(raw)[0];
});

// todo implement multiple selects
onMounted(() => {
  selection.value?.addEventListener('change', () => {
    const consumer = selection.value?.value;
    if (!consumer) {
      return;
    }
    config.set(ZwooConfigKey.FeedbackDisplay, stringifyFeedbackConsumer([consumer as FeedbackConsumerReason]));
  });
});
</script>
