<template>
  <div>
    <button @click="handleClick" :class="{ 'pointer-events-none': verifyState !== 'none' && verifyState !== 'error' }">
      <p v-if="verifyState === 'none'">###I'm not a robot###</p>
      <div v-else-if="verifyState === 'verifying'">#Loader#</div>
      <div v-else-if="verifyState === 'error'">#some error occurred in the checking process - retry#</div>
      <div v-else-if="verifyState === 'done'">
        <p v-if="!success">###Failed Bot Check###</p>
        <p v-else-if="success && humanRate < 0.6">###Half cyborg, but ok try acting less like a bot###</p>
        <p v-else>###Your good###</p>
      </div>
    </button>
  </div>
</template>

<script setup lang="ts">
import { ReCaptchaService } from '@/core/services/api/reCAPTCHA';
import { ref } from 'vue';

const verifyState = ref<'none' | 'verifying' | 'done' | 'error'>('none');
const humanRate = ref<number>(0);
const success = ref<boolean>(false);

const handleClick = async (evt: Event) => {
  evt.stopPropagation();
  evt.preventDefault();
  verifyState.value = 'verifying';

  console.log('recaptcha check');
  try {
    const response = await ReCaptchaService.checkUser();
    verifyState.value = 'done';

    if (response) {
      success.value = response.success;
      humanRate.value = response.score;
    }
  } catch {
    verifyState.value = 'error';
  }
};
</script>
