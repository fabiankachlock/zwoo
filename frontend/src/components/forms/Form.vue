<template>
  <div @click="goBack" class="tc-main-secondary absolute left-3 top-3 text-xl transform transition-transform hover:-translate-x-1">
    <Icon icon="mdi:chevron-left" />
  </div>
  <form class="w-full h-full relative">
    <slot></slot>
  </form>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { useRouter } from 'vue-router';

const router = useRouter();

const goBack = () => router.go(-1);
</script>
