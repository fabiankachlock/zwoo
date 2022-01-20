<template>
  <div class="w-full my-2">
    <p class="tc-main-secondary text-right text-sm italic">
      <span v-t="'version'"></span>{{ ': ' }}
      <span v-if="!info.override"> {{ info.version }} ({{ info.hash }})</span>
      <span v-else>{{ info.override }}</span>
    </p>
  </div>
</template>

<script setup lang="ts">
import { useConfig } from '@/core/adapter/config';

const info = useConfig().versionInfo;
</script>
