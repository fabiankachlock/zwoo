<template>
  <div class="w-full h-full relative flex flex-col flex-nowrap">
    <header class="m-2">
      <div class="w-full rounded-lg bg-darkest flex flex-row px-2 py-1 justify-center flex-wrap items-center">
        <p class="text-3xl tc-main font-bold m-2 text-center">#GameName#</p>
        <div class="space flex-1"></div>
        <div class="actions flex flex-row items-center justify-center m-2">
          <button class="tc-main-dark bg-primary hover:bg-primary-dark transition">#start#</button>
          <button class="tc-main-dark bg-secondary hover:bg-secondary-dark transition">#close#</button>
          <button class="tc-main-dark bg-secondary hover:bg-secondary-dark transition">#leave#</button>
        </div>
      </div>
    </header>
    <main class="m-2 relative">
      <div class="main-content grid gap-2 grid-cols-1 md:grid-cols-2 mx-auto max-w-5xl">
        <div class="bg-lightest md:row-span-2">&lt; Players &gt;</div>
        <div class="bg-lightest">&lt; Rules &gt;</div>
        <div class="bg-lightest">&lt; QRCode &gt;</div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts"></script>

<style>
.actions button {
  @apply mx-1 px-2 py-1 rounded-sm;
}

.main-content div {
  @apply rounded-md p-3;
}
</style>
