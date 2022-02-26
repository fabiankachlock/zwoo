<template>
  <button @click.stop="toggle">
    <div
      class="px-4 relative rounded bg-main hover:bg-dark transition border-2 switch-body"
      :class="{
        'on tc-main bc-primary': isEnabled,
        'off tc-main-light bc-lightest': !isEnabled
      }"
    >
      <div class="thumb"></div>
      <p class="text-md z-10">
        {{ isEnabled ? 'On' : 'Off' }}
      </p>
    </div>
  </button>
</template>

<script setup lang="ts">
import { defineEmits, defineProps, ref, toRefs } from 'vue';

const isEnabled = ref(false);
const props = defineProps<{
  modelValue?: boolean;
}>();

const { modelValue } = toRefs(props);
isEnabled.value = modelValue?.value ?? false;

const emit = defineEmits<{
  (event: 'toggle', enabled: boolean): void;
  (event: 'update:modelValue', enabled: boolean): void;
}>();

const toggle = () => {
  isEnabled.value = !isEnabled.value;
  emit('update:modelValue', isEnabled.value);
  emit('toggle', isEnabled.value);
};
</script>

<style scoped>
.thumb {
  top: 0;
  bottom: 0;
  @apply transition-all duration-200 rounded-sm w-2 absolute;
}
.switch-body.off .thumb {
  left: 0;
  @apply bg-_bg-light-lightest dark:bg-_bg-dark-lightest;
}

.switch-body.on .thumb {
  left: calc(100% - 0.5rem);
  @apply bg-primary-dark dark:bg-primary-light;
}
</style>
