<template>
  <div :class="styles" @click="toggle">
    <Icon v-show="!checked" icon="akar-icons:box" />
    <Icon v-show="checked" icon="akar-icons:check" />
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { ref, defineEmits, defineProps, toRefs, watch } from 'vue';

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
