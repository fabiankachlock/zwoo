<template>
  <div class="w-full mb-4">
    <div class="m-2">
      <label class="block tc-main-secondary text-sm font-bold mb-2" :for="id">{{ t(labelKey) }}</label>
      <input
        :ref="
        r => {
          input = r as HTMLInputElement;
        }
      "
        class="
          bg-dark
          shadow
          appearance-none
          border
          bc-main
          rounded
          w-full
          py-2
          px-3
          tc-main-light
          leading-tight
          focus:outline-none focus:shadow-outline focus:border-primary-light focus:bg-darkest
          dark:focus:border-primary-dark
        "
        :type="isPassword ? 'password' : 'text'"
        :id="id"
        :placeholder="placeholder"
        @keyup="updateInput"
      />
      <div class="my-2">
        <Error v-show="!isValid" :errors="error" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Validator } from '@/core/services/validator/_type';
import { defineEmits, defineProps, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import Error from '../misc/Error.vue';

const props = defineProps<{
  id: string;
  labelKey: string;
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
const error = ref<string[]>([]);

const { t } = useI18n();

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
