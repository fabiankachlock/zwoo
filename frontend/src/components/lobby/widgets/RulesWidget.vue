<template>
  <Widget v-model="isOpen" title="wait.rules" widget-class="bg-light" button-class="bg-main hover:bg-dark">
    <template #actions>
      <div class="flex flex-row">
        <button class="rounded m-1 bg-main hover:bg-dark tc-main-light" @click="safeNewProfileOpen = true">
          <div class="transform transition-transform hover:scale-110 p-1">
            <Icon icon="iconoir:save-floppy-disk" class="icon text-2xl"></Icon>
          </div>
        </button>
        <button class="rounded m-1 mr-2 bg-main hover:bg-dark tc-main-light" @click="openManageDialog">
          <div class="transform transition-transform hover:scale-110 p-1">
            <Icon icon="iconoir:folder" class="icon text-2xl"></Icon>
          </div>
        </button>
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
                <div v-for="profile in profiles" :key="profile.id" class="flex justify-between items-center my-1 bg-light rounded bc-dark border">
                  <p class="pl-2">
                    {{ profile.name }}
                  </p>
                  <div class="flex">
                    <button class="rounded m-1 bg-dark hover:bg-darkest tc-main-light flex px-2 items-center group" @click="applyProfile(profile.id)">
                      <p class="mr-1">
                        {{ t('rules.applyProfile') }}
                      </p>
                      <div class="transform transition-transform group-hover:scale-110 p-1">
                        <Icon icon="iconoir:download" class="icon text-xl"></Icon>
                      </div>
                    </button>
                    <button class="rounded m-1 bg-dark hover:bg-darkest tc-main-light" @click="renameProfile()">
                      <div class="transform transition-transform hover:scale-110 p-1">
                        <Icon icon="iconoir:edit-pencil" class="icon text-xl"></Icon>
                      </div>
                    </button>
                    <button class="rounded m-1 bg-dark hover:bg-darkest tc-main-light" @click="deleteProfile()">
                      <div class="transform transition-transform hover:scale-110 p-1">
                        <Icon icon="akar-icons:trash-can" class="icon text-xl"></Icon>
                      </div>
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </FloatingDialog>
        </div>
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
import { useGameProfiles } from '@/core/adapter/game/features/gameProfiles/useGameProfiles';

import Widget from '../Widget.vue';

const { t } = useI18n();
const gameProfiles = useGameProfiles();
const isOpen = useUserDefaults('lobby:widgetRulesOpen', false);

const safeNewProfileOpen = ref(false);
const manageProfilesOpen = ref(false);
const newProfileName = ref('');
const profiles = computed(() => gameProfiles.profiles);

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
  manageProfilesOpen.value = false;
};

const renameProfile = () => {
  // toodo
  manageProfilesOpen.value = false;
};

const deleteProfile = () => {
  // toodo
  manageProfilesOpen.value = false;
};
</script>
