<template>
  <p>{{ route.params['id'] }}</p>
</template>

<script setup lang="ts">
import { useConfig } from '@/core/adapter/config';
import { useRoute, useRouter } from 'vue-router';

const route = useRoute();
const router = useRouter();
const config = useConfig();

const gameId = route.params['id'];

if (!config.isLoggedIn) {
  router.push('/login?redirect=/join/' + gameId);
}
</script>
