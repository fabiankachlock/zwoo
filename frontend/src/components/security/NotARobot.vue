<template>
  <div class="box my-2 bg-none rounded-lg relative mx-1 border-2">
    <button @click="handleClick" class="block w-full h-full tc-main-dark py-2 px-1" :class="enableButtonPointerEvents">
      <div v-if="verifyState === 'none'">
        <p>
          <img src="https://www.gstatic.com/recaptcha/api2/logo_48.png" alt="reCAPTCHA Logo" class="h-8 mr-2 inline-block align-middle" />
          <span class="align-middle">
            {{ t('recaptcha.title') }}
          </span>
        </p>
      </div>
      <div v-else-if="verifyState === 'verifying'">{{ t('recaptcha.loading') }}</div>
      <div v-else-if="verifyState === 'error'">{{ t('recaptcha.error') }}</div>
      <div v-else-if="verifyState === 'done'">
        <p v-if="!success || humanRate < MIN_RECAPTCHA_SCORE">{{ t('recaptcha.result.fail') }}</p>
        <p v-else>{{ t('recaptcha.result.success') }}</p>
      </div>
    </button>
  </div>
</template>

<script setup lang="ts">
import { computed, defineEmits, ref } from 'vue';
import { useI18n } from 'vue-i18n';

import { ReCaptchaResponse, ReCaptchaService } from '@/core/services/api/reCAPTCHA';
import { MIN_RECAPTCHA_SCORE } from '@/core/services/validator/recaptcha';

const emit = defineEmits<{
  (event: 'responseChanged', response: ReCaptchaResponse): void;
}>();

const { t } = useI18n();
const verifyState = ref<'none' | 'verifying' | 'done' | 'error'>('none');
const humanRate = ref<number>(0);
const success = ref<boolean>(false);

const enableButtonPointerEvents = computed(() => {
  if (verifyState.value === 'verifying' || verifyState.value === 'error' || (verifyState.value === 'done' && success.value)) {
    return 'pointer-events-none';
  }
  return 'pointer-events-auto';
});

const handleClick = async (evt: Event) => {
  evt.stopPropagation();
  evt.preventDefault();
  verifyState.value = 'verifying';

  try {
    const response = await ReCaptchaService.checkUser();
    verifyState.value = 'done';

    if (response) {
      emit('responseChanged', response);
      success.value = response.success;
      humanRate.value = response.score;
    }
  } catch {
    verifyState.value = 'error';
  }
};
</script>

<style>
.box {
  border-color: #1a73e8;
}
</style>
