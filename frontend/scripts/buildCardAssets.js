const path = require('path');
const fs = require('fs-extra');
const DATA_PREFIX = 'data:image/svg+xml';
const ASSETS_DIR = path.join(__dirname, '..', 'assets');
const CARDS_DIR = path.join(ASSETS_DIR, 'cards');
const CARDS_SOURCES = path.join(CARDS_DIR, 'raw');
const OUT_DIR = path.join(CARDS_DIR, 'dist');
const PUBLIC_DIR = path.join(__dirname, '..', 'public');

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
  const themes = await fs.readdir(CARDS_SOURCES);
  const sources = {};

  for (const dir of themes) {
    const configKind = {
      back: path.join(CARDS_SOURCES, dir, 'back'),
      front: path.join(CARDS_SOURCES, dir, 'front')
    };
    const cards = {
      dark: {
        back: [],
        front: []
      },
      light: {
        back: [],
        front: []
      }
    };
    for (const [kind, dir] of Object.entries(configKind)) {
      const configTheme = {
        dark: path.join(dir, 'dark'),
        light: path.join(dir, 'light')
      };
      for (const [theme, dir] of Object.entries(configTheme)) {
        const sprites = await fs.readdir(dir);
        cards[theme][kind] = sprites.filter(
          sprite => !sprite.startsWith('.') && sprite.endsWith('svg')
        );
      }
    }
    sources[dir] = cards;
  }

  const data = {
    themes,
    files: themes
      .map(theme => ({
        [theme]: { dark: theme + '.dark.json', light: theme + '.light.json' }
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
    spriteData[spriteName] = DATA_PREFIX + buffer.toString();
  }
  return spriteData;
}

async function buildCards() {
  if (fs.existsSync(OUT_DIR)) {
    await fs.rm(OUT_DIR, { recursive: true });
  }
  await fs.mkdir(OUT_DIR);

  console.log('scanning directories for build files');
  const [meta, sources] = await scanCardFiles();
  console.log('found', meta.themes.length, 'themes');
  for (const theme of Object.keys(meta.files)) {
    console.log('found', Object.keys(meta.files[theme]).length, 'variants of', theme);
    for (const variant of Object.keys(meta.files[theme])) {
      const fileName = meta.files[theme][variant];
      console.log(
        'building ' + theme + ':' + variant + ' -> ' + fileName
      );
      const basePath = path.join(ASSETS_DIR, 'cards', 'raw', theme);
      const files = [
        ...sources[theme][variant]['back'].map(fileName =>
          path.join(basePath, 'back', variant, fileName)
        ),
        ...sources[theme][variant]['front'].map(fileName =>
          path.join(basePath, 'front', variant, fileName)
        )
      ];
      console.log('found', files.length, 'file(s)');
      const sheetData = await createCardSpriteSheet(files);
      writeJSONFile(path.join(OUT_DIR, fileName), sheetData);
    }
  }
}

(async () => {
  await buildCards();

  console.log('cleaning target folder');
  const targetDirectory = path.join(PUBLIC_DIR, 'assets');
  if (fs.existsSync(targetDirectory)) {
    await fs.rm(targetDirectory, { recursive: true });
  }
  console.log('copying build output');
  await fs.copy(OUT_DIR, targetDirectory);
})();
