<template>
  <div class="w-screen h-screen absolute p-3 overflow-hidden">
    <div class="w-full h-full relative">
      <div class="layout-grid">
        <div class="bg-darkest rounded-lg py-1 px-3 flex flex-row justify-between items-center">
          <p class="tc-main text-xl">#GameName#</p>
          <div class="relative">
            <div @click="menuOpen = !menuOpen" class="tc-main text-2xl text-right cursor-pointer">
              <Icon v-if="menuOpen" icon="akar-icons:cross"></Icon>
              <Icon v-else icon="akar-icons:chevron-down"></Icon>
            </div>
            <div
              :class="{ 'h-28': menuOpen, 'h-0': !menuOpen }"
              class="fixed left-0 right-0 w-screen transition-all overflow-hidden px-6 flex flex-nowrap justify-end"
            >
              <div class="bg-darkest w-full max-w-sm menu-rounded-edges">
                <div class="h-1.5"></div>
                <div class="menu-rounded-edges menu-content border bc-invert-main border-t-0 py-1">
                  <div class="flex flex-col flex-nowrap">
                    <div class="menu-section">
                      <p class="tc-main text-lg">#Options#</p>
                      <div class="menu-options-container flex flex-row justify-end">
                        <DarkModeSwitch />
                        <FullScreenSwitch />
                      </div>
                    </div>
                    <hr class="bc-invert-lightest opacity-40 my-1" />
                    <div class="menu-section">
                      <p class="tc-main text-lg">#Actions#</p>
                      <div>
                        <button @click="cardsActive = !cardsActive" class="tc-main-dark bg-secondary hover:bg-secondary-dark mx-1 px-2 py-1 rounded">
                          #Leave#
                        </button>
                      </div>
                    </div>
                    <!--
                      <hr class="bc-lightest" />
                    <div>chat</div>
                    -->
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="h-full">Main</div>
        <div class="bg-darkest rounded-lg py-1 px-3 h-12 relative">
          <div class="absolute left-0 right-0 bottom-2 z-10 overflow-hidden px-2 flex flex-nowrap justify-center">
            <img
              v-for="card of cards"
              :key="card"
              :style="getComputedCardStyle()"
              :class="{ active: cardsActive, idle: !cardsActive }"
              class="card"
              src="/img/dummy_card.svg"
              alt=""
            />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { onMounted, onUnmounted, reactive, ref } from 'vue';
import DarkModeSwitch from '@/components/settings/DarkModeSwitch.vue';
import FullScreenSwitch from '@/components/settings/FullScreenSwitch.vue';

const CARD_ASPECT_RATIO = 476 / 716;

const menuOpen = ref(false);
const cardsActive = ref(false);
const dimensions = reactive({
  width: window.innerWidth,
  height: window.innerHeight
});
const cardWidth = ref(0);
const deckWidth = ref(0);

onMounted(() => {
  window.addEventListener('resize', onResize);
  onResize();
});

onUnmounted(() => {
  window.removeEventListener('resize', onResize);
});

const onResize = () => {
  dimensions.height = window.innerHeight;
  dimensions.width = window.innerWidth;
  deckWidth.value = dimensions.width - 60;
  calculateCardWidth();
};

const calculateCardWidth = () => {
  const cardMaxWidth = dimensions.width * 0.25;
  const cardMaxHeight = dimensions.height * 0.3;
  const cardCalculatedHeight = cardMaxWidth / CARD_ASPECT_RATIO;
  const cardCalculatedWidth = cardMaxHeight * CARD_ASPECT_RATIO;

  if (cardCalculatedHeight > cardMaxHeight) {
    cardWidth.value = cardCalculatedWidth;
  } else {
    cardWidth.value = cardMaxWidth;
  }
};

const cards = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
const getComputedCardStyle = (): { [key: string]: string } => {
  return {
    width: Math.round(deckWidth.value / cards.length).toString() + 'px'
  };
};
</script>

<style>
.card {
  max-height: 30vh;
  max-width: 25vw;
  @apply transition-all;
}

.card.active {
  transform: translateY(30%);
}

.card.idle {
  transform: translateY(70%);
  opacity: 0.8;
  filter: grayscale(0.5);
}

.layout-grid {
  @apply grid h-full w-full;
  grid-template-rows: min-content auto min-content;
}

.menu-rounded-edges {
  border-radius: 0 0 1rem 1rem;
}

.menu-content {
  height: calc(100% - 0.375rem - 1px); /* - h-1.5 - border-bottom*/
}

.menu-options-container > * {
  @apply mx-1;
}

.menu-section {
  @apply flex justify-between items-center m-2;
}
</style>
