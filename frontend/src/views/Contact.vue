<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

import { FormActions, FormError, FormSubmit, TextInput } from '@/components/forms';
import Form from '@/components/forms/Form.vue';
import ReCaptchaButton from '@/components/forms/ReCaptchaButton.vue';
import TextArea from '@/components/forms/TextArea.vue';
import FlatDialog from '@/components/misc/FlatDialog.vue';
import { useCookies } from '@/core/adapter/cookies';
import { ReCaptchaResponse } from '@/core/services/api/Captcha';
import { BackendErrorType, getBackendErrorTranslation } from '@/core/services/api/Errors';
import { RecaptchaValidator } from '@/core/services/validator/recaptcha';
import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';

const { t } = useI18n();
const reCaptchaValidator = new RecaptchaValidator();

const senderName = ref('');
const message = ref('');
const reCaptchaResponse = ref<ReCaptchaResponse | undefined>(undefined);
const error = ref<string[]>([]);
const isLoading = ref<boolean>(false);
const isSubmitEnabled = computed(
  () => !isLoading.value && reCaptchaValidator.validate(reCaptchaResponse.value).isValid && message.value?.trim() && senderName.value?.trim()
);

onMounted(() => {
  useCookies().loadRecaptcha();
});

const submitForm = () => {
  error.value = [];
  isLoading.value = true;

  try {
    // do http request
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
    <p class="tc-main-secondary mb-5">{{ t('contact.info') }}</p>
    <FlatDialog>
      <Form>
        <TextInput id="sender" labelKey="contact.sender" v-model="senderName" :placeholder="t('contact.namePlaceholder')"></TextInput>
        <TextArea id="message" labelKey="contact.message" v-model="message" :placeholder="t('contact.messagePlaceholder')"></TextArea>
        <ReCaptchaButton
          @update:response="res => (reCaptchaResponse = res)"
          :validator="reCaptchaValidator"
          :response="reCaptchaResponse"
        ></ReCaptchaButton>
        <FormError :error="error"></FormError>
        <FormActions>
          <FormSubmit @click="submitForm" :disabled="!isSubmitEnabled">{{ t('contact.send') }}</FormSubmit>
        </FormActions>
      </Form>
    </FlatDialog>
  </MaxWidthLayout>
</template>
