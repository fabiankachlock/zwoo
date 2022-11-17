<template>
  <div class="m-2 mb-4 flex no-wrap items-center">
    <label class="tc-main-secondary text-sm font-bold my-2 relative">
      <slot></slot>
    </label>
    <button :class="styles" class="hover:bc-primary border border-transparent rounded" @click.prevent="toggle">
      <Icon v-show="!checked" icon="akar-icons:box" />
      <Icon v-show="checked" icon="akar-icons:check" />
    </button>
  </div>
</template>

<script setup lang="ts">
import { defineEmits, defineProps, ref, toRefs, watch } from 'vue';

import { Icon } from '@/components/misc/Icon';

const checked = ref(false);

const props = defineProps<{
  styles: string;
  modelValue: boolean;
}>();

const { modelValue } = toRefs(props);
checked.value = modelValue.value;

watch([modelValue], ([value]) => {
  checked.value = value;
});

const emit = defineEmits<{
  (event: 'update:modelValue', checked: boolean): void;
}>();

const toggle = () => {
  checked.value = !checked.value;
  emit('update:modelValue', checked.value);
};
</script>
