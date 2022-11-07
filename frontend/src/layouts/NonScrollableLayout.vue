<template>
  <div id="_layout__non-scrollable" class="fixed top-0 left-0 right-0 bottom-0 overflow-hidden">
    <slot></slot>
  </div>
</template>

<style lang="css" scoped>
#_layout__non-scrollable {
  height: 100vh;
  max-height: calc(100vh - calc(100vh - 100%)); /* ios workaround */
}
</style>
