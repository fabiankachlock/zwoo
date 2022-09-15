<template>
  <p v-if="readonly" class="tc-main-light leading-tight">{{ modelValue }}</p>
  <input
    v-else
    autocomplete=""
    class="bg-dark shadow appearance-none border bc-main rounded w-20 h-full py-1 px-2 tc-main-light leading-tight focus:outline-none focus:shadow-outline focus:bc-primary focus:bg-darkest"
    :name="name ?? 'rule-input'"
    type="number"
    :id="name ?? 'rule-input'"
    :placeholder="placeholder ?? t('rules.widget.numberPlaceholder')"
    :value="modelValue"
    :readonly="readonly"
    @keyup.stop
    @input="update($event)"
    @click.stop=""
  />
</template>

<script setup lang="ts">
import { defineEmits, defineProps } from 'vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
defineProps<{
  modelValue: number;
  name?: string;
  placeholder?: string;
  readonly?: boolean;
}>();

const emit = defineEmits<{
  (event: 'update:modelValue', enabled: number): void;
}>();

const update = (event: Event) => {
  emit('update:modelValue', new Number((event.target as unknown as { value: number }).value).valueOf());
};
</script>
