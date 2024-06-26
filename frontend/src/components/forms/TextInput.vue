<template>
  <div class="w-full mb-4">
    <div class="m-2">
      <label class="block text-text-secondary text-sm font-bold mb-2" :for="id">
        {{ labelKey && t(labelKey) }}
        <slot></slot>
      </label>
      <div class="relative">
        <input
          :id="id"
          :ref="
            r => {
              input = r as HTMLInputElement;
            }
          "
          autocomplete=""
          class="bg-surface shadow appearance-none border border-border rounded w-full py-2 pl-3 pr-7 text-text leading-tight focus:outline-none focus:shadow-outline focus:border-primary hover:bg-surface-hover focus:bg-surface-hover"
          :name="id"
          :type="isPassword ? (isPasswordVisible ? 'text' : 'password') : 'text'"
          :placeholder="placeholder"
          :value="modelValue"
          @keyup.stop
          @keydown.enter.prevent
          @input="updateInput"
        />
        <button
          v-if="isPassword"
          class="absolute right-2 top-1/2 transform -translate-y-1/2 text-text text-lg"
          @click.stop.prevent="togglePasswordMode"
        >
          <Icon v-show="!isPasswordVisible" icon="mdi:eye-outline" />
          <Icon v-show="isPasswordVisible" icon="mdi:eye-off-outline" />
        </button>
      </div>
      <div class="my-2">
        <Error v-show="!isValid" :errors="error" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';

import { Icon } from '@/components/misc/Icon';
import { Validator } from '@/core/services/validator/_type';

import Error from '../misc/Error.vue';

const props = defineProps<{
  id: string;
  labelKey?: string;
  modelValue: string;
  validator?: Validator<string>;
  placeholder?: string;
  isPassword?: boolean;
}>();

const emit = defineEmits<{
  (event: 'update:modelValue', value: string): void;
  (event: 'update:isValid', valid: boolean): void;
}>();

const input = ref<HTMLInputElement>();
const isValid = ref<boolean>(true);
const isPasswordVisible = ref<boolean>(false);
const error = ref<string[]>([]);

const { t } = useI18n();

const togglePasswordMode = () => {
  isPasswordVisible.value = !isPasswordVisible.value;
};

const updateInput = () => {
  const newValue = input.value?.value || '';
  const validationResult = props.validator ? props.validator.validate(newValue) : undefined;

  if (validationResult) {
    isValid.value = validationResult.isValid;
    error.value = validationResult.getErrors();
  }

  emit('update:modelValue', newValue);
  emit('update:isValid', isValid.value);
};
</script>
