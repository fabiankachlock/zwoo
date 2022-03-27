<template>
  <div class="main-card-wrapper absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 h-full" style="max-height: 70%">
    <img src="/img/cards/__default__/back/dark_upright.png" class="main-card" alt="card" />
  </div>
</template>

<script setup lang="ts"></script>

<style scoped>
.main-card {
  width: auto;
  height: auto;
  max-width: 100%;
  max-height: 100%;
}

.main-card-wrapper {
  max-width: 40vw;
}

@media only screen and (min-width: 440px) {
  .main-card-wrapper {
    max-width: 30vw;
  }
}

@media only screen and (min-width: 620px) {
  .main-card-wrapper {
    max-width: 20vw;
  }
}
</style>
