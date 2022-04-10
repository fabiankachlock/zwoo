<template>Redirecting...</template>

<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router';

const route = useRoute();
const router = useRouter();
router.push('/create-account?b=' + route.params['code']);
</script>
