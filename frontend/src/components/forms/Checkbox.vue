<template>
  <div class="m-2 my-4 flex no-wrap" :class="{ 'flex-row-reverse': position === 'start', [`items-${align ?? 'center'}`]: true }">
    <label class="text-text-secondary text-sm font-bold relative">
      <slot></slot>
    </label>
    <!-- keep this next div, otherwise items-`x` might not be available when it is not used anywhere else -->
    <div class="items-start items-center items-end"></div>
    <button :class="styles" class="hover:border-primary border border-transparent rounded text-primary-text mx-3 my-1" @click.prevent="toggle">
      <Icon v-show="!checked" icon="akar-icons:box" />
      <Icon v-show="checked" icon="akar-icons:check" />
    </button>
  </div>
</template>

<script setup lang="ts">
import { ref, toRefs, watch } from 'vue';

import { Icon } from '@/components/misc/Icon';

const checked = ref(false);

const props = defineProps<{
  styles?: string;
  modelValue: boolean;
  align?: 'start' | 'center' | 'end';
  position?: 'start' | 'end';
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
