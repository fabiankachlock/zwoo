<template>
  <div class="relative w-full p-2 flex flex-col justify-end items-stretch bg-lightest rounded-lg my-4">
    <h3 class="tc-main text-2xl mx-2 my-1" v-if="title">{{ title }}</h3>
    <slot></slot>
  </div>
</template>

<script setup lang="ts">
import { defineProps, toRefs } from 'vue';

const props = defineProps<{
  title?: string;
}>();

const { title } = toRefs(props);
</script>
