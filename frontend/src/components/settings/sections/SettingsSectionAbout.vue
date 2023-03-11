<template>
  <SettingsSection>
    <div
      class="w-full flex flex-row justify-start items-center bg-dark px-3 py-3 my-3 rounded-lg border border-transparent mouse:hover:bc-primary mouse:hover:bg-darkest"
    >
      <a
        class="tc-main-light bg-light border-2 border-transparent px-2 rounded transition hover:bg-main"
        rel="noopener noreferrer nofollow"
        href="https://github.com/fabiankachlock/zwoo/discussions/categories/bug-reports"
      >
        {{ t('settings.reportBug') }}
      </a>
      <router-link class="ml-4 tc-main-light bg-light border-2 border-transparent px-2 rounded transition hover:bg-main" to="/contact">
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
import SettingsRow from '@/components/settings/common/SettingsRow.vue';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

import SettingsSection from '../common/SettingsSection.vue';
import VersionHistory from '../VersionHistory.vue';

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
