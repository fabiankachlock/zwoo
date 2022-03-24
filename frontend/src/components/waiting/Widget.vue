<template>
  <div class="rounded-lg" :class="widgetClass">
    <div class="widget-header flex flex-row flex-nowrap justify-between items-center m-1">
      <div class="widget-title mx-1">
        <p class="text-xl tc-main my-2">{{ t(title) }}</p>
      </div>
      <div class="widget-actions mx-1 flex flex-row flex-nowrap justify-between items-center overflow-hidden">
        <slot name="actions"></slot>
        <button @click="toggleOpenState" class="toggle text-2xl tc-main relative p-4 rounded w-6 h-6 overflow-hidden" :class="buttonClass">
          <Icon
            icon="iconoir:nav-arrow-down"
            class="icon absolute left-1/2 top-1/2 transform -translate-x-1/2 transition duration-300"
            :class="{ 'opacity-0 translate-y-2': isOpen, '-translate-y-1/2': !isOpen }"
          />
          <Icon
            icon="iconoir:nav-arrow-up"
            class="icon absolute left-1/2 top-1/2 transform -translate-x-1/2 transition duration-300"
            :class="{ 'opacity-0 translate-y-2': !isOpen, '-translate-y-1/2': isOpen }"
          />
        </button>
      </div>
    </div>
    <div class="widget-body overflow-hidden transition duration-300" :class="{ open: isOpen }">
      <!-- <div class="divider w-full mb-2 mt-1 bc-invert-darkest border-b"></div> -->
      <div class="content p-2 pt-0">
        <div class="relative">
          <slot></slot>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { defineProps, onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

const props = defineProps<{
  title: string;
  widgetClass?: string;
  buttonClass?: string;
  defaultOpen?: boolean;
}>();

const isOpen = ref(false);

const toggleOpenState = () => {
  isOpen.value = !isOpen.value;
};

onMounted(() => {
  isOpen.value = props.defaultOpen ?? false;
});
</script>

<style scoped>
.widget-body {
  transition-property: max-height;
  max-height: 0;
}
.widget-body.open {
  transition-property: max-height;
  max-height: 5000px;
}

.toggle:hover .icon {
  @apply scale-110;
}
</style>
