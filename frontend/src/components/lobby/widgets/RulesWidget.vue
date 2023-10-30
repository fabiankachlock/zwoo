<template>
  <Widget v-model="isOpen" title="wait.rules" widget-class="bg-light" button-class="bg-main hover:bg-dark">
    <template #actions>
      <div v-if="isHost" class="flex flex-row items-center">
        <div v-if="activeProfile" class="tc-main m1 mr-2 py-0.5 px-1 border bc-darkest bg-lightest rounded flex items-center">
          <p>
            {{ activeProfile.name }}
          </p>
          <button class="p-1 ml-1 transform transition-transform hover:scale-110" @click="activeProfile = undefined">
            <Icon icon="gg:close" />
          </button>
        </div>
        <button class="rounded m-1 bg-main hover:bg-dark tc-main-light" @click="saveProfile">
          <div class="transform transition-transform hover:scale-110 p-1">
            <Icon icon="iconoir:save-floppy-disk" class="icon text-2xl"></Icon>
          </div>
        </button>
        <button class="rounded m-1 mr-2 bg-main hover:bg-dark tc-main-light" @click="openManageDialog">
          <div class="transform transition-transform hover:scale-110 p-1">
            <Icon icon="iconoir:folder" class="icon text-2xl"></Icon>
          </div>
        </button>
      </div>
      <div v-if="safeNewProfileOpen">
        <FloatingDialog content-class="sm:max-w-lg">
          <div class="absolute top-2 right-2 z-10">
            <button class="bg-lightest hover:bg-light p-1.5 tc-main-dark rounded" @click="safeNewProfileOpen = false">
              <Icon icon="akar-icons:cross" class="text-xl" />
            </button>
          </div>
          <div class="relative">
            <Form>
              <FormTitle>
                {{ t('rules.newProfile') }}
              </FormTitle>
              <p class="my-1 text-sm italic tc-main-secondary px-2">
                {{ t('rules.profileInfo') }}
              </p>
              <TextInput id="profile-name" label-key="rules.profileName" :placeholder="t('rules.profilePlaceholder')" v-model="newProfileName" />
              <FormActions>
                <FormSubmit @click="safeNewProfile()">
                  {{ t('rules.createProfile') }}
                </FormSubmit>
              </FormActions>
            </Form>
          </div>
        </FloatingDialog>
      </div>
      <div v-if="manageProfilesOpen">
        <FloatingDialog content-class="sm:max-w-lg">
          <div class="absolute top-2 right-2 z-10">
            <button class="bg-lightest hover:bg-light p-1.5 tc-main-dark rounded" @click="manageProfilesOpen = false">
              <Icon icon="akar-icons:cross" class="text-xl" />
            </button>
          </div>
          <div class="relative tc-main">
            <h3 class="text-xl tc-main my-2">{{ t('rules.manageProfiles') }}</h3>
            <div class="flex flex-col">
              <h4>{{ t('rules.groupSystem') }}</h4>
              <GameProfileVue
                v-for="profile in profiles.filter(p => p.group === GameProfileGroup.System)"
                :key="profile.id"
                :profile="profile"
                @apply="applyProfile(profile.id)"
                @delete="deleteProfile(profile.id)"
                @rename="renameProfile()"
              />
              <h4>{{ t('rules.groupUser') }}</h4>
              <GameProfileVue
                v-for="profile in profiles.filter(p => p.group === GameProfileGroup.User)"
                :key="profile.id"
                :profile="profile"
                @apply="applyProfile(profile.id)"
                @delete="deleteProfile(profile.id)"
                @rename="renameProfile()"
              />
            </div>
          </div>
        </FloatingDialog>
      </div>
    </template>
    <Rules />
  </Widget>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { useI18n } from 'vue-i18n';

import { Form, FormActions, FormSubmit, FormTitle, TextInput } from '@/components/forms';
import Rules from '@/components/lobby/rules/Rules.vue';
import FloatingDialog from '@/components/misc/FloatingDialog.vue';
import { Icon } from '@/components/misc/Icon';
import { useUserDefaults } from '@/composables/userDefaults';
import { GameProfile, useGameProfiles } from '@/core/adapter/game/features/gameProfiles/useGameProfiles';
import { useIsHost } from '@/core/adapter/game/util/userRoles';
import { GameProfileGroup } from '@/core/domain/zrp/zrpTypes';

import GameProfileVue from '../GameProfile.vue';
import Widget from '../Widget.vue';

const { t } = useI18n();
const gameProfiles = useGameProfiles();
const isOpen = useUserDefaults('lobby:widgetRulesOpen', false);
const { isHost } = useIsHost();

const safeNewProfileOpen = ref(false);
const manageProfilesOpen = ref(false);
const newProfileName = ref('');

const profiles = computed(() => gameProfiles.profiles);
const activeProfile = ref<GameProfile | undefined>(undefined);

const saveProfile = () => {
  if (activeProfile.value && activeProfile.value.group === GameProfileGroup.User) {
    gameProfiles.updateGameProfile(activeProfile.value.id);
  } else {
    safeNewProfileOpen.value = true;
  }
};

const safeNewProfile = () => {
  gameProfiles.safeToProfile(newProfileName.value);
  newProfileName.value = '';
  safeNewProfileOpen.value = false;
};

const openManageDialog = () => {
  manageProfilesOpen.value = true;
  gameProfiles.loadProfiles();
};

const applyProfile = (id: string) => {
  gameProfiles.applyProfile(id);
  activeProfile.value = gameProfiles.profiles.find(p => p.id === id);
  manageProfilesOpen.value = false;
};

const renameProfile = () => {
  // toodo
  manageProfilesOpen.value = false;
};

const deleteProfile = (id: string) => {
  gameProfiles.deleteProfile(id);
};
</script>
