<template>
  <SettingsSection :title="t('settings.sections.game')">
    <SettingsRow :title="t('settings.showCardDetail')" :status="t('settings.status.boolean.' + (showCardDetail ? 'on' : 'off'))">
      <ShowCardDetailSwitch />
    </SettingsRow>
    <SettingsRow :title="t('settings.sortCards')" :status="t('settings.status.boolean.' + (sortCards ? 'on' : 'off'))">
      <SortCardsSwitch />
    </SettingsRow>
  </SettingsSection>
</template>

<script setup lang="ts">
import SettingsSection from '../SettingsSection.vue';
import SettingsRow from '../SettingsRow.vue';
import ShowCardDetailSwitch from '../ShowCardDetailSwitch.vue';
import { useI18n } from 'vue-i18n';
import { useConfig } from '@/core/adapter/config';
import { computed } from 'vue';
import SortCardsSwitch from '../SortCardsSwitch.vue';

const { t } = useI18n();
const config = useConfig();
const sortCards = computed(() => config.sortCards);
const showCardDetail = computed(() => config.showCardDetail);
</script>

