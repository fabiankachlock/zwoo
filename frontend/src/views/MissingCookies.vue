<template>
  <div class="max-w-xl mx-auto m-4">
    <h1 class="tc-main text-center text-4xl mb-2">Missing Cookies</h1>
    <p class="tc-main-secondary text-center">You need cookies enabled</p>
    <div class="flex justify-center items-center my-2">
      <div
        class="flex justify-center items-center bg-lightest border-2 border-transparent px-4 py-1 rounded transition hover:bg-light cursor-pointer select-none"
        @click="manageSelection"
      >
        <p class="tc-main-secondary text-center">Manage selection</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useCookies } from '@/core/adapter/cookies';
import { watch } from 'vue';
import { useRouter } from 'vue-router';

const cookies = useCookies();
const router = useRouter();

watch(
  () => cookies.popupOpen,
  open => {
    if (!open) {
      router.push('/home');
    }
  }
);
const manageSelection = () => {
  cookies.popupOpen = true;
};
</script>
