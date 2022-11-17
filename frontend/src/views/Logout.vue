<template>
  <FormLayout>
    <Form>
      <FormError :error="error" />
    </Form>
  </FormLayout>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';

import { Form, FormError } from '@/components/forms/index';
import { useAuth } from '@/core/adapter/auth';
import FormLayout from '@/layouts/FormLayout.vue';

const router = useRouter();
const auth = useAuth();

const error = ref<string[]>([]);

const logout = async () => {
  error.value = [];

  try {
    await auth.logout();
    router.push('/landing');
  } catch (e: unknown) {
    error.value = Array.isArray(e) ? e : [(e as Error).toString()];
  }
};
logout();
</script>
