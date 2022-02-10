<template>
  <FloatingDialog>
    <div>
      Consent to all that tasty cookies
      {{ cookies.recaptchaAllowed }}
    </div>
  </FloatingDialog>
</template>

<script setup lang="ts">
import { useCookies } from '@/core/adapter/cookies';
import FloatingDialog from '../misc/FloatingDialog.vue';

const cookies = useCookies();
</script>
