<template>
  <MaxWidthLayout size="normal" class="imprint text-text-secondary break-words">
    <h1 class="text-text">Impressum</h1>

    <p class="pb-4 italic">ZWOO ist eine Marke der IGD 2.0 UG (haftungsbeschränkt)</p>

    <p>Angaben gemäß § 5 TMG</p>

    <p>IGD 2.0 UG (haftungsbeschränkt)</p>

    <p>
      Alnpeckstraße 4 <br />
      01239 Dresden <br />
      Deutschland
    </p>

    <p>
      <span class="text-base font-bold">Vertreten durch den Geschäftsführer:</span><br />
      Thore Georgi
    </p>

    <p>
      <span class="text-base font-bold">Kontakt:</span><br />
      <a href="mailto:info@igd20.de"> Email: <span style="text-decoration: underline">info@igd20.de</span> </a>
      <br />
      <a style="text-decoration: underline" href="/contact">Kontakt-Formular</a>
    </p>

    <p>
      <span class="text-base font-bold">Registereintrag:</span><br />
      Registergericht: Amtsgericht Dresden<br />
      Registernummer: HRB 43974
    </p>

    <h2 class="text-text">Haftungsausschluss:</h2>

    <h3 class="text-text">Haftung für Inhalte</h3>

    <p>
      Die Inhalte unserer Seiten wurden mit größter Sorgfalt erstellt. Für die Richtigkeit, Vollständigkeit und Aktualität der Inhalte können wir
      jedoch keine Gewähr übernehmen. Als Diensteanbieter sind wir gemäß § 7 Abs.1 TMG für eigene Inhalte auf diesen Seiten nach den allgemeinen
      Gesetzen verantwortlich. Nach §§ 8 bis 10 TMG sind wir als Diensteanbieter jedoch nicht verpflichtet, übermittelte oder gespeicherte fremde
      Informationen zu überwachen oder nach Umständen zu forschen, die auf eine rechtswidrige Tätigkeit hinweisen. Verpflichtungen zur Entfernung oder
      Sperrung der Nutzung von Informationen nach den allgemeinen Gesetzen bleiben hiervon unberührt. Eine diesbezügliche Haftung ist jedoch erst ab
      dem Zeitpunkt der Kenntnis einer konkreten Rechtsverletzung möglich. Bei Bekanntwerden von entsprechenden Rechtsverletzungen werden wir diese
      Inhalte umgehend entfernen.
    </p>

    <h3 class="text-text">Haftung für Links</h3>

    <p>
      Unser Angebot enthält Links zu externen Webseiten Dritter, auf deren Inhalte wir keinen Einfluss haben. Deshalb können wir für diese fremden
      Inhalte auch keine Gewähr übernehmen. Für die Inhalte der verlinkten Seiten ist stets der jeweilige Anbieter oder Betreiber der Seiten
      verantwortlich. Die verlinkten Seiten wurden zum Zeitpunkt der Verlinkung auf mögliche Rechtsverstöße überprüft. Rechtswidrige Inhalte waren zum
      Zeitpunkt der Verlinkung nicht erkennbar. Eine permanente inhaltliche Kontrolle der verlinkten Seiten ist jedoch ohne konkrete Anhaltspunkte
      einer Rechtsverletzung nicht zumutbar. Bei Bekanntwerden von Rechtsverletzungen werden wir derartige Links umgehend entfernen.
    </p>

    <h3 class="text-text">Urheberrecht</h3>

    <p>
      Die durch die Seitenbetreiber erstellten Inhalte und Werke auf diesen Seiten unterliegen dem deutschen Urheberrecht. Die Vervielfältigung,
      Bearbeitung, Verbreitung und jede Art der Verwertung außerhalb der Grenzen des Urheberrechtes bedürfen der schriftlichen Zustimmung des
      jeweiligen Autors bzw. Erstellers. Downloads und Kopien dieser Seite sind nur für den privaten, nicht kommerziellen Gebrauch gestattet. Soweit
      die Inhalte auf dieser Seite nicht vom Betreiber erstellt wurden, werden die Urheberrechte Dritter beachtet. Insbesondere werden Inhalte Dritter
      als solche gekennzeichnet. Sollten Sie trotzdem auf eine Urheberrechtsverletzung aufmerksam werden, bitten wir um einen entsprechenden Hinweis.
      Bei Bekanntwerden von Rechtsverletzungen werden wir derartige Inhalte umgehend entfernen.
    </p>

    <h3 class="text-text">Datenschutz</h3>

    <p>
      Die Nutzung unserer Webseite ist in der Regel ohne Angabe personenbezogener Daten möglich. Soweit auf unseren Seiten personenbezogene Daten
      (beispielsweise Name, Anschrift oder eMail-Adressen) erhoben werden, erfolgt dies, soweit möglich, stets auf freiwilliger Basis. Diese Daten
      werden ohne Ihre ausdrückliche Zustimmung nicht an Dritte weitergegeben.
    </p>
    <router-link to="/privacy"> Unsere Datenschutzerklärung befindet sich <span class="underline">hier</span> </router-link>
  </MaxWidthLayout>
</template>

<script setup lang="ts">
import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';
</script>

<style scoped>
.imprint p {
  @apply my-2 text-lg;
}

.imprint h1 {
  @apply mt-4 mb-2 text-4xl;
}

.imprint h2 {
  @apply mt-4 mb-2 text-2xl;
}

.imprint h3 {
  @apply mt-4 mb-2 text-xl;
}
</style>
