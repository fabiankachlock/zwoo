const path = require('path');
const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;

module.exports = {
  pluginOptions: {
    i18n: {
      locale: 'en',
      fallbackLocale: 'en',
      localeDir: 'locales',
      enableLegacy: false,
      runtimeOnly: false,
      compositionOnly: false,
      fullInstall: true
    }
  },

  chainWebpack: config => {
    config.module
      .rule('i18n-resource')
      .test(/\.(json5?|ya?ml)$/)
      .include.add(path.resolve(__dirname, './src/locales'))
      .end()
      .type('javascript/auto')
      .use('i18n-resource')
      .loader('@intlify/vue-i18n-loader');

    config.module
      .rule('i18n')
      .resourceQuery(/blockType=i18n/)
      .type('javascript/auto')
      .use('i18n')
      .loader('@intlify/vue-i18n-loader');

    config.plugin('html').tap(args => {
      args[0].title = 'zwoo';
      return args;
    });
  },

  configureWebpack: {
    plugins: process.env.ANALYZE === 'true' ? [new BundleAnalyzerPlugin()] : [],
    resolve: {
      alias: {
        'vue-i18n': 'vue-i18n/dist/vue-i18n.esm-bundler.js'
      }
    },
    module: {
      rules: [
        {
          test: /\.mjs$/,
          include: /node_modules/,
          type: 'javascript/auto'
        }
      ]
    }
  },

  pwa: {
    name: 'zwoo',
    themeColor: '#3066BE',
    msTileColor: '#404254',
    manifestOptions: {
      background_color: '#404254'
    },
    workboxOptions: {
      skipWaiting: true
    }
  }
};
