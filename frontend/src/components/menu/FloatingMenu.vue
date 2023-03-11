<template>
  <!-- TODO: this this is probably deprecated -->
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
        <router-link to="/settings">
          <Icon icon="akar-icons:settings-horizontal" class="tc-main text-xl ease-linear transition-transform hover:scale-110"></Icon>
        </router-link>
        <div @click="toggleDarkMode" class="tc-main text-xl ease-linear transition-transform hover:scale-110">
          <Icon icon="ri:moon-fill" v-show="isDarkMode" />
          <Icon icon="ri:sun-fill" v-show="!isDarkMode" />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

import { Icon } from '@/components/misc/Icon';
import { useUserDefaults } from '@/composables/userDefaults';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

const config = useConfig();
const isOpen = useUserDefaults('floatingMenu:open', false);
const toggleOpen = () => (isOpen.value = !isOpen.value);
const visible = computed(() => config.get(ZwooConfigKey.QuickMenu));
const isDarkMode = computed(() => config.get(ZwooConfigKey.UiMode) === 'dark');

const toggleDarkMode = () => {
  config.set(ZwooConfigKey.UiMode, !isDarkMode.value ? 'dark' : 'light');
};
</script>

<style scoped>
.quick-menu {
  bottom: calc(2rem + 0.75rem + env(safe-area-inset-bottom));
}

#content > * {
  @apply mx-1;
}

.open {
  width: 4rem; /* w-16 */
}
</style>
