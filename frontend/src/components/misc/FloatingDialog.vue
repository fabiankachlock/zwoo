<script setup lang="ts">
defineProps<{
  contentClass?: string;
  withoutPadding?: boolean;
}>();

const emit = defineEmits<{
  (event: 'clickOutside'): void;
}>();

const handleClick = () => {
  emit('clickOutside');
};

const handleDialogClick = (event: Event) => {
  event.stopPropagation();
};
</script>
<template>
  <div class="fixed inset-0 z-40" @click="handleClick">
    <div class="relative grid place-items-center w-full h-full">
      <div class="absolute inset-0 backdrop-blur"></div>
      <div class="absolute inset-0 backdrop-color z-10"></div>
      <div class="w-full mx-auto z-50" :class="contentClass ?? 'sm:max-w-3xl'">
        <div
          class="frame bg-bg border border-border shadow-md sm:rounded-xl m-3 relative overflow-y-auto"
          :class="{ 'p-5': !withoutPadding }"
          @click="handleDialogClick"
        >
          <slot></slot>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* slightly transparent fallback */
.backdrop-blur {
  background-color: rgb(var(--color-border-light) / 0.5);
}

/* if backdrop support: very transparent and blurred */
@supports ((-webkit-backdrop-filter: blur(8px)) or (backdrop-filter: blur(8px))) {
  .backdrop-blur {
    background-color: transparent;
    backdrop-filter: blur(8px);
  }
}
.backdrop-color {
  background-color: rgb(var(--color-border-light) / 0.3);
}
.frame {
  max-height: calc(90vh - 2.5rem);
}
</style>
