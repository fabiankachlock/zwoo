<template>
  <div class="w-full h-12 overflow-hidden">
    <div class="overflow-x-scroll flex flex-row flex-nowrap items-center h-full pb-3">
      <Opponent name="teak" :card-amount="7" :is-active="true" />
      <Opponent name="teaklsdlhajdhaksdst" :card-amount="7" :is-active="true" />
      <Opponent name="iauzsdiuasgdlauhktest2" :card-amount="33" :is-active="false" />
      <Opponent name="iauzsdiuasasdsa" :card-amount="3" :is-active="false" />
      <Opponent name="kduat32k tgkadwakd" :card-amount="13" :is-active="false" />
      <Opponent name="kduat32k " :card-amount="13" :is-active="false" />
      <Opponent name="sadp89z3hiaösudas" :card-amount="13" :is-active="false" />
      <Opponent name="uazdlaiudlsa8zahö" :card-amount="13" :is-active="false" />
    </div>
  </div>
</template>

<script setup lang="ts">
import Opponent from './Opponent.vue';
</script>
