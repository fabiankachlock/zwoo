<template>
  <FlatDialog>
    <Form>
      <FormError :error="error" />
    </Form>
  </FlatDialog>
</template>

<script setup lang="ts">
import { Form, FormError } from '@/components/forms/index';
import FlatDialog from '@/components/misc/FlatDialog.vue';
import { useAuth } from '@/core/adapter/auth';
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const auth = useAuth();

const error = ref<string[]>([]);

const logout = async () => {
  error.value = [];

  try {
    await auth.logout();
    router.push('/landing');
  } catch (e: any) {
    error.value = Array.isArray(e) ? e : [e.toString()];
  }
};
logout();
</script>
