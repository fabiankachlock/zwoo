<template>
  <div class="play-main bg-bg">
    <slot></slot>
  </div>
</template>

<style lang="css" scoped>
.play-main {
  @apply fixed top-0 left-0 right-0 bottom-0;
  height: 100vh;
  max-height: calc(100vh - calc(100vh - 100%));
}
</style>
