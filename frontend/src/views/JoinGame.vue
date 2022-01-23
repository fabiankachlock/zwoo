<template>
  <div class="max-w-lg sm:w-full mx-auto h-full">
    <div class="mx-4 sm:mx-0 pb-2">
      <div class="w-full flex flex-row justify-between items-center sticky z-10 bg-main top-10">
        <h2 class="tc-main text-4xl mb-2 py-3">{{ t('join.join', [gameName]) }}</h2>
      </div>
    </div>
  </div>
  <FloatingDialog v-if="needsValidation">
    <Form>
      <FormTitle>
        {{ t('join.enterPassword') }}
      </FormTitle>
      <TextInput id="password" v-model="password" labelKey="join.password" is-password placeholder="******" />
      <FormError :error="error" />
      <FormActions>
        <FormSubmit @click="join">
          {{ t('join.enter') }}
        </FormSubmit>
      </FormActions>
    </Form>
  </FloatingDialog>
</template>

<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import FloatingDialog from '@/components/misc/FloatingDialog.vue';
import { Form, FormActions, FormError, FormSubmit, FormTitle, TextInput } from '@/components/forms/index';
import { onMounted, ref } from 'vue';
import { GameManagementService } from '@/core/services/api/GameManagement';

const { t } = useI18n();
const route = useRoute();
const router = useRouter();
const gameId = route.params['id'] as string;

const gameName = ref(gameId);
const password = ref('');
const needsValidation = ref(false);
const error = ref<string[]>([]);

const join = async () => {
  error.value = [];

  try {
    await GameManagementService.joinGame(gameId, password.value);
    router.push('/game/wait');
  } catch (e: unknown) {
    error.value = Array.isArray(e) ? e : [(e as Error).toString()];
  }
};

onMounted(async () => {
  const gameData = await GameManagementService.getJoinMeta(gameId);

  gameName.value = gameData.name;
  needsValidation.value = gameData.needsValidation;

  if (!gameData.needsValidation) {
    router.replace('/game/wait');
  }
});
</script>
