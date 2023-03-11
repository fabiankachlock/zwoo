<template>
  <SettingsSection :title="t('settings.sections.game')">
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
  </SettingsSection>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';

import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

import SelectTheme from '../SelectTheme.vue';
import SettingsRow from '../SettingsRow.vue';
import SettingsSection from '../SettingsSection.vue';
import ShowCardDetailSwitch from '../ShowCardDetailSwitch.vue';
import SortCardsSwitch from '../SortCardsSwitch.vue';

const { t } = useI18n();
const config = useConfig();
const sortCards = computed(() => config.get(ZwooConfigKey.SortCards));
const showCardDetail = computed(() => config.get(ZwooConfigKey.ShowCardsDetail));
</script>
