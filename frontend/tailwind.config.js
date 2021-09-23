module.exports = {
    purge: ['./public/index.html', './src/**/*.{vue,js,ts,jsx,tsx}'],
    darkMode: 'class',
    theme: {
        colors: {
            primary: {
                DEFAULT: '#3066be'
            },
            secondary: {
                DEFAULT: '#991d1d'
            },
            '_bg-light': {
                lightest: '#fbfaf3',
                light: '#f0efe8',
                DEFAULT: '#e5e4dd',
                dark: '#dad9d2',
                darkest: '#cfcec7',
            },
            '_bg-dark': {
                lightest: '#43465d',
                light: '#383b52',
                DEFAULT: '#2d3047',
                dark: '#22253c',
                darkest: '#171a31'
            },
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
