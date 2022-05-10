const fs = require('fs-extra');
const path = require('path');

const Color = {
  Reset: '\x1b[0m',
  Green: '\x1b[32m',
  Red: '\x1b[31m',
  Cyan: '\x1b[36m',
  Yellow: '\x1b[33m'
};

function objectHasKeyPath(object, keyPath) {
  let partialObject = object;
  for (const key of keyPath) {
    partialObject = partialObject[key];
    if (partialObject === undefined && typeof partialObject !== 'object') return false;
  }
  return true;
}

function traverseObjectRecursive(object, keyHandler, existingKeyPath = []) {
  for (const key of Object.keys(object)) {
    if (typeof object[key] === 'object') {
      traverseObjectRecursive(object[key], keyHandler, [...existingKeyPath, key]);
    } else {
      keyHandler([...existingKeyPath, key]);
    }
  }
}

function checkTranslations(source, sourceName, target, targetName) {
  let allFine = true;

  console.log(Color.Cyan + '[%s] checking against %s' + Color.Reset, targetName, sourceName);

  traverseObjectRecursive(source, keypath => {
    const hasEnglishTranslation = objectHasKeyPath(target, keypath);
    if (!hasEnglishTranslation) {
      console.log(Color.Red + '[%s] missing key: %s' + Color.Reset, targetName, keypath.join('.'));
      allFine = false;
    }
  });

  if (allFine) {
    console.log(Color.Green + '[%s] all fine' + Color.Reset, targetName);
  }
  console.log(Color.Cyan + '[%s] done' + Color.Reset, targetName);
  return allFine;
}

async function main() {
  const deFile = path.join(__dirname, '..', 'src', 'locales', 'de.json');
  const enFile = path.join(__dirname, '..', 'src', 'locales', 'en.json');

  const translations = {
    de: await fs.readJSON(deFile),
    en: await fs.readJSON(enFile)
  };
  const languages = Object.keys(translations);

  let allTranslationsOk = true;
  for (const [language, translation] of Object.entries(translations)) {
    console.log(Color.Yellow + 'checking translations: [%s]' + Color.Reset, language);
    for (const otherLanguage of languages) {
      if (otherLanguage === language) continue;
      if (!checkTranslations(translation, language, translations[otherLanguage], otherLanguage)) {
        allTranslationsOk = false;
      }
    }
  }

  process.exit(allTranslationsOk ? 0 : 1);
}

main();
