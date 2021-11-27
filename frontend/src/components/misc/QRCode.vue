<template>
  <div>
    <img :src="'https://api.qrserver.com/v1/create-qr-code/?size=' + (width ?? 520) + 'x' + (height ?? 520) + ' &data=' + data" alt="QRCode" />
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue';

defineProps<{
  data: string;
  height?: number;
  width?: number;
}>();
</script>

