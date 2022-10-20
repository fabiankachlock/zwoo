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
        <Form show-back-button>
          <FormTitle>
            {{ t('deleteAccount.title') }}
          </FormTitle>
          <TextInput id="password" v-model="password" labelKey="deleteAccount.password" is-password placeholder="******" />
          <FormError :error="error" />
          <FormActions>
            <FormSubmit @click="deleteAccount">
              {{ t('deleteAccount.delete') }}
            </FormSubmit>
          </FormActions>
        </Form>
      </FloatingDialog>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';

import { useAuth } from '@/core/adapter/auth';

import Form from '../forms/Form.vue';
import FormActions from '../forms/FormActions.vue';
import FormError from '../forms/FormError.vue';
import FormSubmit from '../forms/FormSubmit.vue';
import FormTitle from '../forms/FormTitle.vue';
import TextInput from '../forms/TextInput.vue';
import FloatingDialog from '../misc/FloatingDialog.vue';

const auth = useAuth();
const { t } = useI18n();

const showDialog = ref(false);
const password = ref('');
const error = ref<string[]>([]);

const openDialog = () => {
  showDialog.value = true;
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
