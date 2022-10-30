<template>
  <div class="max-w-lg sm:w-full mx-auto">
    <div class="mx-4 sm:mx-0 pb-2 relative">
      <div class="w-full flex flex-row justify-between items-center sticky z-10 bg-main top-0">
        <h2 class="tc-main text-4xl mb-1 pt-3">{{ t('versionHistory.title') }}</h2>
      </div>
      <div class="relative flex flex-col flex-nowrap">
        <div v-if="versions">
          <div v-for="version in versions" :key="version" class="item my-2 rounded-xl border bc-darkest bg-light px-3 py-2">
            <div class="version-card-header flex flex-row flex-nowrap justify-between items-center">
              <div class="version-card-title">
                <p class="text-xl tc-main">{{ t('versionHistory.versionTitle', [version]) }}</p>
              </div>
              <div class="version-card-actions -mr-1 flex flex-row flex-nowrap justify-between items-center overflow-hidden">
                <button
                  @click="toggleVersionOpen(version)"
                  class="toggle text-2xl tc-main relative p-4 rounded overflow-hidden bg-main hover:bg-dark"
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
              <div class="content p-2 pt-0 relative"># Changelog</div>
            </div>
          </div>
        </div>
        <div v-else>
          <p class="tc-main">
            {{ t('versionHistory.failure') }}
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
const versions = ref<string[] | undefined>(['1', '2', '3']);
const versionChangelogs = ref<Record<string, string>>({});
const openVersions = ref<Record<string, boolean>>({});

const toggleVersionOpen = (version: string) => {
  openVersions.value[version] = !openVersions.value[version];
  if (openVersions.value[version] && !versionChangelogs.value[version]) {
    // load version changelog
  }
};

onMounted(() => {
  // load versions
});
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
