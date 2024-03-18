<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

import { TextInput } from '@/components/forms';
import FloatingDialog from '@/components/misc/FloatingDialog.vue';
import { LocalServerConfig } from '@/core/domain/localServer/localServerConfig';

const { t } = useI18n();
const props = defineProps<{
  config: LocalServerConfig;
}>();

const emit = defineEmits<{
  (event: 'close', newConfig: LocalServerConfig): void;
}>();

onMounted(() => {
  customPort.value = props.config.port.toString();
  selectedPort.value = props.config.useDynamicPort ? 'dynamic' : 'custom';

  customOrigins.value = props.config.allowedOrigins;
  selectedSecurity.value = props.config.useStrictOrigins ? 'restricted' : 'open';

  customIP.value = props.config.ip;
  if (props.config.useAllIps) selectedIP.value = 'all';
  else if (props.config.useLocalhost) selectedIP.value = 'localhost';
  else selectedIP.value = 'custom';
});

const portSelection = ref<HTMLSelectElement>();
const selectedPort = ref<'dynamic' | 'custom'>('custom');
const customPort = ref('');

const ipSelection = ref<HTMLSelectElement>();
const selectedIP = ref<'all' | 'localhost' | 'custom'>('custom');
const customIP = ref('');

const securitySelection = ref<HTMLSelectElement>();
const selectedSecurity = ref<'open' | 'restricted'>('open');
const customOrigins = ref('');

const save = () => {
  const newConfig: LocalServerConfig = {
    serverId: props.config.serverId,
    secretKey: props.config.secretKey,

    ip: customIP.value,
    port: parseInt(customPort.value),

    useAllIps: selectedIP.value === 'all',
    useLocalhost: selectedIP.value === 'localhost',

    useDynamicPort: selectedPort.value === 'dynamic',

    useStrictOrigins: selectedSecurity.value === 'restricted',
    allowedOrigins: customOrigins.value
  };

  emit('close', newConfig);
};
</script>

<template>
  <FloatingDialog>
    <div class="flex flex-col gap-2">
      <div class="flex flex-row justify-between">
        <h1 class="tc-main text-xl">
          {{ t('localServer.config') }}
        </h1>
        <button @click="save">Speichern</button>
      </div>
      <div class="flex flex-row justify-between mx-2">
        <p class="tx-secondary">
          {{ t('localServer.ip') }}
        </p>
        <select
          :ref="
            r => {
              ipSelection = r as HTMLSelectElement;
            }
          "
          v-model="selectedIP"
          class="bg-light p-1 rounded tc-main-dark"
        >
          <option value="custom">{{ t('localServer.ip.custom') }}</option>
          <option value="localhost">{{ t('localServer.ip.localhost') }}</option>
          <option value="all">{{ t('localServer.ip.all') }}</option>
        </select>
      </div>
      <TextInput v-model="customIP" v-if="selectedIP === 'custom'" id="server-ip"></TextInput>

      <div class="flex flex-row justify-between mx-2">
        <p class="tx-secondary">
          {{ t('localServer.port') }}
        </p>
        <select
          :ref="
            r => {
              portSelection = r as HTMLSelectElement;
            }
          "
          v-model="selectedPort"
          class="bg-light p-1 rounded tc-main-dark"
        >
          <option value="dynamic">{{ t('localServer.port.dynamic') }}</option>
          <option value="custom">{{ t('localServer.port.custom') }}</option>
        </select>
      </div>
      <TextInput v-if="selectedPort === 'custom'" v-model="customPort" id="server-port"></TextInput>

      <div class="flex flex-row justify-between mx-2">
        <p class="tx-secondary">
          {{ t('localServer.security') }}
        </p>
        <select
          :ref="
            r => {
              securitySelection = r as HTMLSelectElement;
            }
          "
          v-model="selectedSecurity"
          class="bg-light p-1 rounded tc-main-dark"
        >
          <option value="open">{{ t('localServer.security.open') }}</option>
          <option value="restricted">{{ t('localServer.security.restricted') }}</option>
        </select>
      </div>
      <TextInput v-model="customOrigins" v-if="selectedSecurity === 'restricted'" id="server-origins"></TextInput>
    </div>
  </FloatingDialog>
</template>
