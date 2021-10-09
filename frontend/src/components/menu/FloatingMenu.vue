<template>
  <div v-if="visible" class="fixed bottom-3 left-3 rounded-full bg-dark flex flex-row items-center">
    <div
      class="h-full bg-darkest p-2 rounded-full transform transition-transform duration-300 ease-out hover:scale-90"
      :class="{ 'rotate-90': isOpen }"
      @click="toggleOpen"
    >
      <Icon icon="carbon:settings" class="text-2xl tc-main-light" />
    </div>
    <!-- can't be w-auto, otherwise the transition won't work-->
    <div
      :class="{ 'w-16 p-2': isOpen }"
      class="w-0 p-0 h-full transition overflow-hidden duration-300 ease-out"
      style="transition-property: width, padding"
    >
      <div class="h-full flex flex-row items-center justify-end" id="content">
        <FullScreenSwitch />
        <DarkModeSwitch />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useConfig } from '@/core/adapter/config';
import { Icon } from '@iconify/vue';
import { computed, ref } from 'vue';
import DarkModeSwitch from '../settings/DarkModeSwitch.vue';
import FullScreenSwitch from '../settings/FullScreenSwitch.vue';

const config = useConfig();
const isOpen = ref(false);
const toggleOpen = () => (isOpen.value = !isOpen.value);
const visible = computed(() => config.showQuickMenu);
</script>

<style scoped>
#content > * {
  @apply mx-1;
}
</style>
