<template>
  <div v-if="isOpen">
    <ChangelogDialog :version="version" @close="close" />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useChangelog } from '@/core/adapter/changelog';
import ChangelogDialog from './ChangelogDialog.vue';

const changelog = useChangelog();
changelog.setup();
const version = computed(() => changelog.version);
const isOpen = computed(() => changelog.popupOpen);

const close = () => {
  changelog.didShowDialog();
};
</script>
