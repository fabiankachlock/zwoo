<template>
  <FloatingDialog>
    <div class="flex flex-col mx-2 overflow-y-auto">
      <h2 class="tc-main text-xl text-center my-2">{{ t('cookies.title') }}</h2>
      <div class="bc-darkest border-t border-b py-1">
        <h3 class="tc-main text-lg">{{ t('cookies.functional.title') }}</h3>
        <ul class="ml-4 list">
          <li class="bc-main">
            <div>
              <h4 class="text tc-main">{{ t('cookies.functional.auth.name') }}</h4>
              <p class="text-xs tc-main-secondary ml-2 mb-1">{{ t('cookies.functional.auth.description') }}</p>
            </div>
            <div class="bg-main border-2 border-transparent px-4 py-1 ml-4 rounded cursor-not-allowed select-none">
              <p class="tc-main-secondary flex flex-row flex-nowrap justify-end items-center">
                <span>{{ t('cookies.active') }}</span>
                <Icon icon="akar-icons:check" class="ml-2" />
              </p>
            </div>
          </li>
          <li class="bc-main">
            <div>
              <h4 class="text tc-main">{{ t('cookies.functional.recaptcha.name') }}</h4>
              <p class="text-xs tc-main-secondary ml-2 mb-1">{{ t('cookies.functional.recaptcha.description') }}</p>
            </div>
            <button
              class="bg-main border-2 border-transparent px-4 py-1 ml-4 rounded transition hover:bg-dark cursor-pointer select-none"
              :class="{ 'tc-primary': recaptchaActivated, 'tc-main-secondary': !recaptchaActivated }"
              @click="recaptchaActivated = !recaptchaActivated"
            >
              <p class="flex flex-row flex-nowrap justify-end items-center">
                <span> {{ t(recaptchaActivated ? 'cookies.active' : 'cookies.inactive') }} </span>
                <Icon v-if="recaptchaActivated" icon="akar-icons:check" class="ml-2" />
                <Icon v-else icon="akar-icons:cross" class="ml-2" />
              </p>
            </button>
          </li>
        </ul>
      </div>
      <div class="grid grid-cols-2 gap-2 mt-4">
        <button
          class="flex justify-center items-center bg-light border-2 border-transparent px-4 py-1 rounded transition hover:bg-main cursor-pointer select-none"
          @click="acceptSelection"
        >
          <p class="tc-main-secondary text-center">{{ t('cookies.acceptSelection') }}</p>
        </button>
        <button
          class="flex justify-center items-center bg-main border-2 border-primary px-4 py-1 rounded transition hover:bg-dark cursor-pointer select-none"
          @click="acceptAll"
        >
          <p class="tc-primary text-center">{{ t('cookies.acceptAll') }}</p>
        </button>
      </div>
    </div>
  </FloatingDialog>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

import { Icon } from '@/components/misc/Icon';
import { useCookies } from '@/core/adapter/cookies';

import FloatingDialog from '../misc/FloatingDialog.vue';

const cookies = useCookies();
const { t } = useI18n();

const recaptchaActivated = ref(true);

onMounted(() => {
  recaptchaActivated.value = cookies.recaptchaCookie;
});

const acceptSelection = () => {
  cookies.setReCaptchaCookie(recaptchaActivated.value);
  cookies.didShowDialog();
};

const acceptAll = () => {
  cookies.acceptAll();
  cookies.didShowDialog();
};
</script>

<style scoped>
.list li {
  @apply flex flex-row flex-nowrap justify-between items-center my-1 border-b;
}
</style>
