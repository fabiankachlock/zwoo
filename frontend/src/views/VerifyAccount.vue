<template>
  <div>
    <!-- TODO: styling & translations -->
    <div v-if="isLoading"></div>
    <div v-else>
      <p>{{ t(displayText) }}</p>
      <button v-if="isSuccess" @click="goToLogin()"></button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { AuthenticationService } from '@/core/services/api/Authentication';
import { unwrapBackendError } from '@/core/services/api/errors';
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute, useRouter } from 'vue-router';

const route = useRoute();
const router = useRouter();
const { t } = useI18n();

const isLoading = ref(false);
const isSuccess = ref(false);
const displayText = ref('');

onMounted(async () => {
  const id = Array.isArray(route.params['id']) ? route.params['id'][0] : route.params['id'];
  const code = Array.isArray(route.params['code']) ? route.params['code'][0] : route.params['code'];

  const response = await AuthenticationService.verifyAccount(id, code);
  const [success, error] = unwrapBackendError(response);
  isLoading.value = false;
  if (success) {
    displayText.value = 'success';
  } else {
    displayText.value = error?.message ?? 'error'; // TODO: display general error
  }
});

const goToLogin = () => {
  router.push('/login');
};
</script>
