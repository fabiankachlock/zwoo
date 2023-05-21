<template>
  <p v-if="readonly" class="tc-main-light leading-tight">{{ modelValue }}</p>
  <!-- TODO: make min & max danymic -->
  <input
    v-else
    :id="name ?? 'rule-input'"
    autocomplete=""
    class="bg-dark shadow appearance-none border bc-main rounded w-20 h-full py-1 px-2 tc-main-light leading-tight focus:outline-none focus:shadow-outline focus:bc-primary focus:bg-darkest"
    :name="name ?? 'rule-input'"
    type="number"
    min="1"
    max="20"
    :placeholder="placeholder ?? t('rules.widget.numberPlaceholder')"
    :value="modelValue"
    :readonly="readonly"
    @keyup.stop
    @input="update($event)"
    @click.stop=""
  />
</template>

<script setup lang="ts">
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
  const num = new Number((event.target as unknown as { value: number }).value).valueOf();
  // TODO: make min & max dynamic
  emit('update:modelValue', num < 1 ? 1 : num > 20 ? 20 : num);
};
</script>
