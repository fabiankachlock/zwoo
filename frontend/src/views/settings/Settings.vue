<template>
  <MaxWidthLayout size="normal">
    <h1 class="tc-main text-4xl my-5">{{ t('settings.title') }}</h1>
    <div>
      <div class="relative w-full p-2 flex flex-col justify-end items-stretch bg-lightest rounded-lg my-4">
        <div>
          <h3>{{ t(`settings.sections.${currentSection}`) }}</h3>
          <button>TOGGLE</button>
        </div>
        <div>
          <router-link v-for="section in displaySections" :key="section" :to="`/settings/${section}`">
            {{ t(`settings.sections.${section}`) }}
          </router-link>
        </div>
      </div>
      <div>
        <router-view></router-view>
      </div>
    </div>
  </MaxWidthLayout>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute } from 'vue-router';

import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';

const { t } = useI18n();
const route = useRoute();

const allSections = ['general', 'account', 'game', 'developers', 'about'];
const currentSection = ref('');
const showDevSettings = ref(localStorage.getItem('zwoo:dev-settings') !== 'true');

const displaySections = computed(() => (showDevSettings.value ? allSections : allSections.filter(section => section === 'developers')));

watch(
  () => route.fullPath,
  currentPath => {
    currentSection.value = allSections.find(section => currentPath.includes(section)) ?? '';
  },
  {
    immediate: true
  }
);
</script>
