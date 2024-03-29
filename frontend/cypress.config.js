const { defineConfig } = require('cypress');

module.exports = defineConfig({
  projectId: '1yq2ff',
  fixturesFolder: 'tests/e2e/fixtures',
  screenshotsFolder: 'tests/e2e/screenshots',
  videosFolder: 'tests/e2e/videos',
  env: {
    baseUrl: 'http://localhost:8080'
  },
  screenshotOnRunFailure: true,
  e2e: {
    // We've imported your old cypress plugins here.
    // You may want to clean this up later by importing these.
    setupNodeEvents(on, config) {
      return require('./tests/e2e/plugins/index.js')(on, config);
    },

    supportFile: 'tests/e2e/support/index.js',
    specPattern: 'tests/e2e/specs/**/*.cy.{js,jsx,ts,tsx}'
  }
});
