<template>
  <FloatingDialog v-show="changelog" :without-padding="true">
    <div class="sticky top-0 z-10 px-5 pt-5 bg-bg">
      <h2 class="text-text text-xl text-center py-2 border-border border-b">{{ t('changelog.title', [version]) }}</h2>
    </div>
    <Changelog :changelog="changelog" />
    <div class="sticky -bottom-[0.5px] bg-bg px-5 pb-3">
      <div class="flex justify-end items-center border-border border-t">
        <button
          class="mt-4 flex justify-center items-center bg-bg border-2 border-primary px-4 py-1 rounded transition hover:bg-bg-surface cursor-pointer select-none"
          @click="close"
        >
          <p class="text-primary-text text-center">{{ t('changelog.close') }}</p>
        </button>
      </div>
    </div>
  </FloatingDialog>
</template>

<script setup lang="ts">
import { onMounted, ref, toRefs, watch } from 'vue';
import { useI18n } from 'vue-i18n';

import { useApi } from '@/core/adapter/helper/useApi';

import FloatingDialog from '../FloatingDialog.vue';
import Changelog from './Changelog.vue';

const props = defineProps<{
  version: string;
}>();

const emit = defineEmits<{
  (event: 'close'): void;
}>();

const { version } = toRefs(props);
const { t } = useI18n();
const { loadChangelog } = useApi();
const changelog = ref<string | undefined>(undefined);

onMounted(() => {
  loadVersion(version.value);
});

watch(version, newVersion => {
  loadVersion(newVersion);
});

const loadVersion = async (version: string) => {
  try {
    const result = await loadChangelog(version);

    if (result.isError) {
      changelog.value = undefined;
    } else {
      changelog.value = result.data;
    }
  } catch {
    changelog.value = undefined;
  }
  if (changelog.value === undefined) {
    // don't show empty changelogs
    close();
  }
};

const close = () => {
  emit('close');
};
</script>
