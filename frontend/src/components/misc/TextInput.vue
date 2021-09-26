<template>
  <div class="w-full">
    <div class="m-2">
      <label class="block tc-main-secondary text-sm font-bold mb-2" :for="id">{{ t(labelKey) }}</label>
      <input
        :ref="
        r => {
          input = r as HTMLInputElement;
        }
      "
        class="
          shadow
          appearance-none
          border
          bc-invert-main
          rounded
          w-full
          py-2
          px-3
          tc-main-dark
          leading-tight
          focus:outline-none focus:shadow-outline focus:border-primary
        "
        type="text"
        :id="id"
        :placeholder="placeholder"
        @keyup="updateInput"
      />
      <div class="my-2">
        <Error v-show="!isValid" :title="error" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { defineProps, defineEmits, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import Error from './Error.vue';

const props = defineProps<{
  id: string;
  labelKey: string;
  modelValue: string;
  validate?: (str: string) => [boolean, string];
  placeholder?: string;
}>();

const emit = defineEmits<{
  (event: 'update:modelValue', value: string): void;
  (event: 'update:isValid', valid: boolean): void;
}>();

const input = ref<HTMLInputElement>();
const isValid = ref<boolean>(!!props.validate);
const error = ref<string>('');

const { t } = useI18n();

const updateInput = () => {
  const newValue = input.value?.value || '';
  [isValid.value, error.value] = props.validate ? props.validate(newValue) : [true, ''];
  console.log(props.validate!(newValue));

  emit('update:modelValue', newValue);
  emit('update:isValid', isValid.value);
};
</script>
