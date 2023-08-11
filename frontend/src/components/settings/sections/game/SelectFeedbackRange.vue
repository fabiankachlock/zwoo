<template>
  <div>
    <select
      :ref="
        r => {
          selection = r as HTMLSelectElement;
        }
      "
      :value="selectedRange"
      class="bg-light p-1 rounded tc-main-dark"
    >
      <option v-for="opt in availableRanges" :key="opt" :value="opt">{{ t('feedback.range.option.' + opt) }}</option>
    </select>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, toRefs } from 'vue';
import { useI18n } from 'vue-i18n';

import { useConfig } from '@/core/adapter/config';
import {
  FeedbackConsumerReason,
  FeedbackConsumingRange,
  getConfigKeyForFeedbackReason,
  getFeedbackRange,
  stringifyFeedbackRange
} from '@/core/adapter/game/features/feedback/feedbackConfig';

const props = defineProps<{
  reason: FeedbackConsumerReason;
}>();

const { reason } = toRefs(props);

const { t } = useI18n();
const config = useConfig();
const selection = ref<HTMLSelectElement>();
const availableRanges = Object.values(FeedbackConsumingRange);
const configKey = computed(() => getConfigKeyForFeedbackReason(reason.value));
const selectedRange = computed(() => {
  const raw = config.get(configKey.value);
  return getFeedbackRange(raw as string);
});

onMounted(() => {
  selection.value?.addEventListener('change', () => {
    const range = selection.value?.value;
    if (!range) {
      return;
    }
    config.set(configKey.value, stringifyFeedbackRange(range as FeedbackConsumingRange));
  });
});
</script>
