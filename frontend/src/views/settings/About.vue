<template>
  <SettingsSection>
    <div
      class="w-full flex flex-row justify-start items-center bg-bg-surface px-3 py-3 my-3 rounded-lg border border-transparent mouse:hover:border-primary mouse:hover:bg-darkest"
    >
      <a
        class="text-text-light bg-bg border-2 border-transparent px-2 rounded transition hover:bg-bg"
        rel="noopener noreferrer nofollow"
        href="https://github.com/fabiankachlock/zwoo/discussions/categories/bug-reports"
      >
        {{ t('settings.reportBug') }}
      </a>
      <router-link class="ml-4 text-text-light bg-bg border-2 border-transparent px-2 rounded transition hover:bg-bg" to="/contact">
        {{ t('settings.contact') }}
      </router-link>
    </div>
    <Environment show="online">
      <SettingsRow :title="t('settings.versionHistory')">
        <VersionHistory />
      </SettingsRow>
    </Environment>
  </SettingsSection>
  <Version @click="clickVersion()" />
  <UpdateDaemon></UpdateDaemon>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';

import Environment from '@/components/misc/Environment.vue';
import UpdateDaemon from '@/components/misc/UpdateDaemon.vue';
import Version from '@/components/misc/Version.vue';
import VersionHistory from '@/components/settings/sections/about/VersionHistory.vue';
import SettingsRow from '@/components/settings/SettingsRow.vue';
import SettingsSection from '@/components/settings/SettingsSection.vue';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

const { t } = useI18n();
const config = useConfig();

let clicks = 0;
const clickVersion = () => {
  clicks++;
  if (clicks === 10) {
    config.set(ZwooConfigKey.DevSettings, true);
  }
};
</script>
