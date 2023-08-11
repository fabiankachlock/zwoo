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
    <SettingsRow :title="t('settings.feedbackRange')" :settings-key="ZwooConfigKey.FeedbackRange">
      <SelectFeedbackRange />
    </SettingsRow>
    <SettingsRow :title="t('settings.feedbackConsumer')" :settings-key="ZwooConfigKey.FeedbackDisplay">
      <SelectFeedbackDisplay />
    </SettingsRow>
    <SettingsRow
      :title="t('settings.sortCards')"
      :settings-key="ZwooConfigKey.SortCards"
      :status="t('settings.status.boolean.' + (sortCards ? 'on' : 'off'))"
    >
      <SortCardsSwitch />
    </SettingsRow>
  </SettingsSection>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';

import SelectFeedbackDisplay from '@/components/settings/sections/game/SelectFeedbackDisplay.vue';
import SelectFeedbackRange from '@/components/settings/sections/game/SelectFeedbackRange.vue';
import SelectTheme from '@/components/settings/sections/game/SelectTheme.vue';
import ShowCardDetailSwitch from '@/components/settings/sections/game/ShowCardDetailSwitch.vue';
import SortCardsSwitch from '@/components/settings/sections/game/SortCardsSwitch.vue';
import SettingsRow from '@/components/settings/SettingsRow.vue';
import SettingsSection from '@/components/settings/SettingsSection.vue';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

const { t } = useI18n();
const config = useConfig();
const sortCards = computed(() => config.get(ZwooConfigKey.SortCards));
const showCardDetail = computed(() => config.get(ZwooConfigKey.ShowCardsDetail));
</script>
