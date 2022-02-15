<template>
  <div class="h-32 mt-0.5 py-2 px-3 flex flex-col flex-nowrap items-center overflow-y-auto" style="max-height: 60vh">
    <ChatMessage message="Test0" />
    <ChatMessage message="Test1" />
    <ChatMessage message="Test2" is-own />
    <ChatMessage message="Test3" />
    <ChatMessage message="Test4" is-own />
    <ChatMessage message="Test5" is-own />
    <ChatMessage message="Test6 laiugsdad saui saudal sbasuhdl aidöahdsioahsdoihaodishals" />
    <ChatMessage message="Test6" />
    <ChatMessage message="Test6 asugda sklaioflioashfoiadoihoidahsudhliauhcliuhalshlcaisdhs hskahdliahdlaiusdh jhsdlaiuhdla" is-own />
  </div>
</template>

<script setup lang="ts">
import ChatMessage from './ChatMessage.vue';
</script>
