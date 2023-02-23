<template>
  <FloatingDialog :without-padding="true" v-show="changelog">
    <div class="sticky top-0 z-10 px-5 pt-5 bg-lightest">
      <h2 class="tc-main text-xl text-center py-2 bc-darkest border-b">{{ t('changelog.title', [version]) }}</h2>
    </div>
    <Changelog :changelog="changelog" />
    <div class="sticky -bottom-[0.5px] bg-lightest px-5 pb-3">
      <div class="flex justify-end items-center bc-darkest border-t">
        <button
          class="mt-4 flex justify-center items-center bg-main border-2 border-primary px-4 py-1 rounded transition hover:bg-dark cursor-pointer select-none"
          @click="close"
        >
          <p class="tc-primary text-center">{{ t('changelog.close') }}</p>
        </button>
      </div>
    </div>
  </FloatingDialog>
</template>

<script setup lang="ts">
import { defineEmits, defineProps, onMounted, ref, toRefs, watch } from 'vue';
import { useI18n } from 'vue-i18n';

import { ConfigService } from '@/core/api/restapi/Config';
import { unwrapBackendError } from '@/core/api/restapi/Errors';

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
const changelog = ref<string | undefined>(undefined);

onMounted(() => {
  loadVersion(version.value);
});

watch(version, newVersion => {
  loadVersion(newVersion);
});

const loadVersion = async (version: string) => {
  try {
    const result = await ConfigService.fetchChangelog(version);
    const [changelogHtml, error] = unwrapBackendError(result);

    if (error) {
      changelog.value = undefined;
    } else {
      changelog.value = changelogHtml;
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
