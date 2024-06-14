// vite.config.mts
import vueI18n from "file:///C:/dev/csharp/zwoo/frontend/node_modules/@intlify/unplugin-vue-i18n/lib/vite.mjs";
import vue from "file:///C:/dev/csharp/zwoo/frontend/node_modules/@vitejs/plugin-vue/dist/index.mjs";
import path from "path";
import { visualizer } from "file:///C:/dev/csharp/zwoo/frontend/node_modules/rollup-plugin-visualizer/dist/plugin/index.js";
import { defineConfig } from "file:///C:/dev/csharp/zwoo/frontend/node_modules/vite/dist/node/index.js";
import { VitePWA } from "file:///C:/dev/csharp/zwoo/frontend/node_modules/vite-plugin-pwa/dist/index.js";
var __vite_injected_original_dirname = "C:\\dev\\csharp\\zwoo\\frontend";
var manifestIcons = [
  {
    src: "android-chrome-192x192.png",
    sizes: "192x192",
    type: "image/png",
    purpose: "any"
  },
  {
    src: "android-chrome-512x512.png",
    sizes: "512x512",
    type: "image/png",
    purpose: "any"
  },
  {
    src: "android-chrome-maskable-192x192.png",
    sizes: "192x192",
    type: "image/png",
    purpose: "maskable"
  },
  {
    src: "android-chrome-maskable-512x512.png",
    sizes: "512x512",
    type: "image/png",
    purpose: "maskable"
  },
  {
    src: "apple-touch-icon-180x180.png",
    sizes: "180x180",
    type: "image/png",
    purpose: "any"
  },
  {
    src: "coast-228x228.png",
    sizes: "228x228",
    type: "image/png",
    purpose: "any"
  },
  {
    src: "favicon-16x16.png",
    sizes: "16x16",
    type: "image/png",
    purpose: "any"
  },
  {
    src: "favicon-32x32.png",
    sizes: "32x32",
    type: "image/png",
    purpose: "any"
  },
  {
    src: "favicon-48x48.png",
    sizes: "48x48",
    type: "image/png",
    purpose: "any"
  },
  {
    src: "favicon-64x64.png",
    sizes: "64x64",
    type: "image/png",
    purpose: "any"
  },
  {
    src: "favicon-96x96.png",
    sizes: "96x96",
    type: "image/png",
    purpose: "any"
  },
  {
    src: "favicon-128x128.png",
    sizes: "128x128",
    type: "image/png",
    purpose: "any"
  },
  {
    src: "favicon-256x256.png",
    sizes: "256x256",
    type: "image/png",
    purpose: "any"
  }
].map((icon) => ({
  ...icon,
  src: `/img/icons/${icon.src}`
}));
var vite_config_default = defineConfig(
  ({ mode }) => ({
    plugins: [
      vue(),
      vueI18n({
        compositionOnly: true,
        include: path.resolve(__vite_injected_original_dirname, "./src/locales/**")
      }),
      VitePWA({
        injectRegister: "inline",
        strategies: "generateSW",
        manifest: {
          id: "@zwoo/zwoo",
          name: "zwoo",
          description: "zwoo - The Second Challenge.",
          background_color: "#404254",
          orientation: "any",
          short_name: "zwoo",
          theme_color: "#3066BE",
          start_url: ".",
          display: "standalone",
          icons: manifestIcons
        },
        workbox: {
          globIgnores: ["**/config.json"],
          // dont cache config.json, since it should be dynamic
          globPatterns: ["**/*.{js,css,html,ico,png,svg,json}", "**/wasm/**"],
          cleanupOutdatedCaches: true,
          navigateFallbackDenylist: [/^\/docs/, /^\/api/],
          maximumFileSizeToCacheInBytes: 10 * 1024 * 1024
          // dont cache more than 10 mib
        }
      }),
      ...process.env.ANALYZE !== void 0 ? [
        visualizer({
          open: true,
          template: "treemap"
        })
      ] : []
    ],
    envPrefix: "VUE_APP",
    envDir: "env",
    server: {
      port: 8080
    },
    build: {
      sourcemap: mode === "dev-instance"
    },
    resolve: {
      extensions: [".mjs", ".js", ".ts", ".jsx", ".tsx", ".json", ".vue"],
      alias: {
        "@": path.resolve(__vite_injected_original_dirname, "./src")
      }
    }
  })
);
export {
  vite_config_default as default
};
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsidml0ZS5jb25maWcubXRzIl0sCiAgInNvdXJjZXNDb250ZW50IjogWyJjb25zdCBfX3ZpdGVfaW5qZWN0ZWRfb3JpZ2luYWxfZGlybmFtZSA9IFwiQzpcXFxcZGV2XFxcXGNzaGFycFxcXFx6d29vXFxcXGZyb250ZW5kXCI7Y29uc3QgX192aXRlX2luamVjdGVkX29yaWdpbmFsX2ZpbGVuYW1lID0gXCJDOlxcXFxkZXZcXFxcY3NoYXJwXFxcXHp3b29cXFxcZnJvbnRlbmRcXFxcdml0ZS5jb25maWcubXRzXCI7Y29uc3QgX192aXRlX2luamVjdGVkX29yaWdpbmFsX2ltcG9ydF9tZXRhX3VybCA9IFwiZmlsZTovLy9DOi9kZXYvY3NoYXJwL3p3b28vZnJvbnRlbmQvdml0ZS5jb25maWcubXRzXCI7aW1wb3J0IHZ1ZUkxOG4gZnJvbSAnQGludGxpZnkvdW5wbHVnaW4tdnVlLWkxOG4vdml0ZSc7XG5pbXBvcnQgdnVlIGZyb20gJ0B2aXRlanMvcGx1Z2luLXZ1ZSc7XG5pbXBvcnQgcGF0aCBmcm9tICdwYXRoJztcbmltcG9ydCB7IHZpc3VhbGl6ZXIgfSBmcm9tICdyb2xsdXAtcGx1Z2luLXZpc3VhbGl6ZXInO1xuaW1wb3J0IHsgZGVmaW5lQ29uZmlnLCBVc2VyQ29uZmlnIH0gZnJvbSAndml0ZSc7XG5pbXBvcnQgeyBWaXRlUFdBIH0gZnJvbSAndml0ZS1wbHVnaW4tcHdhJztcblxuY29uc3QgbWFuaWZlc3RJY29ucyA9IFtcbiAge1xuICAgIHNyYzogJ2FuZHJvaWQtY2hyb21lLTE5MngxOTIucG5nJyxcbiAgICBzaXplczogJzE5MngxOTInLFxuICAgIHR5cGU6ICdpbWFnZS9wbmcnLFxuICAgIHB1cnBvc2U6ICdhbnknXG4gIH0sXG4gIHtcbiAgICBzcmM6ICdhbmRyb2lkLWNocm9tZS01MTJ4NTEyLnBuZycsXG4gICAgc2l6ZXM6ICc1MTJ4NTEyJyxcbiAgICB0eXBlOiAnaW1hZ2UvcG5nJyxcbiAgICBwdXJwb3NlOiAnYW55J1xuICB9LFxuICB7XG4gICAgc3JjOiAnYW5kcm9pZC1jaHJvbWUtbWFza2FibGUtMTkyeDE5Mi5wbmcnLFxuICAgIHNpemVzOiAnMTkyeDE5MicsXG4gICAgdHlwZTogJ2ltYWdlL3BuZycsXG4gICAgcHVycG9zZTogJ21hc2thYmxlJ1xuICB9LFxuICB7XG4gICAgc3JjOiAnYW5kcm9pZC1jaHJvbWUtbWFza2FibGUtNTEyeDUxMi5wbmcnLFxuICAgIHNpemVzOiAnNTEyeDUxMicsXG4gICAgdHlwZTogJ2ltYWdlL3BuZycsXG4gICAgcHVycG9zZTogJ21hc2thYmxlJ1xuICB9LFxuICB7XG4gICAgc3JjOiAnYXBwbGUtdG91Y2gtaWNvbi0xODB4MTgwLnBuZycsXG4gICAgc2l6ZXM6ICcxODB4MTgwJyxcbiAgICB0eXBlOiAnaW1hZ2UvcG5nJyxcbiAgICBwdXJwb3NlOiAnYW55J1xuICB9LFxuICB7XG4gICAgc3JjOiAnY29hc3QtMjI4eDIyOC5wbmcnLFxuICAgIHNpemVzOiAnMjI4eDIyOCcsXG4gICAgdHlwZTogJ2ltYWdlL3BuZycsXG4gICAgcHVycG9zZTogJ2FueSdcbiAgfSxcbiAge1xuICAgIHNyYzogJ2Zhdmljb24tMTZ4MTYucG5nJyxcbiAgICBzaXplczogJzE2eDE2JyxcbiAgICB0eXBlOiAnaW1hZ2UvcG5nJyxcbiAgICBwdXJwb3NlOiAnYW55J1xuICB9LFxuICB7XG4gICAgc3JjOiAnZmF2aWNvbi0zMngzMi5wbmcnLFxuICAgIHNpemVzOiAnMzJ4MzInLFxuICAgIHR5cGU6ICdpbWFnZS9wbmcnLFxuICAgIHB1cnBvc2U6ICdhbnknXG4gIH0sXG4gIHtcbiAgICBzcmM6ICdmYXZpY29uLTQ4eDQ4LnBuZycsXG4gICAgc2l6ZXM6ICc0OHg0OCcsXG4gICAgdHlwZTogJ2ltYWdlL3BuZycsXG4gICAgcHVycG9zZTogJ2FueSdcbiAgfSxcbiAge1xuICAgIHNyYzogJ2Zhdmljb24tNjR4NjQucG5nJyxcbiAgICBzaXplczogJzY0eDY0JyxcbiAgICB0eXBlOiAnaW1hZ2UvcG5nJyxcbiAgICBwdXJwb3NlOiAnYW55J1xuICB9LFxuICB7XG4gICAgc3JjOiAnZmF2aWNvbi05Nng5Ni5wbmcnLFxuICAgIHNpemVzOiAnOTZ4OTYnLFxuICAgIHR5cGU6ICdpbWFnZS9wbmcnLFxuICAgIHB1cnBvc2U6ICdhbnknXG4gIH0sXG4gIHtcbiAgICBzcmM6ICdmYXZpY29uLTEyOHgxMjgucG5nJyxcbiAgICBzaXplczogJzEyOHgxMjgnLFxuICAgIHR5cGU6ICdpbWFnZS9wbmcnLFxuICAgIHB1cnBvc2U6ICdhbnknXG4gIH0sXG4gIHtcbiAgICBzcmM6ICdmYXZpY29uLTI1NngyNTYucG5nJyxcbiAgICBzaXplczogJzI1NngyNTYnLFxuICAgIHR5cGU6ICdpbWFnZS9wbmcnLFxuICAgIHB1cnBvc2U6ICdhbnknXG4gIH1cbl0ubWFwKGljb24gPT4gKHtcbiAgLi4uaWNvbixcbiAgc3JjOiBgL2ltZy9pY29ucy8ke2ljb24uc3JjfWBcbn0pKTtcblxuLy8gVE9ETzogY29uZmlndXJlIHJlbG9hZCAoc2V0dGluZ3MgYW5kIGludmFsaWQgdmVyc2lvbik6IGh0dHBzOi8vdml0ZS1wd2Etb3JnLm5ldGxpZnkuYXBwL2d1aWRlL3Byb21wdC1mb3ItdXBkYXRlLmh0bWxcblxuLy8gaHR0cHM6Ly92aXRlanMuZGV2L2NvbmZpZy9cbmV4cG9ydCBkZWZhdWx0IGRlZmluZUNvbmZpZyhcbiAgKHsgbW9kZSB9KSA9PlxuICAgICh7XG4gICAgICBwbHVnaW5zOiBbXG4gICAgICAgIHZ1ZSgpLFxuICAgICAgICB2dWVJMThuKHtcbiAgICAgICAgICBjb21wb3NpdGlvbk9ubHk6IHRydWUsXG4gICAgICAgICAgaW5jbHVkZTogcGF0aC5yZXNvbHZlKF9fZGlybmFtZSwgJy4vc3JjL2xvY2FsZXMvKionKVxuICAgICAgICB9KSxcbiAgICAgICAgVml0ZVBXQSh7XG4gICAgICAgICAgaW5qZWN0UmVnaXN0ZXI6ICdpbmxpbmUnLFxuICAgICAgICAgIHN0cmF0ZWdpZXM6ICdnZW5lcmF0ZVNXJyxcbiAgICAgICAgICBtYW5pZmVzdDoge1xuICAgICAgICAgICAgaWQ6ICdAendvby96d29vJyxcbiAgICAgICAgICAgIG5hbWU6ICd6d29vJyxcbiAgICAgICAgICAgIGRlc2NyaXB0aW9uOiAnendvbyAtIFRoZSBTZWNvbmQgQ2hhbGxlbmdlLicsXG4gICAgICAgICAgICBiYWNrZ3JvdW5kX2NvbG9yOiAnIzQwNDI1NCcsXG4gICAgICAgICAgICBvcmllbnRhdGlvbjogJ2FueScsXG4gICAgICAgICAgICBzaG9ydF9uYW1lOiAnendvbycsXG4gICAgICAgICAgICB0aGVtZV9jb2xvcjogJyMzMDY2QkUnLFxuICAgICAgICAgICAgc3RhcnRfdXJsOiAnLicsXG4gICAgICAgICAgICBkaXNwbGF5OiAnc3RhbmRhbG9uZScsXG4gICAgICAgICAgICBpY29uczogbWFuaWZlc3RJY29uc1xuICAgICAgICAgIH0sXG4gICAgICAgICAgd29ya2JveDoge1xuICAgICAgICAgICAgZ2xvYklnbm9yZXM6IFsnKiovY29uZmlnLmpzb24nXSwgLy8gZG9udCBjYWNoZSBjb25maWcuanNvbiwgc2luY2UgaXQgc2hvdWxkIGJlIGR5bmFtaWNcbiAgICAgICAgICAgIGdsb2JQYXR0ZXJuczogWycqKi8qLntqcyxjc3MsaHRtbCxpY28scG5nLHN2Zyxqc29ufScsICcqKi93YXNtLyoqJ10sXG4gICAgICAgICAgICBjbGVhbnVwT3V0ZGF0ZWRDYWNoZXM6IHRydWUsXG4gICAgICAgICAgICBuYXZpZ2F0ZUZhbGxiYWNrRGVueWxpc3Q6IFsvXlxcL2RvY3MvLCAvXlxcL2FwaS9dLFxuICAgICAgICAgICAgbWF4aW11bUZpbGVTaXplVG9DYWNoZUluQnl0ZXM6IDEwICogMTAyNCAqIDEwMjQgLy8gZG9udCBjYWNoZSBtb3JlIHRoYW4gMTAgbWliXG4gICAgICAgICAgfVxuICAgICAgICB9KSxcbiAgICAgICAgLi4uKHByb2Nlc3MuZW52LkFOQUxZWkUgIT09IHVuZGVmaW5lZFxuICAgICAgICAgID8gW1xuICAgICAgICAgICAgICB2aXN1YWxpemVyKHtcbiAgICAgICAgICAgICAgICBvcGVuOiB0cnVlLFxuICAgICAgICAgICAgICAgIHRlbXBsYXRlOiAndHJlZW1hcCdcbiAgICAgICAgICAgICAgfSlcbiAgICAgICAgICAgIF1cbiAgICAgICAgICA6IFtdKVxuICAgICAgXSxcbiAgICAgIGVudlByZWZpeDogJ1ZVRV9BUFAnLFxuICAgICAgZW52RGlyOiAnZW52JyxcbiAgICAgIHNlcnZlcjoge1xuICAgICAgICBwb3J0OiA4MDgwXG4gICAgICB9LFxuICAgICAgYnVpbGQ6IHtcbiAgICAgICAgc291cmNlbWFwOiBtb2RlID09PSAnZGV2LWluc3RhbmNlJ1xuICAgICAgfSxcbiAgICAgIHJlc29sdmU6IHtcbiAgICAgICAgZXh0ZW5zaW9uczogWycubWpzJywgJy5qcycsICcudHMnLCAnLmpzeCcsICcudHN4JywgJy5qc29uJywgJy52dWUnXSxcbiAgICAgICAgYWxpYXM6IHtcbiAgICAgICAgICAnQCc6IHBhdGgucmVzb2x2ZShfX2Rpcm5hbWUsICcuL3NyYycpXG4gICAgICAgIH1cbiAgICAgIH1cbiAgICB9KSBhcyBVc2VyQ29uZmlnXG4pO1xuIl0sCiAgIm1hcHBpbmdzIjogIjtBQUErUSxPQUFPLGFBQWE7QUFDblMsT0FBTyxTQUFTO0FBQ2hCLE9BQU8sVUFBVTtBQUNqQixTQUFTLGtCQUFrQjtBQUMzQixTQUFTLG9CQUFnQztBQUN6QyxTQUFTLGVBQWU7QUFMeEIsSUFBTSxtQ0FBbUM7QUFPekMsSUFBTSxnQkFBZ0I7QUFBQSxFQUNwQjtBQUFBLElBQ0UsS0FBSztBQUFBLElBQ0wsT0FBTztBQUFBLElBQ1AsTUFBTTtBQUFBLElBQ04sU0FBUztBQUFBLEVBQ1g7QUFBQSxFQUNBO0FBQUEsSUFDRSxLQUFLO0FBQUEsSUFDTCxPQUFPO0FBQUEsSUFDUCxNQUFNO0FBQUEsSUFDTixTQUFTO0FBQUEsRUFDWDtBQUFBLEVBQ0E7QUFBQSxJQUNFLEtBQUs7QUFBQSxJQUNMLE9BQU87QUFBQSxJQUNQLE1BQU07QUFBQSxJQUNOLFNBQVM7QUFBQSxF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
