<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

import { Checkbox, FormActions, FormError, FormSubmit, TextInput } from '@/components/forms';
import CaptchaButton from '@/components/forms/CaptchaButton.vue';
import Form from '@/components/forms/Form.vue';
import TextArea from '@/components/forms/TextArea.vue';
import FlatDialog from '@/components/misc/FlatDialog.vue';
import { useCookies } from '@/core/adapter/cookies';
import { useApi } from '@/core/adapter/helper/useApi';
import { BackendErrorType, getBackendErrorTranslation } from '@/core/api/ApiError';
import { CaptchaResponse } from '@/core/api/entities/Captcha';
import { RecaptchaValidator } from '@/core/services/validator/recaptcha';
import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';

const { submitContactForm } = useApi();
const { t } = useI18n();
const reCaptchaValidator = new RecaptchaValidator();

const senderName = ref('');
const senderEmail = ref('');
const message = ref('');
const accepted = ref(false);
const acceptedAt = ref(0);
const reCaptchaResponse = ref<CaptchaResponse | undefined>(undefined);
const error = ref<string[]>([]);
const isLoading = ref<boolean>(false);
const isSubmitEnabled = computed(
  () => !isLoading.value && accepted.value && message.value?.trim() && senderEmail.value?.trim() && senderName.value?.trim()
);
const wasSend = ref(false);

onMounted(() => {
  useCookies().loadRecaptcha();
});

const handleToggleAccept = (value: boolean) => {
  if (value) {
    acceptedAt.value = Date.now();
  }
};

const submitForm = async () => {
  error.value = [];
  isLoading.value = true;

  try {
    await submitContactForm({
      name: senderName.value,
      email: senderEmail.value,
      message: message.value,
      acceptedTerms: accepted.value,
      acceptedTermsAt: acceptedAt.value,
      captchaScore: reCaptchaResponse.value?.score ?? 0,
      site: window.location.href
    });
    wasSend.value = true;
  } catch (e: unknown) {
    reCaptchaResponse.value = undefined;
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
        <CaptchaButton
          :validator="reCaptchaValidator"
          :response="reCaptchaResponse"
          @update:response="res => (reCaptchaResponse = res)"
        ></CaptchaButton>
        <Checkbox styles="tc-primary mx-3" v-model="accepted" @update:model-value="handleToggleAccept">
          {{ t('contact.acceptTerms') }}
        </Checkbox>
        <FormError :error="error"></FormError>
        <FormActions>
          <FormSubmit :disabled="!isSubmitEnabled" @click="submitForm">{{ t('contact.send') }}</FormSubmit>
        </FormActions>
      </Form>
    </FlatDialog>
  </MaxWidthLayout>
</template>
