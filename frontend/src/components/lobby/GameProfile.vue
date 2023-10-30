<script setup lang="ts">
import { useI18n } from 'vue-i18n';

import { GameProfile } from '@/core/adapter/game/features/gameProfiles/useGameProfiles';
import { GameProfileGroup } from '@/core/domain/zrp/zrpTypes';

import { Icon } from '../misc/Icon';

defineProps<{
  profile: GameProfile;
}>();

const emit = defineEmits<{
  (event: 'apply'): void;
  (event: 'rename'): void;
  (event: 'delete'): void;
}>();

const { t } = useI18n();
</script>

<template>
  <div class="flex justify-between items-center my-1 bg-light rounded bc-dark border">
    <p class="pl-2">
      {{ profile.name }}
    </p>
    <div class="flex">
      <button class="rounded m-1 bg-dark hover:bg-darkest tc-main-light flex px-2 items-center group" @click="emit('apply')">
        <p class="mr-1">
          {{ t('rules.applyProfile') }}
        </p>
      </button>
      <button v-if="profile.group === GameProfileGroup.User" class="rounded m-1 bg-dark hover:bg-darkest tc-main-light" @click="emit('rename')">
        <div class="transform transition-transform hover:scale-110 p-1">
          <Icon icon="iconoir:edit-pencil" class="icon text-xl"></Icon>
        </div>
      </button>
      <button v-if="profile.group === GameProfileGroup.User" class="rounded m-1 bg-dark hover:bg-darkest tc-main-light" @click="emit('delete')">
        <div class="transform transition-transform hover:scale-110 p-1">
          <Icon icon="akar-icons:trash-can" class="icon text-xl"></Icon>
        </div>
      </button>
    </div>
  </div>
</template>
