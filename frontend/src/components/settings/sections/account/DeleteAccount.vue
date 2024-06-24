<template>
  <div>
    <button
      class="flex justify-center items-center bg-alt border-2 border-transparent px-2 rounded transition hover:bg-alt-hover cursor-pointer select-none"
      @click="openDialog"
    >
      <p class="text-error-text dark:text-error-light-border text-center">{{ t('settings.deleteAccount') }}</p>
    </button>
    <div v-if="showDialog">
      <FloatingDialog content-class="max-w-lg">
        <Form show-close-button @close="showDialog = false">
          <FormTitle>
            {{ t('deleteAccount.title') }}
          </FormTitle>
          <TextInput id="password" v-model="password" label-key="deleteAccount.password" is-password placeholder="******" />
          <FormError :error="error" />
          <FormActions>
            <FormSubmit :disabled="!password.trim() || isLoading" @click="reassureDecision">
              <span class="text-secondary-text">
                {{ t('deleteAccount.delete') }}
              </span>
            </FormSubmit>
          </FormActions>
        </Form>
        <ReassureDialog
          :is-open="reassureDialogOpen"
          :title="t('deleteAccount.reassure.title')"
          :body="t('deleteAccount.reassure.body')"
          :accept="t('deleteAccount.reassure.accept')"
          @close="handleUserDecision"
        />
      </FloatingDialog>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';

import Form from '@/components/forms/Form.vue';
import FormActions from '@/components/forms/FormActions.vue';
import FormError from '@/components/forms/FormError.vue';
import FormSubmit from '@/components/forms/FormSubmit.vue';
import FormTitle from '@/components/forms/FormTitle.vue';
import TextInput from '@/components/forms/TextInput.vue';
import FloatingDialog from '@/components/misc/FloatingDialog.vue';
import ReassureDialog from '@/components/misc/ReassureDialog.vue';
import { useAuth } from '@/core/adapter/auth';

const auth = useAuth();
const router = useRouter();
const { t } = useI18n();

const showDialog = ref(false);
const reassureDialogOpen = ref(false);
const password = ref('');
const error = ref<string[]>([]);
const isLoading = ref<boolean>(false);

const openDialog = () => {
  showDialog.value = true;
};

const reassureDecision = () => {
  reassureDialogOpen.value = true;
};

const handleUserDecision = (accepted: boolean) => {
  if (accepted) {
    reassureDialogOpen.value = false;
    deleteAccount();
  } else {
    reassureDialogOpen.value = false;
    showDialog.value = false;
  }
};

const deleteAccount = async () => {
  error.value = [];
  isLoading.value = true;
  try {
    await auth.deleteAccount(password.value);
    showDialog.value = false;
    router.push('/');
  } catch (e: unknown) {
    error.value = Array.isArray(e) ? e : [(e as Error).toString()];
  }
  isLoading.value = false;
};
</script>
