<template>
  <div v-if="visible" class="quick-menu fixed left-3 rounded-full bg-dark flex flex-row items-center">
    <div
      class="h-full bg-darkest p-2 rounded-full transition-transform duration-300 ease-out hover:scale-90 cursor-pointer"
      :class="{ 'rotate-90': isOpen }"
      @click="toggleOpen"
    >
      <Icon icon="carbon:settings" class="text-3xl sm:text-2xl tc-main-light" />
    </div>
    <!-- can't be w-auto, otherwise the transition won't work -->
    <div
      :class="{ 'open p-2': isOpen }"
      class="w-0 h-full transition overflow-hidden duration-300 ease-out"
      style="transition-property: width, padding"
    >
      <div class="h-full flex flex-row items-center justify-end" id="content">
        <HomeButton />
        <FullScreenSwitch />
        <DarkModeSwitch />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { Icon } from '@/components/misc/Icon';
import { useUserDefaults } from '@/composables/userDefaults';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

import DarkModeSwitch from '../settings/DarkModeSwitch.vue';
import FullScreenSwitch from '../settings/FullScreenSwitch.vue';
import HomeButton from '../settings/HomeButton.vue';

const config = useConfig();
const isOpen = useUserDefaults('floatingMenu:open', false);
const toggleOpen = () => (isOpen.value = !isOpen.value);
const visible = computed(() => config.get(ZwooConfigKey.QuickMenu));
</script>

<style scoped>
.quick-menu {
  bottom: calc(2rem + 0.75rem + env(safe-area-inset-bottom));
}

#content > * {
  @apply mx-1;
}

.open {
  width: 6rem; /* w-24 */
}
</style>
