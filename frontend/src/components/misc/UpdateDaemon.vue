<template>
  <div v-if="updateExists" class="w-full px-2 my-2 mb-4">
    <div class="bg-dark hover:bg-darkest rounded-lg px-3 py-3">
      <div class="flex justify-between items-center">
        <p class="tc-main">{{ t('settings.update.available') }}</p>
        <div class="bg-light hover:bg-main rounded px-2 py-1 tc-main">
          <p v-if="refreshing">{{ t('settings.update.downloading') }}</p>
          <button v-else @click="runUpdate">{{ t('settings.update.updateNow') }}</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

const registration = ref<ServiceWorkerRegistration | null>(null);
const updateExists = ref<boolean>(false);
const refreshing = ref<boolean>(false);

onMounted(() => {
  // Listen for our custom event from the SW registration
  document.addEventListener('swUpdated', evt => updateAvailable(evt as unknown as { detail: ServiceWorkerRegistration }), { once: true });

  // Prevent multiple refreshes
  navigator.serviceWorker.addEventListener('controllerchange', () => {
    if (refreshing.value) {
      window.location.reload();
    }
  });
});

const updateAvailable = (event: { detail: ServiceWorkerRegistration }) => {
  registration.value = event.detail;
  updateExists.value = true;
};

const runUpdate = () => {
  refreshing.value = true;
  if (!registration.value || !registration.value.waiting) return;
  registration.value.waiting.postMessage({ type: 'SKIP_WAITING' });
};
</script>
