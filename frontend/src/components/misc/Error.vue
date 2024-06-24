<template>
  <div class="w-full p-1 rounded bg-error/50 al border-2 border-error">
    <pre class="text-text text-xs italic error-value">{{ errorText }}</pre>
  </div>
</template>

<script setup lang="ts">
import { computed, toRefs } from 'vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
const props = defineProps<{
  errors: string[];
}>();
const { errors } = toRefs(props);

const transformError = (key: string) => `${t('errors.error')}: ${t(key)}`;
const errorText = computed(() => errors.value.map(transformError).join('\n'));
</script>

<style scoped>
.error-value {
  white-space: normal;
}
</style>
