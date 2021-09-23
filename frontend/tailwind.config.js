module.exports = {
    purge: ['./public/index.html', './src/**/*.{vue,js,ts,jsx,tsx}'],
    darkMode: 'class',
    theme: {
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
                'text-light': 'FF6666'
            },
            '_bg-light': {
                lightest: '#FFFEF7',
                light: '#F2F1EB',
                DEFAULT: '#E5E4DD',
                dark: '#D9D8D2',
                darkest: '#CCCBC6'
            },
            '_bg-dark': {
                lightest: '#3D4161',
                light: '#353954',
                DEFAULT: '#2D3047',
                dark: '#25273B',
                darkest: '#1D1F2E'
            },
            '_text-light': {
                light: '#F0EFEB',
                DEFAULT: '#E6E5E1',
                dark: '#DEDDD9',
                secondary: '#BFBEBA'
            },
            '_text-dark': {
                light: '#191936',
                DEFAULT: '#07071C',
                dark: '#050514',
                secondary: '#4B4B61',
            }
        },
        extend: {
            fontFamily: {
                sans: "'sans', apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Fira Sans', 'Droid Sans', 'Helvetica Neue', sans-serif",
                mono: "'IBM Plex Mono', source-code-pro, Menlo, Monaco, Consolas, 'Courier New', monospace",
            },
        },
    },
    variants: {
        extend: {},
    },
    plugins: [],
}
