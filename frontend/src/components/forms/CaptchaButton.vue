<template>
  <NotARobot :token="token" @response-changed="changed" />
</template>

<script setup lang="ts">
import { toRefs } from 'vue';

import { Validator } from '@/core/services/validator/_type';

import NotARobot from '../security/NotARobot.vue';

const props = defineProps<{
  validator?: Validator<string | undefined>;
  token?: string;
}>();

const { validator } = toRefs(props);

const emit = defineEmits<{
  (event: 'update:isValid', valid: boolean): void;
  (event: 'update:response', token?: string): void;
}>();

const changed = (response: string | undefined) => {
  if (validator?.value && validator.value) {
    const result = validator.value.validate(response);
    emit('update:isValid', result.isValid);
  }
  emit('update:response', response);
};
</script>
