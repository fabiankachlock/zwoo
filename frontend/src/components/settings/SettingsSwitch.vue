<template>
  <button @click.stop="toggle">
    <div
      class="relative rounded bg-main hover:bg-alt transition border-2 switch-body"
      :class="{
        'on tc-main bc-primary': modelValue,
        'off tc-main-light bc-lightest': !modelValue,
        'pr-6': modelValue,
        'pl-6': !modelValue
      }"
    >
      <div class="thumb">
        <div class="w-full h-full grid place-items-center">
          <slot></slot>
        </div>
      </div>
      <p class="text-md z-10 w-8 mx-1">
        {{ t(`controls.toggle.${modelValue ? 'on' : 'off'}`) }}
      </p>
    </div>
  </button>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

const props = defineProps<{
  modelValue: boolean;
  readonly?: boolean;
}>();

const emit = defineEmits<{
  (event: 'toggle', enabled: boolean): void;
  (event: 'update:modelValue', enabled: boolean): void;
}>();

const toggle = () => {
  if (props.readonly) return;
  emit('update:modelValue', !props.modelValue);
  emit('toggle', !props.modelValue);
};
</script>

<style scoped>
.thumb {
  top: 0;
  bottom: 0;
  @apply transition-all duration-200 rounded-sm w-6 absolute;
}
.switch-body.off .thumb {
  left: 0;
  background: var(--color-bg-surface);
}

.switch-body.on .thumb {
  left: calc(100% - 1.5rem);
  background: var(--color-primary);
}
</style>
