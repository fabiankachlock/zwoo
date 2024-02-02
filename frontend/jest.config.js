module.exports = {
  verbose: true,
  testMatch: ['**/tests/unit/**/*.spec.[jt]s?(x)', '**/__tests__/*.[jt]s?(x)'],
  moduleFileExtensions: ['js', 'ts', 'tsx', 'vue'],
  moduleNameMapper: {
    '^@/(.*)$': '<rootDir>/src/$1'
  },
  transform: {
    '^.+\\.vue$': '@vue/vue3-jest',
    '^.+\\.ts$': [
      'ts-jest',
      {
        babelConfig: true
      }
    ]
  }
};
