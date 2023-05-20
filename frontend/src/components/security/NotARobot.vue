<template>
  <div class="box my-2 bg-none rounded-lg relative mx-1 border-2">
    <button class="block w-full h-full tc-main-dark py-2 px-1" :class="enableButtonPointerEvents" @click="handleClick">
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
import { computed, defineEmits, defineProps, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';

import { useCaptcha } from '@/core/adapter/captcha';
import { CaptchaResponse } from '@/core/api/entities/Captcha';
import { MIN_RECAPTCHA_SCORE } from '@/core/services/validator/recaptcha';

const props = defineProps<{
  response?: CaptchaResponse;
}>();

const emit = defineEmits<{
  (event: 'responseChanged', response: CaptchaResponse): void;
}>();

const { t } = useI18n();
const captcha = useCaptcha();
const verifyState = ref<'none' | 'verifying' | 'done' | 'error'>('none');
const success = computed(() => props.response?.success ?? false);
const humanRate = computed(() => props.response?.score ?? 0);

watch(
  () => props.response,
  newResponse => {
    if (!newResponse) {
      verifyState.value = 'none';
    } else {
      verifyState.value = 'done';
    }
  }
);

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
    const response = await captcha.performCheck();
    if (response) {
      emit('responseChanged', response);
    }
  } catch {
    verifyState.value = 'error';
  }
};
</script>

<style scoped>
.box {
  border-color: #1a73e8;
}
</style>
