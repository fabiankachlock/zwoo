<template>
  <div class="w-full my-2">
    <p class="tc-main-secondary text-right text-sm italic"><span v-t="'version'"></span> {{ info.version }} ({{ info.hash }})</p>
  </div>
</template>

<script setup lang="ts">
import { useConfig } from '@/core/adapter/config';

const info = useConfig().versionInfo;
</script>

