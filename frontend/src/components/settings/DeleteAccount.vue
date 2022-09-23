<template>
  <div>
    <button
      class="flex justify-center items-center bg-light border-2 border-transparent px-2 rounded transition hover:bg-main cursor-pointer select-none"
      @click="openDialog"
    >
      <p class="text-error-dark-border dark:text-error-light-border text-center">{{ t('settings.deleteAccount') }}</p>
    </button>
    <div v-if="showDialog">
      <FloatingDialog>
        <Form show-close-button @close="showDialog = false">
          <FormTitle>
            {{ t('deleteAccount.title') }}
          </FormTitle>
          <TextInput id="password" v-model="password" labelKey="deleteAccount.password" is-password placeholder="******" />
          <FormError :error="error" />
          <FormActions>
            <FormSubmit @click="reassureDecision">
              <span class="tc-secondary">
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
import { useAuth } from '@/core/adapter/auth';
import FloatingDialog from '../misc/FloatingDialog.vue';
import Form from '../forms/Form.vue';
import FormTitle from '../forms/FormTitle.vue';
import TextInput from '../forms/TextInput.vue';
import FormActions from '../forms/FormActions.vue';
import FormError from '../forms/FormError.vue';
import FormSubmit from '../forms/FormSubmit.vue';
import ReassureDialog from '../misc/ReassureDialog.vue';

const auth = useAuth();
const { t } = useI18n();

const showDialog = ref(false);
const reassureDialogOpen = ref(false);
const password = ref('');
const error = ref<string[]>([]);

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
  try {
    await auth.deleteAccount(password.value);
    showDialog.value = false;
  } catch (e: unknown) {
    error.value = Array.isArray(e) ? e : [(e as Error).toString()];
  }
};
</script>
