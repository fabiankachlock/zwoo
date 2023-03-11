<template>
  <MaxWidthLayout size="normal">
    <h1 class="tc-main text-4xl my-5">{{ t('settings.title') }}</h1>
    <div>
      <div>MENU</div>
      <div>
        <router-view></router-view>
      </div>
    </div>
  </MaxWidthLayout>
</template>

<script setup lang="ts">
// import { ref } from 'vue';
import { useI18n } from 'vue-i18n';

import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';

const { t } = useI18n();
// const showDevSettings = ref(localStorage.getItem('zwoo:dev-settings') === 'true');

// let clicks = 0;
// const clickVersion = () => {
//   clicks++;
//   if (clicks === 10) {
//     localStorage.setItem('zwoo:dev-settings', 'true');
//     showDevSettings.value = true;
//   }
// };
</script>
