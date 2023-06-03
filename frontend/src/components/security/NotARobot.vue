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
  </div>
</template>

<script setup lang="ts">
import VueHcaptcha from '@hcaptcha/vue3-hcaptcha';
import { computed, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';

import { useColorTheme } from '@/core/adapter/helper/useColorTheme';
import { useRuntimeConfig } from '@/core/runtimeConfig';

const props = defineProps<{
  token?: string;
}>();

const emit = defineEmits<{
  (event: 'responseChanged', token?: string): void;
}>();

const { locale } = useI18n();
const theme = useColorTheme();
const config = useRuntimeConfig();
const siteKey = computed(() => config.captchaKey);
const captchaRef = ref<VueHcaptcha | undefined>(undefined);

const handleError = () => {
  emit('responseChanged', undefined);
};

const handleExpired = () => {
  emit('responseChanged', undefined);
};

const handleVerify = async (token: string) => {
  emit('responseChanged', token);
};

watch(
  () => props.token,
  newToken => {
    if (!newToken) {
      captchaRef.value?.reset();
    }
  }
);
</script>
