<template>
  <FloatingDialog>
    <div class="flex flex-col mx-2 overflow-y-scroll">
      <h2 class="tc-main text-xl text-center my-2">Consent to all those tasty cookies</h2>
      <div class="bc-darkest border-t border-b py-1">
        <h3 class="tc-main text-lg">Functional cookies</h3>
        <ul class="ml-4 list">
          <li class="bc-main">
            <div>
              <h4 class="text tc-main">Authentication Cookies</h4>
              <p class="text-xs tc-main-secondary ml-2 mb-1">Use for session and login</p>
            </div>
            <div class="bg-main border-2 border-transparent px-4 py-1 rounded cursor-not-allowed select-none">
              <p class="tc-main-secondary flex flex-row flex-nowrap justify-end items-center">
                <span> Active </span>
                <Icon icon="akar-icons:check" class="ml-2" />
              </p>
            </div>
          </li>
          <li class="bc-main">
            <div>
              <h4 class="text tc-main">Google Recaptcha Cookies</h4>
              <p class="text-xs tc-main-secondary ml-2 mb-1">Google reCAPTCHA Service</p>
            </div>
            <div
              class="bg-main border-2 border-transparent px-4 py-1 rounded transition hover:bg-dark cursor-pointer select-none"
              :class="{ 'tc-primary': recaptchaActivated, 'tc-main-secondary': !recaptchaActivated }"
              @click="recaptchaActivated = !recaptchaActivated"
            >
              <p class="flex flex-row flex-nowrap justify-end items-center">
                <span> Active </span>
                <Icon v-if="recaptchaActivated" icon="akar-icons:check" class="ml-2" />
                <Icon v-else icon="akar-icons:cross" class="ml-2" />
              </p>
            </div>
          </li>
        </ul>
      </div>
      <div class="grid grid-cols-2 gap-2 mt-4">
        <div class="bg-light border-2 border-transparent px-4 py-1 rounded transition hover:bg-main cursor-pointer select-none">
          <p class="tc-main-secondary text-center">Accept Selection</p>
        </div>
        <div class="bg-main border-2 border-primary px-4 py-1 rounded transition hover:bg-dark cursor-pointer select-none">
          <p class="tc-primary text-center">Accept All</p>
        </div>
      </div>
    </div>
  </FloatingDialog>
</template>

<script setup lang="ts">
import FloatingDialog from '../misc/FloatingDialog.vue';
import { Icon } from '@iconify/vue';
import { ref } from 'vue';

const recaptchaActivated = ref(true);
</script>

<style>
.list li {
  @apply flex flex-row flex-nowrap justify-between items-center my-1 border-b;
}
</style>
