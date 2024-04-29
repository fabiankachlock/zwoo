const colors = require('tailwindcss/colors');
const defaultTheme = require('tailwindcss/defaultTheme');

module.exports = {
  content: ['./public/index.html', './src/**/*.{vue,js,ts,jsx,tsx}'],
  darkMode: 'class',
  theme: {
    screens: {
      xs: '360px',
      ...defaultTheme.screens
    },
    extend: {
      colors: {
        // primary: {
        //   light: '#2CAFF5',
        //   DEFAULT: '#29A4E7',
        //   dark: '#279CDB',
        //   'text-dark': '#1F7AAB',
        //   'text-light': '#2DB2FA'
        //   // light: '#336ECC',
        //   // DEFAULT: '#3066BE',
        //   // dark: '#2D60B3',
        //   // 'text-dark': '#2659AB',
        //   // 'text-light': '#679DF5'
        // },
        // secondary: {
        //   light: '#3C3F8C',
        //   DEFAULT: '#36397F',
        //   dark: '#313473',
        //   'text-dark': '#2F326E',
        //   'text-light': '#5E62DB'
        //   // light: '#B32222',
        //   // DEFAULT: '#A61F1F',
        //   // dark: '#991D1D',
        //   // 'text-dark': '#991A1A',
        //   // 'text-light': '#FF6666'
        // },
        // '_bg-light': {
        //   lightest: '#FFFFFC',
        //   light: '#F2F2F0',
        //   DEFAULT: '#D2D0D9',
        //   dark: '#D9D8D7',
        //   darkest: '#EEEBF5'
        // },
        // '_bg-dark': {
        //   lightest: '#4A4C61',
        //   light: '#404254',
        //   DEFAULT: '#282530',
        //   dark: '#2D2E3B',
        //   darkest: '#15131C'
        // },
        // '_text-light': {
        //   light: '#EDECE8',
        //   DEFAULT: '#E6E5E1',
        //   dark: '#DEDDD9',
        //   secondary: '#BFBEBA'
        // },
        // '_text-dark': {
        //   light: '#191936',
        //   DEFAULT: '#07071C',
        //   dark: '#050514',
        //   secondary: '#49495E'
        // },
        // error: {
        //   'light-bg': colors.red[500] + '80', // add alpha
        //   'light-border': colors.red[500],
        //   'dark-bg': colors.red[700] + '80', // add alpha
        //   'dark-border': colors.red[700],
        //   'light-text': colors.gray[800],
        //   'dark-text': colors.gray[100]
        // }
      },
      fontFamily: {
        sans: "'Fira Sans', sans",
        mono: "'IBM Plex Mono', source-code-pro, Menlo, Monaco, Consolas, 'Courier New', monospace"
      },
      animation: {
        'spin-slow': 'spin 3s linear infinite'
      },
      screens: {
        touch: { raw: '(hover: none)' },
        mouse: { raw: '(hover: hover) and (pointer: fine)' }
      }
    }
  },
  variants: {
    extend: {}
  },
  plugins: []
};
