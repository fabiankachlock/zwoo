<template>
  <SettingsSection>
    <SettingsRow :title="t('settings.cardTheme')" :settings-key="ZwooConfigKey.CardsTheme">
      <SelectTheme />
    </SettingsRow>
    <SettingsRow
      :title="t('settings.showCardDetail')"
      :settings-key="ZwooConfigKey.ShowCardsDetail"
      :status="t('settings.status.boolean.' + (showCardDetail ? 'on' : 'off'))"
    >
      <ShowCardDetailSwitch />
    </SettingsRow>
    <SettingsRow
      :title="t('settings.sortCards')"
      :settings-key="ZwooConfigKey.SortCards"
      :status="t('settings.status.boolean.' + (sortCards ? 'on' : 'off'))"
    >
      <SortCardsSwitch />
    </SettingsRow>
    <SettingsRow :title="t('settings.feedbackChat')" :settings-key="ZwooConfigKey.FeedbackChat">
      <SelectFeedbackRange :reason="FeedbackConsumerReason.Chat" />
    </SettingsRow>
    <SettingsRow :title="t('settings.feedbackSnackbar')" :settings-key="ZwooConfigKey.FeedbackSnackbar">
      <SelectFeedbackRange :reason="FeedbackConsumerReason.Snackbar" />
    </SettingsRow>
  </SettingsSection>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';

import SelectFeedbackRange from '@/components/settings/sections/game/SelectFeedbackRange.vue';
import SelectTheme from '@/components/settings/sections/game/SelectTheme.vue';
import ShowCardDetailSwitch from '@/components/settings/sections/game/ShowCardDetailSwitch.vue';
import SortCardsSwitch from '@/components/settings/sections/game/SortCardsSwitch.vue';
import SettingsRow from '@/components/settings/SettingsRow.vue';
import SettingsSection from '@/components/settings/SettingsSection.vue';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';
import { FeedbackConsumerReason } from '@/core/adapter/game/features/feedback/feedbackConfig';

const { t } = useI18n();
const config = useConfig();
const sortCards = computed(() => config.get(ZwooConfigKey.SortCards));
const showCardDetail = computed(() => config.get(ZwooConfigKey.ShowCardsDetail));
</script>
