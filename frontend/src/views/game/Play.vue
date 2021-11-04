<template>
  <div class="w-screen h-screen absolute p-3 overflow-hidden">
    <div class="w-full h-full relative">
      <div class="layout-grid">
        <div class="bg-darkest rounded-lg py-1 px-3 flex flex-row justify-between items-center">
          <p class="tc-main text-xl">#GameName#</p>
          <div class="relative">
            <div @click="menuOpen = !menuOpen" class="tc-main text-2xl text-right">
              <Icon v-if="menuOpen" icon="akar-icons:cross"></Icon>
              <Icon v-else icon="akar-icons:chevron-down"></Icon>
            </div>
            <div
              :class="{ 'h-36': menuOpen, 'h-0': !menuOpen }"
              class="fixed top-11 left-0 right-0 w-screen transition-all overflow-hidden px-6 flex flex-nowrap justify-end"
            >
              <div class="bg-darkest w-full max-w-sm menu border bc-invert-main border-t-0">content</div>
            </div>
          </div>
        </div>
        <div class="h-full">Main</div>
        <div>Footer</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { ref } from 'vue';

const menuOpen = ref(false);
</script>

<style>
.layout-grid {
  @apply grid;
  grid-template-rows: min-content auto min-content;
}

.menu {
  border-radius: 0 0 1rem 1rem;
}
</style>
