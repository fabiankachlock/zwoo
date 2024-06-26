<template>
  <div class="max-w-lg sm:w-full mx-auto">
    <div class="mx-4 sm:mx-0 pb-2 relative">
      <div class="w-full flex flex-row justify-between items-center sticky z-10 bg-bg top-0">
        <h2 class="text-text text-4xl mb-1 pt-3">{{ t('versionHistory.title') }}</h2>
      </div>
      <div class="relative flex flex-col flex-nowrap">
        <div v-if="versions">
          <div v-for="version in versions" :key="version" class="item my-2 rounded-lg border border-border bg-surface px-3 py-2">
            <div
              class="version-card-header flex flex-row flex-nowrap justify-between items-center cursor-pointer"
              @click="toggleVersionOpen(version)"
            >
              <div class="version-card-title">
                <p class="text-xl text-text">{{ t('versionHistory.versionTitle', [version]) }}</p>
              </div>
              <div class="version-card-actions -mr-1 flex flex-row flex-nowrap justify-between items-center overflow-hidden">
                <button
                  class="toggle text-2xl text-text relative p-4 rounded-md overflow-hidden bg-alt hover:bg-alt-hover border border-border"
                  @click.stop="toggleVersionOpen(version)"
                >
                  <Icon
                    icon="iconoir:nav-arrow-down"
                    class="icon absolute left-1/2 top-1/2 -translate-x-1/2 transition duration-300"
                    :class="{ 'opacity-0 translate-y-2': openVersions[version], '-translate-y-1/2': !openVersions[version] }"
                  />
                  <Icon
                    icon="iconoir:nav-arrow-up"
                    class="icon absolute left-1/2 top-1/2 -translate-x-1/2 transition duration-300"
                    :class="{ 'opacity-0 translate-y-2': !openVersions[version], '-translate-y-1/2': openVersions[version] }"
                  />
                </button>
              </div>
            </div>
            <div
              class="version-card-body transition duration-300"
              :class="{ open: openVersions[version], 'overflow-hidden': !openVersions[version] }"
            >
              <div class="content relative border-t border-border mt-2">
                <Changelog :changelog="versionChangelogs[version]" />
              </div>
            </div>
          </div>
        </div>
        <div v-else>
          <p class="text-text">
            {{ t('versionHistory.failure') }}
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

import Changelog from '@/components/misc/changelog/Changelog.vue';
import { Icon } from '@/components/misc/Icon';
import { useApi } from '@/core/adapter/helper/useApi';

const { t } = useI18n();
const { loadVersionHistory, loadChangelog } = useApi();
const versions = ref<string[] | undefined>([]);
const versionChangelogs = ref<Record<string, string>>({});
const openVersions = ref<Record<string, boolean>>({});

onMounted(async () => {
  const res = await loadVersionHistory();
  if (res.wasSuccessful) {
    versions.value = res.data.versions;
  }
});

const toggleVersionOpen = (version: string) => {
  openVersions.value[version] = !openVersions.value[version];
  if (openVersions.value[version] && !versionChangelogs.value[version]) {
    loadChangelogOfVersion(version);
  }
};

const loadChangelogOfVersion = async (version: string) => {
  if (versionChangelogs.value[version]) {
    return;
  }
  const changelogResponse = await loadChangelog(version);
  if (changelogResponse.wasSuccessful) {
    versionChangelogs.value[version] = changelogResponse.data;
  }
};
</script>

<style scoped>
.version-card-body {
  transition-property: max-height;
  max-height: 0;
}
.version-card-body.open {
  transition-property: max-height;
  max-height: 5000px;
}

.toggle:hover .icon {
  @apply scale-110;
}
</style>
