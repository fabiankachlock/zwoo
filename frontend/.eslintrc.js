module.exports = {
  root: true,
  env: {
    es2021: true
  },

  extends: ['plugin:vue/vue3-essential', 'eslint:recommended', '@vue/typescript/recommended', '@vue/prettier', '@vue/eslint-config-typescript'],
  parserOptions: {
    ecmaVersion: 2021,
    parser: '@typescript-eslint/parser'
  },
  rules: {
    'no-unused-vars': 'off',
    '@typescript-eslint/no-unused-vars': ['error'],
    'no-console': process.env.NODE_ENV === 'production' ? 'warn' : 'off',
    'no-debugger': process.env.NODE_ENV === 'production' ? 'warn' : 'off',
    'no-control-regex': 'off',
    'sort-imports': 'off',
    '@typescript-eslint/ban-types': 'warn',
    'vue/multi-word-component-names': 'off'
  },
  overrides: [
    {
      files: ['**/__tests__/*.{j,t}s?(x)', '**/tests/unit/**/*.(spec|test).{j,t}s?(x)'],
      env: {
        jest: true
      }
    }
  ]
};
