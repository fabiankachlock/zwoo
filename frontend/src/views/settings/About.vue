<template>
  <SettingsSection>
    <div class="w-full flex flex-row justify-start items-center">
      <a
        class="text-warning-text bg-alt border-2 border-transparent px-2 rounded transition hover:bg-alt-hover"
        rel="noopener noreferrer nofollow"
        href="https://github.com/fabiankachlock/zwoo/discussions/categories/bug-reports"
      >
        {{ t('settings.reportBug') }}
      </a>
      <router-link class="mx-2 text-text bg-alt border-2 border-transparent px-2 rounded transition hover:bg-alt-hover" to="/contact">
        {{ t('settings.contact') }}
      </router-link>
      <Environment show="online">
        <VersionHistory />
      </Environment>
    </div>
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
