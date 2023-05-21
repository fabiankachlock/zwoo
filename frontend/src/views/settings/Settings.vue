<template>
  <MaxWidthLayout size="normal">
    <h1 class="tc-main text-4xl my-5">{{ t('settings.title') }}</h1>
    <div class="flex flex-col lg:flex-row items-start">
      <div class="relative w-full p-2 bg-lightest rounded-lg my-4 lg:mr-4 lg:max-w-xs">
        <div class="flex justify-between items-center">
          <h3 class="tc-main text-2xl mx-2 my-1">{{ t(`settings.sections.${currentSection}`) }}</h3>
          <button
            class="toggle lg:hidden bg-dark hover:bg-darkest text-2xl tc-main relative p-4 rounded w-6 h-6 overflow-hidden"
            @click="toggleOpenState"
          >
            <Icon
              icon="iconoir:nav-arrow-down"
              class="icon absolute left-1/2 top-1/2 -translate-x-1/2 transition duration-300"
              :class="{ 'opacity-0 translate-y-2': isMenuOpen, '-translate-y-1/2': !isMenuOpen }"
            />
            <Icon
              icon="iconoir:nav-arrow-up"
              class="icon absolute left-1/2 top-1/2 -translate-x-1/2 transition duration-300"
              :class="{ 'opacity-0 translate-y-2': !isMenuOpen, '-translate-y-1/2': isMenuOpen }"
            />
          </button>
        </div>
        <div class="menu-body transition duration-300" :class="{ open: isMenuOpen, 'overflow-hidden lg:block lg:max-h-full': !isMenuOpen }">
          <router-link
            v-for="section in displaySections"
            :key="section"
            class="block tc-main-light bg-dark px-3 py-1 my-2 rounded-lg border border-transparent mouse:hover:bc-primary mouse:hover:bg-darkest"
            :class="{ 'bc-primary': section === currentSection }"
            :to="`/settings/${section}`"
          >
            {{ t(`settings.sections.${section}`) }}
          </router-link>
        </div>
      </div>
      <div class="w-full lg:flex-1">
        <router-view></router-view>
      </div>
    </div>
  </MaxWidthLayout>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute } from 'vue-router';

import { Icon } from '@/components/misc/Icon';
import { useAuth } from '@/core/adapter/auth';
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';
import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';

const { t } = useI18n();
const route = useRoute();
const auth = useAuth();
const config = useConfig();

const allSections = ['general', 'account', 'game', 'developers', 'about'];
const isLoggedIn = computed(() => auth.isLoggedIn);
const currentSection = ref('');
const showDevSettings = computed(() => config.get(ZwooConfigKey.DevSettings));
const isMenuOpen = ref(true);

const blockedSections = computed(() => [...[isLoggedIn.value ? '-' : 'account'], ...[showDevSettings.value ? '-' : 'developers']]);
const displaySections = computed(() => allSections.filter(section => !blockedSections.value.includes(section)));

watch(
  () => route.fullPath,
  currentPath => {
    console.log(blockedSections.value, displaySections.value);
    currentSection.value = allSections.find(section => currentPath.includes(section)) ?? '';
  },
  {
    immediate: true
  }
);
const toggleOpenState = () => {
  isMenuOpen.value = !isMenuOpen.value;
};
</script>

<style scoped>
.menu-body {
  transition-property: max-height;
  max-height: 0;
  @apply lg:max-h-full;
}
.menu-body.open {
  transition-property: max-height;
  max-height: 5000px;
}

.toggle:hover .icon {
  @apply scale-110;
}
</style>
