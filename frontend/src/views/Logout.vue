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
import { ref } from 'vue';
import { useConfig } from '@/core/adapter/config';
import { useRouter } from 'vue-router';

const error = ref<string[]>([]);
const router = useRouter();

const logout = async () => {
  error.value = [];

  try {
    await useConfig().logout();
    router.push('/landing');
  } catch (e: any) {
    error.value = Array.isArray(e) ? e : [e.toString()];
  }
};
logout();
</script>
