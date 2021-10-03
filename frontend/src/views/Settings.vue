<template>
  <h1 class="tc-main text-4xl mx-6 my-5">{{ t('settings.title') }}</h1>
  <div class="mx-6 my-5">
    <SettingsSectionGeneral />
  </div>
</template>

<script setup lang="ts">
import SettingsSectionGeneral from '@/components/settings/sections/SettingsSectionGeneral.vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
</script>
