<template>
  <button @click.stop="toggle">
    <div
      class="px-4 relative rounded bg-main hover:bg-dark transition border-2 switch-body"
      :class="{
        'on tc-main bc-primary': modelValue,
        'off tc-main-light bc-lightest': !modelValue
      }"
    >
      <div class="thumb"></div>
      <p class="text-md z-10 w-6">
        {{ t(`controls.toggle.${modelValue ? 'on' : 'off'}`) }}
      </p>
    </div>
  </button>
</template>

<script setup lang="ts">
import { defineEmits, defineProps } from 'vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

const props = defineProps<{
  modelValue: boolean;
}>();

const emit = defineEmits<{
  (event: 'toggle', enabled: boolean): void;
  (event: 'update:modelValue', enabled: boolean): void;
}>();

const toggle = () => {
  emit('update:modelValue', !props.modelValue);
  emit('toggle', !props.modelValue);
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
