<template>
  <div
    @click="toggleOpenState"
    v-tooltip="t(isOpen ? 'wait.collapse' : 'wait.expand')"
    class="px-2 py-1 my-1 bg-dark border bc-darkest transition hover:bc-primary rounded-lg hover:bg-darkest cursor-pointer"
  >
    <div>
      <p class="text-lg tc-main-dark">{{ t(title) }}</p>
      <div>
        <slot></slot>
      </div>
    </div>
    <div class="rule-body overflow-hidden transition duration-300" :class="{ open: isOpen }">
      <div class="divider w-full my-2 bc-invert-darkest border-b"></div>
      <div class="content">
        <p class="text-sm italic tc-main-secondary">
          {{ t(description) }}
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { defineProps, ref } from 'vue';
import { useI18n } from 'vue-i18n';

defineProps<{
  title: string;
  description: string;
}>();

const { t } = useI18n();
const isOpen = ref(false);

const toggleOpenState = () => {
  isOpen.value = !isOpen.value;
};
</script>

<style scoped>
.rule-body {
  transition-property: max-height;
  max-height: 0;
}
.rule-body.open {
  transition-property: max-height;
  max-height: 200px;
}
</style>
