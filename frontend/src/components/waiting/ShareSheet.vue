<template>
  <h3 class="text-xl tc-main my-2">{{ t('sharesheet.title') }}</h3>
  <p class="my-1 text-sm italic tc-main-secondary">
    {{ t('sharesheet.info') }}
  </p>
  <div v-if="!canShare" class="">
    <div>
      <div>twitter, etc</div>
    </div>
  </div>
  <div class="error-message">
    <Error v-if="error" :errors="[error]" />
  </div>
</template>

<script setup lang="ts">
import { useShare } from '@/composables/Share';
import { useGameConfig } from '@/core/adapter/game';
import { onMounted, ref, defineEmits } from 'vue';
import { useI18n } from 'vue-i18n';
import Error from '../misc/Error.vue';

const emit = defineEmits<{
  (event: 'shouldClose'): void;
}>();

const game = useGameConfig();
const { t } = useI18n();
const { share, canShare } = useShare();
const error = ref<string | undefined>(undefined);

onMounted(() => {
  if (canShare.value) {
    try {
      share({
        title: `join ${game.name}`,
        text: 'you got invited into a zwoo game, use the link to join now!',
        url: `http://localhost:8080/join/${game.gameId}`
      })
        .catch((err: string) => {
          console.log(err);
          error.value = err;
        })
        .then(() => {
          emit('shouldClose');
        });
    } catch (e: any) {
      error.value = e.toString();
    }
  }
});
</script>
