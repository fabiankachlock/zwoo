<template>
  <button
    v-if="showBackButton"
    @click="goBack"
    class="tc-main-secondary absolute left-3 top-3 text-xl transform transition-transform hover:-translate-x-1"
  >
    <Icon icon="mdi:chevron-left" />
  </button>
  <button
    v-if="showCloseButton"
    @click="emit('close')"
    class="tc-main-secondary absolute right-3 top-3 text-xl transform transition-transform hover:scale-110"
  >
    <Icon icon="gg:close" />
  </button>
  <form class="w-full h-full relative">
    <slot></slot>
  </form>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { useRouter } from 'vue-router';
import { defineProps, defineEmits } from 'vue';

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
