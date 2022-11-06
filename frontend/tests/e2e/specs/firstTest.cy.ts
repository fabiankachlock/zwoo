// https://docs.cypress.io/api/introduction/api.html

describe('App loads', () => {
  it('visits the app root url', () => {
    cy.visit(`${Cypress.env('baseUrl')}/`);
    cy.get('h1').first().contains('zwoo');
  });
});
