const path = require('path');
const fs = require('fs-extra');
const DATA_PREFIX = 'data:image/svg+xml;base64, ';
const ASSETS_DIR = path.join(__dirname, '..', 'assets');
const CARDS_DIR = path.join(ASSETS_DIR, 'cards');
const CARDS_SOURCES = path.join(CARDS_DIR, 'raw');
const OUT_DIR = path.join(CARDS_DIR, 'dist');
const PUBLIC_DIR = path.join(__dirname, '..', 'public');

const VARIANT_DARK = 'dark';
const VARIANT_LIGHT = 'light';
const VARIANT_AUTO = '@auto';

function computeThemeVariants(variants) {
  if (variants.includes(VARIANT_DARK) && variants.includes(VARIANT_LIGHT)) {
    return [...variants, VARIANT_AUTO];
  }
  return variants;
}

async function writeJSONFile(path, data) {
  if (!fs.existsSync(path)) {
    await fs.createFile(path);
  } else {
    await fs.remove(path);
    await fs.createFile(path);
  }
  await fs.writeFile(path, JSON.stringify(data, null, 2));
}

async function scanCardFiles() {
  const allThemes = await fs.readdir(CARDS_SOURCES);
  const sources = {};

  for (const themeDir of allThemes) {
    const themeCards = {};
    const cardKinds = {
      back: path.join(CARDS_SOURCES, themeDir, 'back'),
      front: path.join(CARDS_SOURCES, themeDir, 'front')
    };
    for (const [kind, kindDirectory] of Object.entries(cardKinds)) {
      const variants = await fs.readdir(kindDirectory);
      for (const variant of variants) {
        if (!themeCards[variant]) {
          themeCards[variant] = {};
        }
        const sprites = await fs.readdir(path.join(kindDirectory, variant));
        themeCards[variant][kind] = sprites.filter(sprite => !sprite.startsWith('.') && sprite.endsWith('svg'));
      }
    }
    sources[themeDir] = themeCards;
  }

  const data = {
    themes: allThemes,
    variants: allThemes
      .map(theme => ({
        [theme]: computeThemeVariants(Object.keys(sources[theme]))
      }))
      .reduce((acc, curr) => ({ ...acc, ...curr }), {}),
    files: allThemes
      .map(theme => ({
        [theme]: Object.keys(sources[theme]).reduce((acc, curr) => ({ ...acc, [curr]: theme + '.' + curr + '.json' }), {})
      }))
      .reduce((acc, curr) => ({ ...acc, ...curr }), {})
  };

  writeJSONFile(path.join(OUT_DIR, 'meta.json'), data);
  writeJSONFile(path.join(OUT_DIR, 'sourcemap.json'), sources);
  return [data, sources];
}

/**
 * files: {
 *  back: [],
 *  front: []
 * }
 */
async function createCardSpriteSheet(files) {
  const spriteData = {};
  for (const file of files) {
    const filePath = file.split('.')[0];
    const paths = filePath.split('/');
    const spriteName = paths[paths.length - 1];
    const buffer = await fs.readFile(file);
    const content = buffer.toString('base64');
    // const replacedContent = content.replace(/"/g, "'");
    spriteData[spriteName] = DATA_PREFIX + content;
  }
  return spriteData;
}

async function buildCards() {
  if (fs.existsSync(OUT_DIR)) {
    await fs.rm(OUT_DIR, { recursive: true });
  }
  await fs.mkdir(OUT_DIR);

  console.log('scanning directories for build files');
  const scanStart = process.hrtime();
  const [meta, sources] = await scanCardFiles();
  const scanEnd = process.hrtime(scanStart);
  console.log('found %d themes (took %dms)', meta.themes.length, scanEnd[1] / 1000000);
  let outFiles = 0;
  for (const theme of Object.keys(meta.files)) {
    console.log(' found', Object.keys(meta.files[theme]).length, 'variants of', theme, '(' + Object.keys(meta.files[theme]).join(', ') + ')');
    for (const variant of Object.keys(meta.files[theme])) {
      const fileName = meta.files[theme][variant];
      console.log('   building ' + theme + '[' + variant + '] into ' + fileName);
      const buildStart = process.hrtime();
      const basePath = path.join(ASSETS_DIR, 'cards', 'raw', theme);
      const files = [
        ...sources[theme][variant]['back'].map(fileName => path.join(basePath, 'back', variant, fileName)),
        ...sources[theme][variant]['front'].map(fileName => path.join(basePath, 'front', variant, fileName))
      ];
      console.log('     found', files.length, 'file(s)');
      console.log('     building...');
      const sheetData = await createCardSpriteSheet(files);
      writeJSONFile(path.join(OUT_DIR, fileName), sheetData);
      outFiles += 1;
      const buildEnd = process.hrtime(buildStart);
      console.log('     ' + theme + '[' + variant + '] successfully build (' + fileName + ') - %dms', buildEnd[1] / 1000000);
    }
  }
  console.log('done');
  console.log('build', outFiles, 'sprite maps');
}

(async () => {
  const allStart = process.hrtime();
  console.log('build card assets');
  await buildCards();

  console.log('cleaning target folder');
  const targetDirectory = path.join(PUBLIC_DIR, 'assets');
  if (fs.existsSync(targetDirectory)) {
    await fs.rm(targetDirectory, { recursive: true });
  }
  console.log('copying build output');
  await fs.copy(OUT_DIR, targetDirectory);
  const allEnd = process.hrtime(allStart);
  console.log('done - %dms', allEnd[1] / 1000000);
})();
