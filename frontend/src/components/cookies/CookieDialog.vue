<template>
  <FloatingDialog>
    <div class="flex flex-col mx-2 overflow-y-auto">
      <h2 class="tc-main text-xl text-center mt-2">{{ t('cookies.title') }}</h2>
      <p class="tc-main-secondary text-sm">{{ t('cookies.userInformation') }}</p>
      <p class="tc-main-secondary text-sm mb-2">
        {{ t('cookies.privacyLink') }}
        <router-link to="/privacy" class="underline" @click="close">
          {{ t('nav.privacy') }}
        </router-link>
      </p>
      <div class="py-1">
        <div class="flex justify-between items-center bg-light rounded p-1" @click="toggle('functional')">
          <h3 class="tc-main text-lg ml-1">{{ t('cookies.functional.title') }}</h3>
          <button class="text-2xl tc-main hover:tc-primary relative p-3 rounded w-6 h-4 overflow-hidden">
            <Icon
              icon="iconoir:nav-arrow-down"
              class="icon absolute left-1/2 top-1/2 -translate-x-1/2 transition duration-300"
              :class="{ 'opacity-0 translate-y-2': isOpen('functional'), '-translate-y-1/2': !isOpen('functional') }"
            />
            <Icon
              icon="iconoir:nav-arrow-up"
              class="icon absolute left-1/2 top-1/2 -translate-x-1/2 transition duration-300"
              :class="{ 'opacity-0 translate-y-2': !isOpen('functional'), '-translate-y-1/2': isOpen('functional') }"
            />
          </button>
        </div>

        <div
          class="relative grid overflow-hidden grid-rows-[0fr] transition-[grid-template-rows]"
          :class="{ 'grid-rows-[1fr]': isOpen('functional') }"
        >
          <div class="min-h-0">
            <div class="item">
              <div>
                <h4 class="text tc-main">{{ t('cookies.functional.auth.name') }}</h4>
                <p class="text-xs tc-main-secondary mb-1">{{ t('cookies.functional.auth.description') }}</p>
              </div>
              <div class="bg-main border-2 border-transparent px-4 py-1 ml-4 rounded cursor-not-allowed select-none">
                <p class="tc-main-secondary flex flex-row flex-nowrap justify-end items-center">
                  <span>{{ t('cookies.active') }}</span>
                  <Icon icon="akar-icons:check" class="ml-2" />
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="grid grid-rows-2 sm:grid-cols-2 sm:grid-rows-1 gap-2 mt-4">
        <div></div>
        <button class="flex justify-center items-center bg-main px-4 py-1 rounded transition hover:bg-dark cursor-pointer select-none" @click="close">
          <p class="tc-main-secondary text-center">{{ t('cookies.__close') }}</p>
        </button>
      </div>
      <!-- <div class="grid grid-rows-2 sm:grid-cols-2 sm:grid-rows-1 gap-2 mt-4">
        <button
          class="flex justify-center items-center bg-main px-4 py-1 rounded transition hover:bg-dark cursor-pointer select-none"
          @click="rejectAll"
        >
          <p class="tc-main-secondary text-center">{{ t('cookies.rejectAll') }}</p>
        </button>
        <button
          class="flex justify-center items-center bg-main px-4 py-1 rounded transition hover:bg-dark cursor-pointer select-none"
          @click="acceptSelection"
        >
          <p class="tc-main-secondary text-center">{{ t('cookies.acceptSelection') }}</p>
        </button>
      </div> -->
    </div>
  </FloatingDialog>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { useI18n } from 'vue-i18n';

import { Icon } from '@/components/misc/Icon';
import { useCookies } from '@/core/adapter/cookies';

import FloatingDialog from '../misc/FloatingDialog.vue';

const cookies = useCookies();
const { t } = useI18n();
const sectionsOpen = reactive<Record<string, boolean>>({});
// const recaptchaActivated = ref(true);

const isOpen = (key: string) => sectionsOpen[key];

const toggle = (key: string) => {
  sectionsOpen[key] = !sectionsOpen[key];
};

onMounted(() => {
  // recaptchaActivated.value = cookies.recaptchaCookie;
});

// const acceptSelection = () => {
//   // cookies.setReCaptchaCookie(recaptchaActivated.value);
//   cookies.didShowDialog();
// };

// const rejectAll = () => {
//   cookies.rejectAll();
//   cookies.didShowDialog();
// };

const close = () => cookies.rejectAll();
</script>

<style scoped>
.item {
  @apply flex flex-row flex-nowrap justify-between items-start my-1 ml-4;
}
</style>
