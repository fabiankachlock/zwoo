<template>
  <div class="max-w-lg sm:w-full mx-auto">
    <div class="mx-4 sm:mx-0 pb-2 relative">
      <div class="w-full flex flex-row justify-between items-center sticky z-10 bg-main top-0">
        <h2 class="tc-main text-4xl mb-1 pt-3">{{ t('versionHistory.title') }}</h2>
      </div>
      <div class="relative flex flex-col flex-nowrap">
        <div v-if="versions">
          <div
            v-for="version in versions"
            :key="version"
            class="item my-2 rounded-xl border bc-darkest mouse:hover:bg-darkest mouse:hover:bc-primary bg-dark px-3 py-2"
          >
            <div class="flex flex-row justify-between flex-wrap items-center">
              <div class="text tc-main-light flex flex-row flex-nowrap justify-start items-center">
                <p class="mr-2">{{ t('versionHistory.versionTitle', [version]) }}</p>
              </div>
              <div class="flex flex-1 flex-row flex-nowrap justify-end items-stretch"># Button</div>
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
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
const versions = ref<string[] | undefined>(undefined);
</script>
