<template>
  <div v-if="isSupported" class="w-full h-full flex flex-col flex-nowrap p-3">
    <div v-if="cameraOn" class="rounded-sm mx-auto">
      <video :ref="r => (videoElement = r as HTMLVideoElement)" :srcObject="mediaStream" autoplay="true"></video>
    </div>
    <div>
      <p class="tc-main-secondary text-center text-lg my-2">
        {{ status }}
      </p>
    </div>
    <div>
      <div class="flex justify-center items-center my-2">
        <button class="bg-dark tc-main px-6 py-2 rounded hover:bg-darkest transition-transform hover:scale-95" @click="handleButtonClick">
          {{ t(cameraOn ? 'join.closeCam' : 'join.scanCode') }}
        </button>
      </div>
    </div>
  </div>
  <div v-else class="tc-main">Feature is not supported</div>
</template>

<script setup lang="ts">
import { onUnmounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';

import Logger from '@/core/services/logging/logImport';

const emit = defineEmits<{
  (event: 'close'): void;
}>();

const { t } = useI18n();
const router = useRouter();

const cameraOn = ref(false);
const status = ref('');
const isSupported = ref('BarcodeDetector' in window);
const mediaStream = ref<MediaStream | undefined>();
const videoElement = ref<HTMLVideoElement>();

/* eslint-disable */
//  @ts-ignore
const decoder = isSupported.value ? new BarcodeDetector() : undefined;
/* eslint-enable */

onUnmounted(() => {
  if (cameraOn.value) {
    closeCamera();
  }
});

const handleButtonClick = () => {
  if (cameraOn.value) {
    cameraOn.value = false;
    closeCamera();
  } else {
    cameraOn.value = true;
    setTimeout(() => {
      scanQrCode();
    });
  }
};

const getUserMedia = async (): Promise<MediaStream | undefined> => {
  try {
    return await navigator.mediaDevices.getUserMedia({ video: { facingMode: 'environment' } });
  } catch (e: unknown) {
    status.value = t('join.enableCam');
    console.error(e);
    return undefined;
  }
};

const scanQrCode = async () => {
  mediaStream.value = await getUserMedia();
  renderLoop();
};

const closeCamera = async () => {
  cameraOn.value = false;
  mediaStream.value?.getTracks().forEach(track => track.stop());
  setTimeout(() => {
    if (!cameraOn.value) {
      emit('close');
    }
  }, 1000);
};

const render = async () => {
  try {
    if (!videoElement.value) return;
    const barCodes = await decoder?.detect(videoElement.value);
    if (barCodes && barCodes.length > 0) {
      status.value = barCodes[0].rawValue;
      if (validateAndRedirect()) {
        closeCamera();
      }
    }
  } catch (e: unknown) {
    Logger.error(`${JSON.stringify(e)}`);
    status.value = t('join.nothingFound');
  }
};

const renderLoop = () => {
  if (cameraOn.value) {
    requestAnimationFrame(renderLoop);
    render();
  }
};

const joinCodeRegex = /^http(s?):\/\/(zwoo-ui\.web\.app|zwoo\.igd20\.de)\/join\/(.*)$/;

let navigated = false;

const validateAndRedirect = () => {
  const result = joinCodeRegex.exec(status.value);
  if (result && result?.length > 3) {
    if (!navigated) {
      router.push('/join/' + result[3]);
      navigated = true;
    }
    return true;
  }

  status.value = t('join.notValid', [status.value]);
  return false;
};
</script>
