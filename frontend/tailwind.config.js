const defaultTheme = require('tailwindcss/defaultTheme');

module.exports = {
  content: ['./index.html', './src/**/*.{vue,js,ts,jsx,tsx}'],
  darkMode: 'class',
  theme: {
    screens: {
      xs: '360px',
      ...defaultTheme.screens
    },
    extend: {
      colors: {
        primary: {
          DEFAULT: 'rgb(var(--color-primary) / <alpha-value>)',
          hover: 'rgb(var(--color-primary-hover) / <alpha-value>)',
          'contrast-text': 'rgb(var(--color-primary-contrast-text) / <alpha-value>)',
          text: 'rgb(var(--color-primary-text) / <alpha-value>)'
        },
        bg: {
          DEFAULT: 'rgb(var(--color-bg) / <alpha-value>)',
          hover: 'rgb(var(--color-bg-hover) / <alpha-value>)',
          inverse: 'rgb(var(--color-inverse-bg) / <alpha-value>)'
        },
        surface: {
          DEFAULT: 'rgb(var(--color-bg-surface) / <alpha-value>)',
          hover: 'rgb(var(--color-bg-surface-hover) / <alpha-value>)'
        },
        alt: {
          DEFAULT: 'rgb(var(--color-bg-alt) / <alpha-value>)',
          hover: 'rgb(var(--color-bg-alt-hover) / <alpha-value>)'
        },
        border: {
          DEFAULT: 'rgb(var(--color-border) / <alpha-value>)',
          divider: 'rgb(var(--color-divider) / <alpha-value>)',
          light: 'rgb(var(--color-border-light) / <alpha-value>)'
        },
        text: {
          DEFAULT: 'rgb(var(--color-text) / <alpha-value>)',
          secondary: 'rgb(var(--color-text-secondary) / <alpha-value>)'
        },
        success: {
          DEFAULT: 'rgb(var(--color-success) / <alpha-value>)',
          hover: 'rgb(var(--color-success-hover) / <alpha-value>)',
          text: 'rgb(var(--color-success-text) / <alpha-value>)'
        },
        warning: {
          DEFAULT: 'rgb(var(--color-warning) / <alpha-value>)',
          hover: 'rgb(var(--color-warning-hover) / <alpha-value>)',
          text: 'rgb(var(--color-warning-text) / <alpha-value>)'
        },
        error: {
          DEFAULT: 'rgb(var(--color-error) / <alpha-value>)',
          hover: 'rgb(var(--color-error-hover) / <alpha-value>)',
          text: 'rgb(var(--color-error-text) / <alpha-value>)'
        },
        secondary: {
          DEFAULT: 'rgb(var(--color-secondary) / <alpha-value>)',
          hover: 'rgb(var(--color-secondary-hover) / <alpha-value>)',
          text: 'rgb(var(--color-secondary-text) / <alpha-value>)',
          'contrast-text': 'rgb(var(--color-secondary-contrast-text) / <alpha-value>)'
        }
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
