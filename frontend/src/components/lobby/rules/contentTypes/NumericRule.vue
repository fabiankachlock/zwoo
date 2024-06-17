<template>
  <p v-if="readonly" class="text-text-light leading-tight">{{ modelValue }}</p>
  <input
    v-else
    :id="name ?? 'rule-input'"
    autocomplete=""
    class="bg-bg-surface shadow appearance-none border border-border rounded w-20 h-full py-1 px-2 text-text-light leading-tight focus:outline-none focus:shadow-outline focus:border-primary focus:bg-darkest"
    :name="name ?? 'rule-input'"
    type="number"
    :min="min"
    :max="max"
    :placeholder="placeholder ?? t('rules.widget.numberPlaceholder')"
    :value="modelValue"
    :readonly="readonly"
    @keyup.stop
    @input="update($event)"
    @click.stop=""
  />
</template>

<script setup lang="ts">
import { toRefs } from 'vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
const props = defineProps<{
  modelValue: number;
  name?: string;
  placeholder?: string;
  readonly?: boolean;
  min?: number;
  max?: number;
}>();
const { min, max } = toRefs(props);

const emit = defineEmits<{
  (event: 'update:modelValue', enabled: number): void;
}>();

const update = (event: Event) => {
  const target = event.target as unknown as { value: string };
  let num = new Number(target.value).valueOf();
  if (max?.value !== undefined && num > max.value) {
    num = max.value;
  }
  if (min?.value !== undefined && num < min.value) {
    num = min.value;
  }

  emit('update:modelValue', num);
  // "allow" empty values
  if (target.value !== '') {
    target.value = num.toString();
  }
};
</script>
