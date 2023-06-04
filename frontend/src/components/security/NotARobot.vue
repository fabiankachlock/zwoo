<template>
  <div class="box my-2 bg-none rounded-lg relative mx-2">
    <VueHcaptcha
      v-if="siteKey"
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
import { onMounted, ref, watch } from 'vue';
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
const siteKey = ref<string | undefined>(undefined);
const captchaRef = ref<VueHcaptcha | undefined>(undefined);

onMounted(async () => {
  siteKey.value = await config.captchaKey;
});

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
