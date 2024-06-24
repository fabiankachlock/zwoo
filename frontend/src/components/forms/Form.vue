<template>
  <button
    v-if="showBackButton"
    id="back-button"
    class="text-text-secondary bg-alt hover:bg-alt-hover absolute left-2 top-2 text-xl p-1 rounded-md cursor-pointer"
    @click="goBack"
  >
    <Icon icon="mdi:chevron-left" class="text-1xl transition-transform scale-125" />
  </button>
  <button
    v-if="showCloseButton"
    id="close-button"
    class="text-text-secondary bg-alt hover:bg-alt-hover absolute right-2 top-2 text-xl p-1 rounded-md cursor-pointer"
    @click="emit('close')"
  >
    <Icon icon="gg:close" class="text-xl transition-transform" />
  </button>
  <form class="w-full h-full relative" @submit.prevent>
    <slot></slot>
  </form>
</template>

<script setup lang="ts">
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

<style scoped lang="css">
#back-button:hover #icon {
  @apply -translate-x-[2px];
}

#close-button:hover #icon {
  @apply scale-110;
}
</style>
