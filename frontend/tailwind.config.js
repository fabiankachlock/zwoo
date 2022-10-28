const colors = require('tailwindcss/colors');

module.exports = {
  content: ['./public/index.html', './src/**/*.{vue,js,ts,jsx,tsx}'],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        primary: {
          light: '#336ECC',
          DEFAULT: '#3066BE',
          dark: '#2D60B3',
          'text-dark': '#2659AB',
          'text-light': '#679DF5'
        },
        secondary: {
          light: '#B32222',
          DEFAULT: '#A61F1F',
          dark: '#991D1D',
          'text-dark': '#991A1A',
          'text-light': '#FF6666'
        },
        '_bg-light': {
          lightest: '#FFFFFC',
          light: '#F2F2F0',
          DEFAULT: '#E6E5E3',
          dark: '#D9D8D7',
          darkest: '#CCCCCA'
        },
        '_bg-dark': {
          lightest: '#4A4C61',
          light: '#404254',
          DEFAULT: '#363847',
          dark: '#2D2E3B',
          darkest: '#23242E'
        },
        '_text-light': {
          light: '#EDECE8',
          DEFAULT: '#E6E5E1',
          dark: '#DEDDD9',
          secondary: '#BFBEBA'
        },
        '_text-dark': {
          light: '#191936',
          DEFAULT: '#07071C',
          dark: '#050514',
          secondary: '#49495E'
        },
        error: {
          'light-bg': colors.red[500] + '80', // add alpha
          'light-border': colors.red[500],
          'dark-bg': colors.red[700] + '80', // add alpha
          'dark-border': colors.red[700],
          'light-text': colors.gray[800],
          'dark-text': colors.gray[100]
        }
      },
      fontFamily: {
        sans: "'sans', apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Fira Sans', 'Droid Sans', 'Helvetica Neue', sans-serif",
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
