<template>
  <NotARobot @response-changed="changed" :response="response" />
</template>

<script setup lang="ts">
import { defineEmits, defineProps, toRefs } from 'vue';

import { ReCaptchaResponse } from '@/core/services/api/Captcha';
import { Validator } from '@/core/services/validator/_type';

import NotARobot from '../security/NotARobot.vue';

const props = defineProps<{
  validator?: Validator<ReCaptchaResponse>;
  response?: ReCaptchaResponse;
}>();

const { validator } = toRefs(props);

const emit = defineEmits<{
  (event: 'update:isValid', valid: boolean): void;
  (event: 'update:response', response: ReCaptchaResponse): void;
}>();

const changed = (response: ReCaptchaResponse) => {
  if (validator && validator.value) {
    const result = validator.value.validate(response);
    emit('update:isValid', result.isValid);
  }
  emit('update:response', response);
};
</script>
