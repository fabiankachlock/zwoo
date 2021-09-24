<template>
  <h1 class="tc-main text-4xl mx-6 my-5">Settings</h1>
  <div class="mx-6 my-5">
    <p v-t="'test'"></p>
    <p>{{ t('test2') }}</p>
    <div class="relative w-full p-2 flex flex-col justify-end items-stretch bg-lightest rounded-lg">
      <SettingsRow title="DarkMode">
        <DarkModeSwitch />
      </SettingsRow>
    </div>
  </div>
</template>

<script setup lang="ts">
import SettingsRow from '@/components/settings/SettingsRow.vue';
import DarkModeSwitch from '@/components/settings/DarkModeSwitch.vue';
import { useI18n } from 'vue-i18n';

const { t, locale } = useI18n();
locale.value = 'de';
</script>

<i18n global lang="json">
{
  "de": {
    "test": "de"
  },
  "en": {
    "test": "en"
  }
}
</i18n>
