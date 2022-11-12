<template>
  <button
    v-if="showBackButton"
    @click="goBack"
    id="back-button"
    class="tc-main-secondary bg-light hover:bg-main absolute left-2 top-2 text-xl p-1 rounded-md cursor-pointer"
  >
    <Icon icon="mdi:chevron-left" class="text-1xl transition-transform scale-125" />
  </button>
  <button
    v-if="showCloseButton"
    @click="emit('close')"
    id="close-button"
    class="tc-main-secondary bg-light hover:bg-main absolute right-2 top-2 text-xl p-1 rounded-md cursor-pointer"
  >
    <Icon icon="gg:close" class="text-xl transition-transform" />
  </button>
  <form class="w-full h-full relative">
    <slot></slot>
  </form>
</template>

<script setup lang="ts">
import { defineEmits, defineProps } from 'vue';
import { useRouter } from 'vue-router';

import { Icon } from '@/components/misc/Icon';

defineProps<{
  showBackButton?: boolean;
  showCloseButton?: boolean;
}>();

const emit = defineEmits<{
  (event: 'close'): void;
}>();

const router = useRouter();

const goBack = () => router.go(-1);
</script>

<styles scoped lang="css">
#back-button:hover #icon {
  @apply -translate-x-[2px];
}

#close-button:hover #icon {
  @apply scale-110;
}
</styles>
