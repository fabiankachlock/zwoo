<template>
  <FloatingDialog :without-padding="true" v-show="changelog">
    <div class="sticky top-0 z-10 px-5 pt-5 bg-lightest">
      <h2 class="tc-main text-xl text-center py-2 bc-darkest border-b">{{ t('changelog.title', [version]) }}</h2>
    </div>
    <div id="changelog" :ref="r => changelogWrapper = (r as HTMLDivElement)" class="py-1 px-5"></div>
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

import { ConfigService } from '@/core/services/api/Config';
import { unwrapBackendError } from '@/core/services/api/errors';

import FloatingDialog from '../FloatingDialog.vue';

const props = defineProps<{
  version: string;
}>();

const emit = defineEmits<{
  (event: 'close'): void;
}>();

const { version } = toRefs(props);
const { t } = useI18n();
const changelogWrapper = ref<HTMLDivElement | null>(null);
const changelog = ref<string | undefined>(undefined);

onMounted(() => {
  loadVersion(version.value);
});

watch(version, newVersion => {
  loadVersion(newVersion);
});

watch(changelog, newChangelog => {
  if (changelogWrapper.value && newChangelog) {
    changelogWrapper.value.innerHTML = newChangelog;
  }
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

<style>
#changelog {
  @apply text-_text-dark-secondary dark:text-_text-light-secondary;
}

#changelog h1 {
  @apply text-4xl;
  @apply text-primary-text-dark dark:text-primary-text-light;
}

#changelog h2 {
  @apply text-3xl;
}

#changelog h3 {
  @apply text-2xl;
}

#changelog h4 {
  @apply text-xl;
}

#changelog h5 {
  @apply text-lg;
}

#changelog h6 {
  @apply text-sm;
}

#changelog ol {
  @apply list-inside list-decimal;
}

#changelog ul {
  @apply list-inside list-disc;
}
</style>
