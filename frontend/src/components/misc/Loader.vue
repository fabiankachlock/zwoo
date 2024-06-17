<script setup lang="ts"></script>
<template>
  <div class="fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 text-7xl">
    <svg class="text-primary" xmlns="http://www.w3.org/2000/svg" width="1em" height="1em" viewBox="0 0 24 24">
      <g id="all">
        <circle cx="12" cy="3" r="0" fill="#E50402">
          <animate
            id="svgSpinners6DotsScaleMiddle0"
            attributeName="r"
            begin="0;svgSpinners6DotsScaleMiddle2.end-0.5s"
            calcMode="spline"
            dur="0.6s"
            keySplines=".27,.42,.37,.99;.53,0,.61,.73"
            values="0;2;0"
          />
        </circle>
        <circle cx="16.5" cy="4.21" r="0" fill="#F03D02">
          <animate
            id="svgSpinners6DotsScaleMiddle1"
            attributeName="r"
            begin="svgSpinners6DotsScaleMiddle0.begin+0.1s"
            calcMode="spline"
            dur="0.6s"
            keySplines=".27,.42,.37,.99;.53,0,.61,.73"
            values="0;2;0"
          />
        </circle>
        <circle cx="7.5" cy="4.21" r="0" fill="#78078B">
          <animate
            id="svgSpinners6DotsScaleMiddle2"
            attributeName="r"
            begin="svgSpinners6DotsScaleMiddle4.begin+0.1s"
            calcMode="spline"
            dur="0.6s"
            keySplines=".27,.42,.37,.99;.53,0,.61,.73"
            values="0;2;0"
          />
        </circle>
        <circle cx="19.79" cy="7.5" r="0" fill="#FA7701">
          <animate
            id="svgSpinners6DotsScaleMiddle3"
            attributeName="r"
            begin="svgSpinners6DotsScaleMiddle1.begin+0.1s"
            calcMode="spline"
            dur="0.6s"
            keySplines=".27,.42,.37,.99;.53,0,.61,.73"
            values="0;2;0"
          />
        </circle>
        <circle cx="4.21" cy="7.5" r="0" fill="#4923B8">
          <animate
            id="svgSpinners6DotsScaleMiddle4"
            attributeName="r"
            begin="svgSpinners6DotsScaleMiddle6.begin+0.1s"
            calcMode="spline"
            dur="0.6s"
            keySplines=".27,.42,.37,.99;.53,0,.61,.73"
            values="0;2;0"
          />
        </circle>
        <circle cx="21" cy="12" r="0" fill="#FCBA02">
          <animate
            id="svgSpinners6DotsScaleMiddle5"
            attributeName="r"
            begin="svgSpinners6DotsScaleMiddle3.begin+0.1s"
            calcMode="spline"
            dur="0.6s"
            keySplines=".27,.42,.37,.99;.53,0,.61,.73"
            values="0;2;0"
          />
        </circle>
        <circle cx="3" cy="12" r="0" fill="#1742EA">
          <animate
            id="svgSpinners6DotsScaleMiddle6"
            attributeName="r"
            begin="svgSpinners6DotsScaleMiddle8.begin+0.1s"
            calcMode="spline"
            dur="0.6s"
            keySplines=".27,.42,.37,.99;.53,0,.61,.73"
            values="0;2;0"
          />
        </circle>
        <circle cx="19.79" cy="16.5" r="0" fill="#CADF06">
          <animate
            id="svgSpinners6DotsScaleMiddle7"
            attributeName="r"
            begin="svgSpinners6DotsScaleMiddle5.begin+0.1s"
            calcMode="spline"
            dur="0.6s"
            keySplines=".27,.42,.37,.99;.53,0,.61,.73"
            values="0;2;0"
          />
        </circle>
        <circle cx="4.21" cy="16.5" r="0" fill="#035FA5">
          <animate
            id="svgSpinners6DotsScaleMiddle8"
            attributeName="r"
            begin="svgSpinners6DotsScaleMiddlea.begin+0.1s"
            calcMode="spline"
            dur="0.6s"
            keySplines=".27,.42,.37,.99;.53,0,.61,.73"
            values="0;2;0"
          />
        </circle>
        <circle cx="16.5" cy="19.79" r="0" fill="#46C30C">
          <animate
            id="svgSpinners6DotsScaleMiddle9"
            attributeName="r"
            begin="svgSpinners6DotsScaleMiddle7.begin+0.1s"
            calcMode="spline"
            dur="0.6s"
            keySplines=".27,.42,.37,.99;.53,0,.61,.73"
            values="0;2;0"
          />
        </circle>
        <circle cx="7.5" cy="19.79" r="0" fill="#03792C">
          <animate
            id="svgSpinners6DotsScaleMiddlea"
            attributeName="r"
            begin="svgSpinners6DotsScaleMiddleb.begin+0.1s"
            calcMode="spline"
            dur="0.6s"
            keySplines=".27,.42,.37,.99;.53,0,.61,.73"
            values="0;2;0"
          />
        </circle>
        <circle cx="12" cy="21" r="0" fill="#0CA01A">
          <animate
            id="svgSpinners6DotsScaleMiddleb"
            attributeName="r"
            begin="svgSpinners6DotsScaleMiddle9.begin+0.1s"
            calcMode="spline"
            dur="0.6s"
            keySplines=".27,.42,.37,.99;.53,0,.61,.73"
            values="0;2;0"
          />
        </circle>
      </g>
      <animateTransform
        attributeType="xml"
        attributeName="transform"
        type="rotate"
        from="0"
        to="360"
        dur="6s"
        additive="sum"
        repeatCount="indefinite"
      />
    </svg>
  </div>
</template>

<style scoped lang="css"></style>
