<template>
  <button @click.stop="toggle">
    <div
      class="relative rounded bg-alt hover:bg-alt-hover transition border-2 switch-body"
      :class="{
        'on text-text border-primary': modelValue,
        'off text-text-secondary border-border': !modelValue,
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
  @apply bg-surface;
}

.switch-body.off:hover .thumb {
  @apply bg-surface-hover;
}

.switch-body.on .thumb {
  @apply bg-primary;
  left: calc(100% - 1.5rem);
}
</style>
