<template>
  <button @click:prevent="handleClick">
    <p v-if="verifyState === 'none'">###I'm not a robot###</p>
    <div v-if="verifyState === 'verifying'">#Loader#</div>
    <div v-if="verifyState === 'done'">
      <p v-if="!success">###Failed Bot Check###</p>
      <p v-if="success && humanRate < 0.6">###Half cyborg, but ok try acting less like a bot###</p>
      <p v-else>###Your good###</p>
    </div>
  </button>
</template>

<script setup lang="ts">
import { ReCaptchaService } from '@/core/services/api/reCAPTCHA';
import { ref } from 'vue';

const verifyState = ref<'none' | 'verifying' | 'done'>('none');
const humanRate = ref<number>(0);
const success = ref<boolean>(false);

const handleClick = async () => {
  verifyState.value = 'verifying';

  const response = await ReCaptchaService.performCheck();
  verifyState.value = 'done';

  if (response) {
    success.value = response.success;
    humanRate.value = response.score;
  }
};
</script>
