<template>
  <img
    :src="'https://api.qrserver.com/v1/create-qr-code/?size=' + (width ?? 520) + 'x' + (height ?? 520) + ' &data=' + data"
    alt="QRCode"
    :class="imageClass"
  />
</template>

<script setup lang="ts">
defineProps<{
  data: string;
  height?: number;
  width?: number;
  imageClass?: string;
}>();
</script>
