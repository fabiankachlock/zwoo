const { join } = require('path');
const fs = require('fs-extra');
const DATA_PREFIX = 'data:image/svg+xml;base64, ';
const ASSETS_DIR = join(__dirname, '..', 'assets');
const CARDS_DIR = join(ASSETS_DIR, 'cards');
const CARDS_SOURCES = join(CARDS_DIR, 'raw');
const OUT_DIR = join(CARDS_DIR, 'dist');
const PUBLIC_DIR = join(__dirname, '..', 'public');
const THEME_CONFIG_FILENAME = 'zwoo-theme.json';

const VARIANT_DARK = 'dark';
const VARIANT_LIGHT = 'light';
const VARIANT_AUTO = '@auto';

const BaseThemeConfig = {
  name: 'the theme name', // required
  description: 'a theme description', // optional
  author: 'the theme author', // optional
  isMultiLayer: true, // optional,
  variants: ['a', 'list', 'of', 'variants'], // required
  imageType: 'svg+xml', // optional
  overrides: {
    // optional
    cardFront: 'front',
    cardBack: 'back',
    layerPlaceholder: '$',
    encoding: 'base64',
    imageDataPrefix: DATA_PREFIX
  },
  // internal
  _dir: '', // the theme directory,
  _files: {}, // output file names
  _sources: {
    // all theme card paths
    '[variant]': {
      back: [],
      font: []
    }
  }
};

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

function validateThemeConfig(config) {
  return 'name' in config && 'variants' in config;
}

async function findThemes() {
  const allThemes = await fs.readdir(CARDS_SOURCES);
  const themes = [];
  for (const theme of allThemes) {
    try {
      const configPath = join(CARDS_SOURCES, theme, THEME_CONFIG_FILENAME);
      if (!fs.existsSync(configPath)) throw 'not-found';
      const configuration = await fs.readFile(configPath);
      const parsedConfiguration = JSON.parse(configuration.toString());
      if (!validateThemeConfig(parsedConfiguration)) throw 'invalid-config';
      themes.push({
        ...parsedConfiguration,
        _dir: theme
      });
    } catch (e) {
      if (e instanceof SyntaxError) {
        console.error('Invalid theme configuration in ' + theme + ' theme');
        console.trace(e);
        throw new Error('Invalid theme configuration');
      } else if (e === 'not-found') {
        console.error('No theme configuration found in ' + theme);
        console.error('Make sure to include a ' + THEME_CONFIG_FILENAME + 'in your theme directory');
        throw new Error('Theme configuration not found');
      } else if (e === 'invalid-config') {
        console.error('Invalid theme configuration of  ' + theme);
        throw new Error('Invalid theme configuration');
      } else {
        console.trace(e);
      }
    }
  }
  return themes;
}

async function searchThemeSources(themeConfig) {
  const themeSources = {};
  const cardKinds = {
    back: join(CARDS_SOURCES, themeConfig._dir, themeConfig.overrides?.cardBack ?? BaseThemeConfig.overrides.cardBack),
    front: join(CARDS_SOURCES, themeConfig._dir, themeConfig.overrides?.cardFront ?? BaseThemeConfig.overrides.cardFront)
  };
  for (const [kind, kindDirectory] of Object.entries(cardKinds)) {
    for (const variant of themeConfig.variants) {
      if (!fs.existsSync(join(kindDirectory, variant))) {
        console.error('Cannot find directory for ' + variant + ' variant of theme ' + themeConfig.name);
        throw new Error('Cannot find directory ' + variant);
      }
      if (!themeSources[variant]) {
        themeSources[variant] = {};
      }
      const sprites = await fs.readdir(join(kindDirectory, variant));
      themeSources[variant][kind] = sprites.filter(sprite => !sprite.startsWith('.'));
    }
  }
  return {
    ...themeConfig,
    _sources: themeSources,
    _files: themeConfig.variants.reduce((acc, curr) => ({ ...acc, [curr]: themeConfig.name + '.' + curr + '.json' }), {})
  };
}

