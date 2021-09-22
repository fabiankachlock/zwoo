<template>
  <div id="main">
    <div id="header-wrapper">
      <header>
        <div id="row">
          <h1 id="title">zwoo</h1>
          <div id="menu">
            <div @click="toggleMenu" id="menu-button">
              <Icon :icon="icon" />
            </div>
          </div>
        </div>
      </header>
    </div>
    <p style="font-size: var(--f-s)">Test</p>
    <p style="font-size: var(--f-m)">Test</p>
    <p style="font-size: var(--f-l)">Test</p>
    <p style="font-size: var(--f-xl)">Test</p>
    <p style="font-size: var(--f-xxl)">Test</p>
    <router-view />
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { ref, computed } from 'vue';

const isMenuOpen = ref(false);
const icon = computed(() => (isMenuOpen.value ? 'mdi:menu-open' : 'mdi:menu'));

const toggleMenu = () => {
  isMenuOpen.value = !isMenuOpen.value;
};
</script>

<style scoped>
#main {
  width: 100%;
  height: 100%;
  position: relative;
  overflow-x: hidden;
  overflow-y: scroll;
}

#header-wrapper {
  height: 48px;
  background-color: var(--bg-darker);
  border-bottom: 1px solid var(--primary);
  box-shadow: 0px 3px 5px 2px var(--shadow-dark);
}

#row {
  height: 100%;
  margin: 0 1rem;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-direction: row;
}

header {
  height: 100%;
}

#title {
  font-size: var(--f-xxl);
  color: var(--primary);
}

#menu {
  height: 100%;
  display: flex;
  align-items: center;
}

#menu-button {
  display: none;
  height: var(--f-xl);
  font-size: var(--f-xl);
  transition: 200ms linear all;
  color: var(--primary);
}

@media screen and (min-width: 768px) {
  #header-wrapper {
    height: 60px;
  }
  #menu-button {
    display: block;
  }
}
</style>
