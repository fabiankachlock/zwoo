<template>
  <FlatDialog>
    <Form>
      <FormError :error="error" />
    </Form>
  </FlatDialog>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';

import { Form, FormError } from '@/components/forms/index';
import FlatDialog from '@/components/misc/FlatDialog.vue';
import { useAuth } from '@/core/adapter/auth';

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