async function createMetaFiles(themes) {
  const sources = themes.map(t => ({ [t.name]: t._sources })).reduce((acc, curr) => ({ ...acc, ...curr }), {});
  const data = {
    themes: themes.map(t => t.name),
    variants: themes
      .map(theme => ({
        [theme.name]: computeThemeVariants(Object.keys(sources[theme.name]))
      }))
      .reduce((acc, curr) => ({ ...acc, ...curr }), {}),
    files: themes
      .map(theme => ({
        [theme.name]: Object.keys(sources[theme.name]).reduce((acc, curr) => ({ ...acc, [curr]: theme.name + '.' + curr + '.json' }), {})
      }))
      .reduce((acc, curr) => ({ ...acc, ...curr }), {})
  };

  writeJSONFile(join(OUT_DIR, 'meta.json'), data);
  writeJSONFile(join(OUT_DIR, 'sourcemap.json'), sources);
}

/**
 * files: {
 *  back: [],
 *  front: []
 * }
 */
async function createCardSpriteSheet(files, encoding, dataPrefix) {
  const spriteData = {};
  for (const file of files) {
    const paths = file.split('/');
    const spriteName = paths[paths.length - 1].split('.')[0];
    const buffer = await fs.readFile(file);
    const content = buffer.toString(encoding);
    spriteData[spriteName] = dataPrefix + content;
  }
  return spriteData;
}

let outFiles = 0;
async function buildTheme(theme) {
  console.log('building ' + theme.name);
  console.log(' found', theme.variants.length, 'variants (' + theme.variants.join(', ') + ')');
  const themeEncoding = theme.overrides?.encoding ?? BaseThemeConfig.overrides.encoding;
  const themDataPrefix = theme.overrides?.imageDataPrefix ?? `data:image/${theme.imageType ?? BaseThemeConfig.imageType};${themeEncoding},`;

  for (const variant of theme.variants) {
    const fileName = theme._files[variant];
    console.log('   building ' + theme.name + '[' + variant + '] into ' + fileName);
    const buildStart = process.hrtime();
    const basePath = join(CARDS_SOURCES, theme.name);
    const files = [
      ...theme._sources[variant]['back'].map(fileName =>
        join(basePath, theme.overrides?.cardBack ?? BaseThemeConfig.overrides.cardBack, variant, fileName)
      ),
      ...theme._sources[variant]['front'].map(fileName =>
        join(basePath, theme.overrides?.cardFront ?? BaseThemeConfig.overrides.cardFront, variant, fileName)
      )
    ];
    console.log('     found', files.length, 'file(s)');
    console.log('     building...');
    const sheetData = await createCardSpriteSheet(files, themeEncoding, themDataPrefix);
    writeJSONFile(join(OUT_DIR, fileName), sheetData);
    outFiles += 1;
    const buildEnd = process.hrtime(buildStart);
    console.log('     ' + theme.name + '[' + variant + '] successfully build (' + fileName + ') - %dms', buildEnd[1] / 1000000);
  }
}

async function buildCards(themes) {
  for (const theme of themes) {
    await buildTheme(theme);
    console.log('  done');
  }
  console.log('done');
  console.log('build', outFiles, 'sprite maps');
}

(async () => {
  const allStart = process.hrtime();
  console.log('build card assets');
  if (fs.existsSync(OUT_DIR)) {
    await fs.rm(OUT_DIR, { recursive: true });
  }
  await fs.mkdir(OUT_DIR);

  console.log('scanning directories for build files');
  const scanStart = process.hrtime();

  const themes = await findThemes();
  const themesWithSources = [];
  for (const theme of themes) {
    themesWithSources.push(await searchThemeSources(theme));
  }
  const scanEnd = process.hrtime(scanStart);
  console.log('found %d themes (took %dms)', themes.length, scanEnd[1] / 1000000);
  console.log('start build process');

  await createMetaFiles(themesWithSources);
  await buildCards(themesWithSources);

  console.log('cleaning target folder');
  const targetDirectory = join(PUBLIC_DIR, 'assets');
  if (fs.existsSync(targetDirectory)) {
    await fs.rm(targetDirectory, { recursive: true });
  }
  console.log('copying build output');
  await fs.copy(OUT_DIR, targetDirectory);
  const allEnd = process.hrtime(allStart);
  console.log('done - %dms', allEnd[1] / 1000000);
})();
