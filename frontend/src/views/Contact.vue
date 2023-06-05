<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

import { FormActions, FormError, FormSubmit, TextInput } from '@/components/forms';
import CaptchaButton from '@/components/forms/CaptchaButton.vue';
import Form from '@/components/forms/Form.vue';
import TextArea from '@/components/forms/TextArea.vue';
import FlatDialog from '@/components/misc/FlatDialog.vue';
import { useCookies } from '@/core/adapter/cookies';
import { useApi } from '@/core/adapter/helper/useApi';
import { BackendErrorType, getBackendErrorTranslation } from '@/core/api/ApiError';
import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';

const { submitContactForm } = useApi();
const { t } = useI18n();

const senderName = ref('');
const senderEmail = ref('');
const message = ref('');
const captchaResponse = ref<string | undefined>(undefined);
const error = ref<string[]>([]);
const isLoading = ref<boolean>(false);
const isSubmitEnabled = computed(() => !isLoading.value && message.value?.trim() && senderEmail.value?.trim() && senderName.value?.trim());
const wasSend = ref(false);

onMounted(() => {
  useCookies().loadRecaptcha();
});

const submitForm = async () => {
  error.value = [];
  isLoading.value = true;

  try {
    await submitContactForm({
      name: senderName.value,
      email: senderEmail.value,
      message: message.value,
      captchaToken: captchaResponse.value ?? '',
      site: window.location.href
    });
    wasSend.value = true;
  } catch (e: unknown) {
    captchaResponse.value = undefined;
    setTimeout(() => {
      error.value = Array.isArray(e) ? e : [getBackendErrorTranslation(e as BackendErrorType)];
    });
  }
  isLoading.value = false;
};
</script>

<template>
  <MaxWidthLayout size="small">
    <h1 class="text-4xl tc-main mt-5 mb-3">{{ t('contact.title') }}</h1>
    <p v-if="!wasSend" class="tc-main-secondary mb-5">{{ t('contact.info') }}</p>
    <p v-else class="tc-main-secondary mb-5">{{ t('contact.thanks') }}</p>
    <FlatDialog v-if="!wasSend">
      <Form>
        <TextInput id="sender" labelKey="contact.sender" v-model="senderName" :placeholder="t('contact.namePlaceholder')"></TextInput>
        <TextInput id="email" labelKey="contact.email" v-model="senderEmail" :placeholder="t('contact.emailPlaceholder')"></TextInput>
        <TextArea id="message" labelKey="contact.message" v-model="message" :placeholder="t('contact.messagePlaceholder')"></TextArea>
        <CaptchaButton :token="captchaResponse" @update:response="res => (captchaResponse = res)"></CaptchaButton>
        <FormError :error="error"></FormError>
        <FormActions>
          <FormSubmit :disabled="!isSubmitEnabled" @click="submitForm">{{ t('contact.send') }}</FormSubmit>
        </FormActions>
      </Form>
    </FlatDialog>
  </MaxWidthLayout>
</template>
