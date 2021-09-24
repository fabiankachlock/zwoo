<template>
  <h1 class="tc-main text-4xl mx-6 my-5">Settings</h1>
  <div class="mx-6 my-5">
    <div class="relative w-full p-2 flex flex-col justify-end items-stretch bg-lightest rounded-lg">
      <SettingsRow title="darkmode">
        <DarkModeSwitch />
      </SettingsRow>
      <SettingsRow title="language">
        <LanguageSelection />
      </SettingsRow>
    </div>
  </div>
</template>

<script setup lang="ts">
import SettingsRow from '@/components/settings/SettingsRow.vue';
import DarkModeSwitch from '@/components/settings/DarkModeSwitch.vue';
import LanguageSelection from '@/components/settings/LanguageSelection.vue';
</script>
