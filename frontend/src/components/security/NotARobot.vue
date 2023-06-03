<template>
  <div class="box my-2 bg-none rounded-lg relative mx-1 border-2">
    <VueHcaptcha
      :sitekey="siteKey"
      :language="locale"
      :theme="theme"
      @error="handleError"
      @verify="handleVerify"
      @expired="handleExpired"
      :ref="captchaRef"
    />
    <!-- <button class="block w-full h-full tc-main-dark py-2 px-1" :class="enableButtonPointerEvents" @click="handleClick">
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
    </button> -->
  </div>
</template>

<script setup lang="ts">
import VueHcaptcha from '@hcaptcha/vue3-hcaptcha';
import { computed, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';

import { useCaptcha } from '@/core/adapter/captcha';
import { useColorTheme } from '@/core/adapter/helper/useColorTheme';
import { CaptchaResponse } from '@/core/api/entities/Captcha';
import { useRuntimeConfig } from '@/core/runtimeConfig';

const props = defineProps<{
  response?: CaptchaResponse;
}>();

const emit = defineEmits<{
  (event: 'responseChanged', response: CaptchaResponse): void;
}>();

const { locale } = useI18n();
const theme = useColorTheme();
const captcha = useCaptcha();
const config = useRuntimeConfig();
const siteKey = computed(() => config.recaptchaKey);
const captchaRef = ref<VueHcaptcha | undefined>(undefined);

const handleError = () => {
  emit('responseChanged', {
    score: 0,
    success: false,
    error: 'error'
  });
};

const handleExpired = () => {
  emit('responseChanged', {
    score: 0,
    success: false,
    error: 'expired'
  });
};

const handleVerify = async (token: string) => {
  const response = await captcha.check(token);
  if (response) {
    emit('responseChanged', response);
  } else {
    emit('responseChanged', {
      score: 0,
      success: false,
      error: 'failed'
    });
    captchaRef.value?.reset();
  }
};

watch(
  () => props.response,
  newResponse => {
    if (!newResponse) {
      captchaRef.value?.reset();
    }
  }
);
</script>
